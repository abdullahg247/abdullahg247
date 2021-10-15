using Controllers;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ParsedItemDetail : UserControl
    {

        private RegexItemDetailController m_controller;


        public ParsedItemDetail(IEntity entity)
        {
            InitializeComponent();
            InitializeData(entity);
        }

        private void InitializeData(IEntity entity)
        {
            m_controller = new RegexItemDetailController(entity as RegexData);
            this.DataContext = m_controller;
            BuildResultTree();
        }

        private void BuildResultTree()
        {
            if (m_controller?.RegexData?.Matches?.Count > 0)
            {
                TvMatches.Items.Clear();
                foreach (Match match in m_controller.RegexData.Matches)
                {
                    TreeViewItem tvItem = new TreeViewItem();
                    tvItem.DataContext = match;
                    tvItem.Header = match.Value;
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        TreeViewItem tvGroupItem = new TreeViewItem();
                        tvGroupItem.Header = "[" + m_controller.RegexData.Regex.GroupNameFromNumber(i) + "] " + match.Groups[i].Value;
                        tvGroupItem.DataContext = match.Groups[i];
                        tvItem.Items.Add(tvGroupItem);
           //             tvGroupItem.Selected += TvItem_Selected;
                    }
                    TvMatches.Items.Add(tvItem);
                }
            }
        }
        //private void TvItem_Selected(object sender, RoutedEventArgs e)
        //{
        //    TreeViewItem tv = TvMatches.SelectedItem as TreeViewItem;
        //    if (tv?.DataContext is Group)
        //    {
        //        var match = tv.DataContext as Group;
        //        RtbText.Focus();
        //        RtbText.Select(match.Index, match.Length);
        //        e.Handled = true;
        //        return;
        //    }
        //}

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    foreach (TreeViewItem item in TvMatches.Items)
        //    {
        //        if (item?.Items?.Count > 0)
        //        {
        //            foreach (TreeViewItem detlItem in item.Items)
        //            {
        //                detlItem.Selected -= TvItem_Selected;
        //            }
        //        }
        //    }
        //}

        private void TvMatches_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ClearEvents();
        }

        private void BtnTestRegex_Click(object sender, RoutedEventArgs e)
        {
            m_controller.ParseData();
            ClearEvents();
            BuildResultTree();
        }

        private void ClearEvents()
        {
            TreeViewItem tv = TvMatches.SelectedItem as TreeViewItem;
            if (tv?.DataContext is Match)
            {
                var match = tv.DataContext as Match;
                RtbText.Focus();
                RtbText.Select(match.Index, match.Length);
            }
        }
    }
}
