using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow
    {
        public string newName;

        public RenameWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RenameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NameTextBox.Text))
            {
                newName = TreeViewValidator.ValidateStringForTreeViewItem(NameTextBox.Text);
                DialogResult = true;
            }
            
            Close();
        }
    }
}
