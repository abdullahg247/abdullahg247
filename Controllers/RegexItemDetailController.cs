using Controllers.Annotations;
using DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Controllers
{
    public class RegexItemDetailController : INotifyPropertyChanged
    {
        private Double _timeEllapsedInMiliSec;

        public Double TimeElapsedInMiliSec
        {
            get { return _timeEllapsedInMiliSec; }
            set
            {

                _timeEllapsedInMiliSec = value;
                OnPropertyChanged(nameof(TimeElapsedInMiliSec));
            }
        }


        private RegexData m_regexData;
        private string _patteren;

        public string Patteren
        {
            get { return _patteren; }
            set
            {
                _patteren = value;
                OnPropertyChanged(nameof(Patteren));
            }
        }


        public RegexData RegexData
        {
            get { return m_regexData; }
            set
            {
                m_regexData = value;
                OnPropertyChanged(nameof(RegexData));
            }
        }

        private bool m_isNotrunning = true;

        public bool IsNotrunning
        {
            get { return m_isNotrunning = true; }
            set
            {
                m_isNotrunning = value;
                OnPropertyChanged(nameof(IsNotrunning));
            }
        }


        public RegexItemDetailController(RegexData regexData)
        {
            RegexData = regexData;
            this.Patteren = regexData.CustomPatteren;
        }
        public async void ParseData()
        {
            await Task.Run(() =>
            {
                IsNotrunning = false;
                if (!string.IsNullOrEmpty(RegexData.Text) && !string.IsNullOrEmpty(RegexData.CustomPatteren))
                {

                    Stopwatch sw = Stopwatch.StartNew();
                    try
                    {
                        RegexData.Regex = new Regex(RegexData.CustomPatteren, RegexOptions.IgnoreCase);
                        var matches = RegexData.Regex.Matches(RegexData.Text);
                        if (matches?.Count > 0)
                        {
                            RegexData.Matches = new ObservableCollection<Match>(matches.Cast<Match>());
                        }
                    }
                    catch
                    {

                    }
                    sw.Stop();
                    TimeElapsedInMiliSec = sw.Elapsed.Milliseconds;
                    IsNotrunning = true;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
