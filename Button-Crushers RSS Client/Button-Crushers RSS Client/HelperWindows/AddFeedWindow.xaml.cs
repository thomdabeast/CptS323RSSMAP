using System;
using System.Windows;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    /// <summary>
    /// Interaction logic for AddFeedWindow.xaml
    /// </summary>
    public partial class AddFeedWindow
    {
        /// <summary>
        /// The feed name and url shall be retrieved through this
        /// </summary>
        public Tuple<string, string> Feed { get; private set; }

        /// <summary>
        /// ctor for adding a Feed Window
        /// </summary>
        public AddFeedWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets result to true, returns the items, and close the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Feed = new Tuple<string, string>(TreeViewValidator.ValidateStringForTreeViewItem(NameTextBox.Text),
                                             TreeViewValidator.ValidateStringForTreeViewItem(urlTextBox.Text));
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// set result to false and close dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Set the name text box to the active (focused) control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }
    }
}
