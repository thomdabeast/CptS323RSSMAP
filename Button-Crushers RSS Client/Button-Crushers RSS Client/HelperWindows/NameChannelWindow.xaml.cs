using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
  /// <summary>
  /// Interaction logic for NameChannelWindow.xaml
  /// </summary>
  public partial class NameChannelWindow
  {
    /// <summary>
    /// Channel name shall be retrieved through this
    /// </summary>
    public string Channel { get; private set; }

    /// <summary>
    /// Ctor for a NameChannelWindow
    /// </summary>
    public NameChannelWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Event handler for the add button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Add_Click(object sender, RoutedEventArgs e)
    {
      Channel = TreeViewValidator.ValidateStringForTreeViewItem(NameTextBox.Text);
      DialogResult = true;
      Close();
    }

    /// <summary>
    /// Event handler for the cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    /// <summary>
    /// Set the textbox to focused on load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      NameTextBox.Focus();
    }
  }
}
