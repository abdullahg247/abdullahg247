using DataModels;
using System.Windows;
using UserControls;
using UserControls.UserControlEnums;

namespace ParserDesktopApp.Raw
{
    /// <summary>
    /// Interaction logic for RawContainer.xaml
    /// </summary>
    public partial class RawContainer : Window
    {
        public RawContainer(UserControlNameEnum _enum, IEntity entity)
        {
            InitializeComponent();
            InitializeContentControlAndDataContext(_enum, entity);
        }

        private void InitializeContentControlAndDataContext(UserControlNameEnum @enum, IEntity entity)
        {
            if(@enum == UserControlNameEnum.ParsedItemDetail)
            {
                contentControl.Content = new ParsedItemDetail(entity);
            }
        }
    }
}
