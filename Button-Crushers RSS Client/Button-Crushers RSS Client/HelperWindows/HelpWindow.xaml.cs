using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
