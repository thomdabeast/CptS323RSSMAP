using System.Windows;
using RSSEngine;

namespace Button_Crushers_RSS_Client
{
  /// <summary>
  ///   Interaction logic for MainInterfaceControl.xaml
  /// </summary>
    public partial class MainInterfaceControl
    {
        public MainInterfaceControl()
        {
          InitializeComponent();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Window settings = new SettingsWindow();
        }

        public void LoadArticle(Article article)
        {
            MainInterfaceWebBrowser.Navigate(article.Link);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadArticle(new Article("a", "hilary", "1111", "http://www.cnn.com/2015/10/07/asia/doctors-without-borders-afghanistan-airstrike/index.html?eref=rss_topstories"));
        }
    }
}