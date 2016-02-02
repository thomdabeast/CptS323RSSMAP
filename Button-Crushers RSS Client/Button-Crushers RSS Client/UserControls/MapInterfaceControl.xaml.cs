using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Button_Crushers_RSS_Client.Map_Utility;
using RSSEngine;
using System.Drawing;
using System.Windows.Media;

namespace Button_Crushers_RSS_Client
{
    /// <summary>
    /// Interaction logic for MapInterfaceControl.xaml
    /// </summary>
    public partial class MapInterfaceControl
    {

        private readonly TaskScheduler _context = TaskScheduler.FromCurrentSynchronizationContext();

        public MapInterfaceControl()
        {
            InitializeComponent();
        }


        private void MapInterfaceControl_Checked(object sender, RoutedEventArgs e)
        {
            // get the treeViewItem containing the checkBox
            CheckBox checkBox = e.Source as CheckBox;
            TreeViewItem treeItem = (TreeViewItem)checkBox?.Parent;

            if (checkBox?.IsChecked != null && (bool)checkBox.IsChecked)
            {
                // expand treeviewitem
                treeItem.IsExpanded = true;

                // Go through each sub treeViewItem
                foreach (TreeViewItem item in treeItem.Items)
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
            SearchBox.Text = string.Empty;
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "Search";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(LoadItemsOnMap, Task.Factory.CancellationToken, TaskCreationOptions.None, _context);
            if (SearchBox.Text != string.Empty)
            {

            }
        }

        private void MapInterfaceControl1_Loaded(object sender, RoutedEventArgs e)
        {
            // Need to add checkboxes
            InterfaceDataSource.AddCheckboxesToTree();
            MapContentTreeView.Content = InterfaceDataSource.TreeView;
            InterfaceDataSource.DataPropertyChanged += UpdateItemSource;
            InterfaceDataSource.SelectedItemChanged += ContentTreeView_SelectedItemChanged;
            InterfaceDataSource.CheckBoxClicked += InterfaceDataSource_CheckBoxClicked;
        }

        private void InterfaceDataSource_CheckBoxClicked(object sender, PropertyChangedEventArgs e)
        {
            var feeds = new List<Feed>();
            CheckBox cb = sender as CheckBox;

            if (cb.IsChecked == true)
            { // add feeds to map
                AddItemsToMap(GetFeeds(cb.Parent));
            }
            else
            { // remove 
                RemoveItemsFromMap(GetFeeds(cb.Parent));
            }
        }

        private List<Feed> GetFeeds(object tvi)
        {
            TreeViewItem t = tvi as TreeViewItem;
            var feeds = new List<Feed>();
            if (t?.Items.Count > 0)
            {
                foreach (var item in t?.Items)
                {
                    feeds.AddRange(GetFeeds(item));
                }
            }
            var feed = InterfaceDataSource.Search((t.Header as CheckBox).Content.ToString());
            if (feed != null)
            {
                feeds.Add(feed);
               
            }
            return feeds;
        }

        private void AddItemsToMap(List<Feed> feeds)
        {
            var articles = new List<Article>();

            foreach (var feed in feeds)
            {
                articles.AddRange(feed?.Articles);
            }

            char[] delim = { ' ' };

            foreach (Article article in articles)
            {
                string str = article.Title + article.Summary;
                var location = CsvAdapter.LookUpCity(str.Split(delim, StringSplitOptions.RemoveEmptyEntries));

                if (location != null)
                {
                    double latitude;
                    double longitude;

                    if (double.TryParse(location.Item2, out latitude) &&
                        double.TryParse(location.Item3, out longitude))
                    {
                        var latAndLong = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);
                        var pushPin = new Pushpin { Location = latAndLong, Content = article.Title, Tag = article };
                        pushPin.ToolTip = pushPin.Content.ToString();

                        // to handle changing main windows control to maininterface
                        pushPin.MouseDoubleClick += InterfaceDataSource.PushPin_MouseDoubleClick;

                        // to remove pushpins from map
                        pushPin.MouseDoubleClick += PushPin_MouseDoubleClick;

                        // Keep reference for easy deletion from map
                        InterfaceDataSource.Pushpins.Add(pushPin);
                        //add a context menu maybe? or a mouse over event?
                        MyMap.Children.Add(pushPin);
                    }
                }
            }
        }

