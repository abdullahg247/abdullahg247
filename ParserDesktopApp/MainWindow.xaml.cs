using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Controllers;
using DataModels;
using ParserDesktopApp.Raw;

namespace ParserDesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RegexController controller;

        public RegexController Controller
        {
            get => controller;
            set => controller = value;
        }


        public MainWindow()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            this.controller = new RegexController(ShowMessagesFromController);
            this.DataContext = controller;
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            controller.RegexDataCollection.Add(new RegexData());
            Controller.NotifyCounts();
        }

        private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
                Controller.RegexDataCollection.Remove(button.DataContext as RegexData);
            Controller.NotifyCounts();
        }

        private void BtnImportFromPath_Click(object sender, RoutedEventArgs e)
        {
            Controller.LoadDataFromPath(AddDataInCol);
        }

        private void AddDataInCol(RegexData data)
        {
            Dispatcher.Invoke(() =>
            {
                Controller.RegexDataCollection.Add(data);
            });
        }


        private void BtnParse_OnClick(object sender, RoutedEventArgs e)
        {
            Controller.ParseData();
        }

        private void ShowMessagesFromController(Exception ex)
        {
            MessageBox.Show(ex.ToString() + "\n" + ex.StackTrace);
        }

        private void cmbFoundNotFound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller != null)
            {
                string content = (cmbFoundNotFound.SelectedItem as ComboBoxItem).Content.ToString();
                Controller.ApplyFilter(content);
            }
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            (button.DataContext as RegexData).CustomPatteren = controller.Patteren;
            RawContainer rc = new RawContainer(UserControls.UserControlEnums.UserControlNameEnum.ParsedItemDetail, button.DataContext as IEntity);
            rc.Width = 600;
            rc.Height = 400;
            rc.Title = button.Content.ToString();
            rc.ShowDialog();
        }

        private void tbxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            controller.FilterText = (sender as TextBox)?.Text;
            controller.OnPropertyChanged(nameof(controller.RegexDataCollection));
        }

        private void btnCopyFilteredIds_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(string.Join("\n", controller.RegexDataCollection.Select(p => p.DocumentId)));
        }
    }
}
