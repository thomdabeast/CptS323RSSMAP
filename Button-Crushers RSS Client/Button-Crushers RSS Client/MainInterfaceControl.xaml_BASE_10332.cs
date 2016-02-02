using System.Windows;

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
  }
}