        private void PushPin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in InterfaceDataSource.Pushpins)
            {
                MyMap.Children.Remove(item);
            }
            InterfaceDataSource.Pushpins.Clear();
        }

        private void RemoveItemsFromMap(List<Feed> feeds)
        {
            var articles = new List<Article>();

            foreach (var feed in feeds)
            {
                articles.AddRange(feed.Articles);
            }

            for (int i = 0; i < InterfaceDataSource.Pushpins.Count; i++)
            {
                foreach (Article item in articles)
                {
                    if (item.Title.Equals(InterfaceDataSource.Pushpins[i].Content))
                    {
                        InterfaceDataSource.Pushpins[i].MouseDoubleClick -= InterfaceDataSource.PushPin_MouseDoubleClick;
                        InterfaceDataSource.Pushpins[i].MouseDoubleClick -= PushPin_MouseDoubleClick;
                        MyMap.Children.Remove(InterfaceDataSource.Pushpins[i]);
                        InterfaceDataSource.Pushpins.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// needs to be on a worker thread, it is expensive
        /// </summary>
        /// <remarks>
        /// call like this
        /// Task.Factory.StartNew(LoadItemsOnMap, Task.Factory.CancellationToken, TaskCreationOptions.None, _context);
        /// Keep out of load and unload
        /// </remarks>
        private void LoadItemsOnMap()
        {
            var articles = new List<Article>();

            foreach (var feed in InterfaceDataSource.GetFeedsInSubscriptions())
            {
                articles.AddRange(feed.Articles);
            }

            char[] delim = { ' ' };

            var locations = articles.Select(article => article.Summary + article.Title)
                .Select(x => CsvAdapter.LookUpCity(x.Split(delim, StringSplitOptions.RemoveEmptyEntries)))
                .ToList();
            //first or default will not throw an exception, but it will add nulls, remove those
            locations.RemoveAll(t => t == null);

            foreach (var location in locations)
            {
                double latitude;
                double longitude;

                if (double.TryParse(location.Item2, out latitude) &&
                    double.TryParse(location.Item3, out longitude))
                {
                    var latAndLong = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);
                    var pushPin = new Pushpin { Location = latAndLong };
                    // to handle changing main windows control to maininterface
                    pushPin.MouseDoubleClick += InterfaceDataSource.PushPin_MouseDoubleClick;

                    // to remove pushpins from map
                    pushPin.MouseDoubleClick += PushPin_MouseDoubleClick;

                    //Keep reference so we can delete from map easily
                    InterfaceDataSource.Pushpins.Add(pushPin);
                    //add a context menu maybe? or a mouse over event?
                    MyMap.Children.Add(pushPin);
                }
            }
        }

        private void MapInterfaceControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            //MUST DO THIS, can only have one ref
            InterfaceDataSource.DataPropertyChanged -= UpdateItemSource;
            InterfaceDataSource.SelectedItemChanged -= ContentTreeView_SelectedItemChanged;
            InterfaceDataSource.CheckBoxClicked -= InterfaceDataSource_CheckBoxClicked;
            MapContentTreeView.Content = null;
        }

        private void UpdateItemSource(object sender, PropertyChangedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var feedToUpdate = sender as Feed;
                var currentName = (InterfaceDataSource.SelectedTreeViewItem as TreeViewItem)?.Header.ToString();

                if (currentName == feedToUpdate?.Name)
                {

                }
            }, Task.Factory.CancellationToken, TaskCreationOptions.None, _context);
        }

        private void ContentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = sender as TreeView;

            if (tree != null)
            {

            }
        }


        private void MapContentTreeView_MouseEnter(object sender, MouseEventArgs e)
        { // Disable mouse actions when working in treeview
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick;
            MyMap.MouseDown += MyMap_MouseDown;
        }

        private void MyMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MyMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MapContentTreeView_MouseLeave(object sender, MouseEventArgs e)
        {
            MyMap.MouseDoubleClick -= MyMap_MouseDoubleClick;
        }
    }
}
