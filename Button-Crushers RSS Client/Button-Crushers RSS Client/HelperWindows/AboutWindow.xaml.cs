using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow 
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void AboutButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
