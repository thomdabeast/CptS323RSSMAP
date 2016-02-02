using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Button_Crushers_RSS_Client
{
  /// <summary>
  /// Interaction logic for MapInterfaceControl.xaml
  /// </summary>
  public partial class MapInterfaceControl : UserControl
  {
    public MapInterfaceControl()
    {
      InitializeComponent();
    }

    private void myMap_Loaded(object sender, RoutedEventArgs e)
    {
      // Attach image url to Image tag
      settings.Source = new BitmapImage(new Uri("settings.png", UriKind.Relative));
      MakeContextMenu();

      // treeview stuff
      TreeViewItem treeItem = new TreeViewItem();

      treeItem.Header = new CheckBox() { Content = "My Collection" };
      ((CheckBox)treeItem.Header).Checked += MapInterfaceControl_Checked;
      treeItem.Items.Add(new TreeViewItem() { Header = new CheckBox() { Content = "CNN" } });
      treeItem.Items.Add(new TreeViewItem() { Header = new CheckBox() { Content = "MSNBC" } });
      treeItem.Items.Add(new TreeViewItem() { Header = new CheckBox() { Content = "Seahawks" } });

      treeView.Items.Add(treeItem);
    }

    private void MakeContextMenu()
    {
      ContextMenu mnu = new ContextMenu();
      MenuItem mnuAddCollection = new MenuItem();
      mnuAddCollection.Header = "Add Collection";
      mnuAddCollection.Click += MnuAddCollection_Click;
      MenuItem mnuAddFeed = new MenuItem();
      mnuAddFeed.Header = "Add Feed";
      mnuAddFeed.Click += MnuAddFeed_Click;
      MenuItem mnuDelete = new MenuItem();
      mnuDelete.Header = "Delete";
      mnuDelete.Click += MnuDelete_Click;

      mnu.Items.Add(mnuAddCollection);
      mnu.Items.Add(mnuAddFeed);
      mnu.Items.Add(mnuDelete);
      treeView.ContextMenu = mnu;
    }

    private void MnuDelete_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MnuAddFeed_Click(object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void MnuAddCollection_Click(object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void MapInterfaceControl_Checked(object sender, RoutedEventArgs e)
    {
      // get the treeViewItem containing the checkBox
      CheckBox cb = e.Source as CheckBox;
      TreeViewItem tvi = (TreeViewItem)cb?.Parent;

      if (cb.IsChecked == true)
      {
        // expand treeviewitem
        tvi.IsExpanded = true;

        // Go through each sub treeViewItem
        foreach (TreeViewItem item in tvi.Items)
        {
          CheckBox chkr = (CheckBox)item.Header;
          if (chkr?.IsChecked != null && !(bool)chkr.IsChecked)
          {
            chkr.IsChecked = true;
          }
        }
      }
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
      // Search for URL
      if (e.Key == Key.Enter)
      {
      }
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      searchBox.Text = string.Empty;
    }

    private void searchBox_LostFocus(object sender, RoutedEventArgs e)
    {
      searchBox.Text = "Search";
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("Look for " + searchBox.Text + " feed.");
    }

    private void settings_click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("Settings UI");
    }

    private void Canvas_MouseEnter(object sender, MouseEventArgs e)
    {

    }

    private void Canvas_MouseLeave(object sender, MouseEventArgs e)
    {

    }

    private void Canvas_MouseEnter_1(object sender, MouseEventArgs e)
    {
      Canvas c = e.Source as Canvas;
      var textBlock = (TextBlock)c.Children[1];
      if (textBlock != null)
      {
        textBlock.Visibility = Visibility.Visible;
      }
      var block = (TextBlock)c.Children[2];
      if (block != null)
      {
        block.Visibility = Visibility.Visible;
      }
    }

    private void Canvas_MouseLeave_1(object sender, MouseEventArgs e)
    {
      Canvas c = e.Source as Canvas;
      var textBlock = (TextBlock)c.Children[1];
      if (textBlock != null)
      {
        textBlock.Visibility = Visibility.Hidden;
      }
      var block = (TextBlock)c.Children[2];
      if (block != null)
      {
        block.Visibility = Visibility.Hidden;
      }
    }
  }
}
