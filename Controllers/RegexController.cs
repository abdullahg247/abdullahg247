using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Controllers.Annotations;
using DataModels;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Controllers
{
    public class RegexController : INotifyPropertyChanged
    {
        private string myVar;

        public string MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        private double _totalSecs;

        public double TotalSecs
        {
            get { return _totalSecs; }
            set
            {
                _totalSecs = value;
                OnPropertyChanged(nameof(TotalSecs));
            }
        }


        private bool _isThreadNotRunning = true;

        public bool IsThreadNotRunning
        {
            get { return _isThreadNotRunning; ; }
            set
            {
                _isThreadNotRunning = value;
                OnPropertyChanged(nameof(IsThreadNotRunning));
            }
        }

        private int _foundCount;

        public int FoundCount
        {
            get
            {
                _foundCount = Convert.ToInt32(RegexDataCollection?.Count(x => x.Matches?.Count > 0));
                return _foundCount;
            }
        }

        private int _notFoundCount;

        public int NotFoundCount
        {
            get
            {
                _notFoundCount = Convert.ToInt16(RegexDataCollection?.Count()) - _foundCount;
                return _notFoundCount;
            }
        }

        private string _mFilePath;

        public string FilePath
        {
            get => _mFilePath;
            set
            {
                _mFilePath = value;
                OnPropertyChanged((nameof(FilePath)));
            }
        }

        public void NotifyCounts()
        {
            OnPropertyChanged(nameof(FoundCount));
            OnPropertyChanged(nameof(NotFoundCount));
        }

        private string _patteren;
        public string Patteren
        {
            get => _patteren;
            set
            {
                _patteren = value;
                OnPropertyChanged(nameof(Patteren));
            }
        }

        private ObservableCollection<RegexData> _regexDataCollection;
        private readonly Action<Exception> _showMessagesFromController;
        private string filterContent;

        public ObservableCollection<RegexData> RegexDataCollection
        {
            get
            {

                IEnumerable<RegexData> filteredCol = null;
                if (!string.IsNullOrEmpty(filterContent))
                {

                    if (filterContent == "Found")
                        filteredCol = _regexDataCollection.Where(x => x.Matches?.Count > 0);
                    else if (filterContent == "Not Found")
                        filteredCol = _regexDataCollection.Where(x => x.Matches == null || x.Matches?.Count <= 0);
                }
                if (!string.IsNullOrEmpty(FilterText))
                {
                    if (filteredCol != null)
                    {
                        filteredCol = filteredCol.Where(p => Convert.ToString(p.DocumentId).Contains(FilterText));
                    }
                    else
                    {
                        filteredCol = _regexDataCollection.Where(p => Convert.ToString(p.DocumentId).Contains(FilterText));
                    }
                }
                if (filteredCol != null)
                {
                    if (filteredCol.Count() > 0)
                    {
                        return new ObservableCollection<RegexData>(filteredCol);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                    return _regexDataCollection;
            }
            set
            {
                _regexDataCollection = value;
                OnPropertyChanged(nameof(RegexDataCollection));
            }
        }

        public string FilterText { get; set; }

        public void ApplyFilter(string content)
        {
            filterContent = content;
            OnPropertyChanged(nameof(RegexDataCollection));
        }

        public RegexController()
        {
            RegexDataCollection = new ObservableCollection<RegexData>();
        }

        public RegexController(Action<Exception> showMessagesFromController)
        {
            this._showMessagesFromController = showMessagesFromController;
            RegexDataCollection = new ObservableCollection<RegexData>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async void LoadDataFromPath(Action<RegexData> methodFromUi)
        {
            if (string.IsNullOrEmpty(FilePath) && string.IsNullOrWhiteSpace(FilePath))
            {
                var choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Excel files Excel (*.xlsx)|*.xlsx|(*.xls)|*.xls";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = true;
                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    this.FilePath = choofdlog.FileName;
                }
            }
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
                {
                    try
                    {
                        IsThreadNotRunning = false;
                        //var xlApp = new Excel.Application();
                        //var xlWorkbook = xlApp.Workbooks.Open(FilePath);
                        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(FilePath, false))
                        {
                            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                            //Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                            var rows = sheetData.Descendants<Row>();
                            var rowCount = rows.Count();
                            //var colCount = 2;
                            RegexDataCollection = new ObservableCollection<RegexData>();
                            #region commented
                            foreach (var row in rows)
                            {
                                var regexData = new RegexData();
                                int celId = 0;
                                foreach (Cell cell in row.Descendants<Cell>())
                                {

                                    if (celId == 0)
                                    {
                                        regexData.DocumentId = GetValue(spreadsheetDocument, cell);
                                    }
                                    else
                                    {
                                        regexData.Text += "" + GetValue(spreadsheetDocument, cell);
                                    }
                                    celId++;
                                }
                                methodFromUi(regexData);
                            }
                            //for (var i = 1; i <= rowCount; i++)
                            //{
                            //    var regexData = new RegexData();
                            //    for (var j = 1; j <= colCount; j++)
                            //    {
                            //        rows.ElementAt(i)
                            //        if (rows.ElementAt(i).Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                            //        {
                            //            if (j % 2 == 0)
                            //            {
                            //                regexData.Text = xlRange.Cells[i, j].Value2.ToString();
                            //            }
                            //            else
                            //            {
                            //                regexData.DocumentId = Convert.ToInt32(xlRange.Cells[i, j].Value2.ToString());
                            //            }
                            //        }

                            //    }
                            //    methodFromUi(regexData);
                            //}
                            #endregion
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            //Marshal.ReleaseComObject(xlRange);
                            //Marshal.ReleaseComObject(xlWorksheet);
                            //xlWorkbook.Close();
                            //Marshal.ReleaseComObject(xlWorkbook);
                            // xlApp.Quit();
                            // Marshal.ReleaseComObject(xlApp);
                        }
                    }
                    catch (Exception ex)
                    {
                        _showMessagesFromController(ex);
                    }
                }
                NotifyCounts();
                IsThreadNotRunning = true;
            });

        }
        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }
        public async void ParseData()
        {
            await Task.Run(() =>
            {
                IsThreadNotRunning = false;
                if (!string.IsNullOrEmpty(this.Patteren) && this.RegexDataCollection?.Count > 0)
                {
                    TotalSecs = 0.0;
                    foreach (var item in this.RegexDataCollection)
                    {
                        MatchCollection matchCollection = null;
                        var ptrn = !string.IsNullOrEmpty(item.CustomPatteren) && !string.IsNullOrWhiteSpace(item.CustomPatteren) ? item.CustomPatteren : Patteren;
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            if (!string.IsNullOrEmpty(item.Text))
                            {
                                item.Regex = new Regex(ptrn, RegexOptions.IgnoreCase);
                                matchCollection = item.Regex.Matches(item.Text);
                            }
                        }
                        catch (Exception ex)
                        {
                            _showMessagesFromController(ex);
                        }
                        sw.Stop();
                        item.TimeSpan = sw.Elapsed;
                        if (matchCollection?.Count > 0)
                        {
                            item.Matches = new ObservableCollection<Match>(matchCollection.Cast<Match>());
                        }
                        else
                        {
                            item.Matches = new ObservableCollection<Match>();
                        }
                        TotalSecs += (Math.Round(item.TimeSpan.TotalMilliseconds / 1000, 6));
                    }
                    NotifyCounts();

                }
                IsThreadNotRunning = true;
            });
        }
    }
}
