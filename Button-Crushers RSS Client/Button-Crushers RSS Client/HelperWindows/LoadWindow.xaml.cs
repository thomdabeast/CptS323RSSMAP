using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    /// <summary>
    /// Interaction logic for LoadWindow.xaml
    /// </summary>
    public partial class LoadWindow
    {
        public LoadWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO modify load method to return a bool
            InterfaceDataSource.Load(NameTextBox.Text);
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
