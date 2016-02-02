using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RSSEngine;

namespace Button_Crushers_RSS_Client
{
    public static partial class InterfaceDataSource
    {
        #region TreeMethods

        #region Event Handler

        /// <summary>
        /// Force the item under the right click to be focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ContentTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //find the original object that raised the event
            if (e.OriginalSource != null)
            {
                var clickedItem = VisualTreeHelper.GetParent(e.OriginalSource as UIElement) as UIElement;

                //find the clicked TreeViewItem
                while ((clickedItem != null) && !(clickedItem is TreeViewItem))
                {
                    clickedItem = VisualTreeHelper.GetParent(clickedItem) as UIElement;
                }

                var clickedTreeViewItem = clickedItem as TreeViewItem;

                if (clickedTreeViewItem != null)
                {
                    clickedTreeViewItem.IsSelected = true;
                    clickedTreeViewItem.Focus();
                }
            }
        }

        #endregion

        /// <summary>
        /// Make a contextMenu item
        /// </summary>
        /// <param name="gestureText"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public static MenuItem MakeContextMenuItem(string gestureText, RoutedEventHandler eventHandler)
        {
            MenuItem addItem = new MenuItem { InputGestureText = gestureText };
            addItem.Click += eventHandler;
            return addItem;
        }

        public static TreeViewItem MakeTreeViewItem(string name)
        {
            return Builder.Build(name);
        }

        /// <summary>
        /// Adds a channel to the visual tree
        /// </summary>
        /// <remarks>
        /// Mainly for the facade
        /// </remarks>
        public static Tuple<TreeViewItem,Channel> AddChannelToTree()
        {
            var createChannelWindow = new HelperWindows.NameChannelWindow();
            createChannelWindow.ShowDialog();
            Tuple<TreeViewItem, Channel> result = null;

            if (createChannelWindow.DialogResult == true)
            {
                // Create feed and add it
                var channel = new Channel(createChannelWindow.Channel);

                var channelViewItem = MakeTreeViewItem(channel.ChannelName);

                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItem));
                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));

                TreeView.Items.Add(channelViewItem);
                AddChannelToSubscriptions(channel);
                result = new Tuple<TreeViewItem, Channel>(channelViewItem, channel);
            }

            return result;
        }

        /// <summary>
        /// Add a feed to the tree and subscription object
        /// </summary>
        /// <remarks>
        /// Mainly used in facade
        /// </remarks>
        public static void AddFeedToTree(Tuple<TreeViewItem, Channel> parent)
        {
            var createFeedWindow = new HelperWindows.AddFeedWindow();
            createFeedWindow.ShowDialog();

            if (createFeedWindow.DialogResult == true)
            {
                // Create feed and add it
                var feed = new Feed(createFeedWindow.Feed.Item1, createFeedWindow.Feed.Item2);

                var feedViewItem = MakeTreeViewItem(createFeedWindow.Feed.Item1);

                feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));

                if (parent != null)
                {
                    parent.Item2?.AddFeed(feed);
                    parent.Item1.Items.Add(feedViewItem);
                }
                else
                {
                    TreeView.Items.Add(feedViewItem);
                }
            
                _subscriptions.AddFeed(feed);
            }
        }

        /// <summary>
        /// tester for Facade
        /// </summary>
        public static void AddFeedToTree()
        {
            var createFeedWindow = new HelperWindows.AddFeedWindow();
            createFeedWindow.ShowDialog();

            if (createFeedWindow.DialogResult == true)
            {
                // Create feed and add it
                var feed = new Feed(createFeedWindow.Feed.Item1, createFeedWindow.Feed.Item2);

                var feedViewItem = MakeTreeViewItem(createFeedWindow.Feed.Item1);

                feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));

                if (SelectedTreeViewItem != null)
                {
                    var channel = _subscriptions.SearchForChannel(((TreeViewItem)SelectedTreeViewItem)?.Header.ToString());
                    if (channel != null)
                    {
                        channel.AddFeed(feed);
                        (SelectedTreeViewItem as TreeViewItem).Items.Add(feedViewItem);
                    }
                }
                else
                {
                    TreeView.Items.Add(feedViewItem);
                }
                

                _subscriptions.AddFeed(feed);
            }
        }


        /// <summary>
        /// This is the method to be used by other tree view items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddChannelItem(object sender, RoutedEventArgs e)
        {
            // Get input
            HelperWindows.NameChannelWindow newChannel = new HelperWindows.NameChannelWindow();
            newChannel.ShowDialog();

            TreeViewItem parent = TreeView.SelectedItem as TreeViewItem;

            if (parent != null)
            {
                if (TreeView.SelectedItem != null && true == newChannel.DialogResult)
                {
                    // create channel and add
                    Channel channel = new Channel(newChannel.Channel);

                    TreeViewItem channelViewItem = MakeTreeViewItem(channel.ChannelName);

                    channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
                    channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItem));
                    channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                    try {
                        parent.Items.Add(channelViewItem);
                    }
                    catch (InvalidOperationException)
                    { // Handle exception by just adding channel to root node
                        TreeView.Items.Add(channelViewItem);
                    }
                    var channelParent = _subscriptions.SearchForChannel(parent.Header.ToString());

                    channelParent?.AddChannel(channel);
                }
            }
        }

        /// <summary>
        /// Adds a feed to the visual tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddFeedItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = TreeView.SelectedItem as TreeViewItem;

            if (parent != null)
            {
                HelperWindows.AddFeedWindow addFeedWindow = new HelperWindows.AddFeedWindow();
                addFeedWindow.ShowDialog();

                if (true == addFeedWindow.DialogResult)
                {
                    Feed newFeed = new Feed(addFeedWindow.Feed.Item1, addFeedWindow.Feed.Item2);
                    AddFeed(newFeed);

                    TreeViewItem feedViewItem = MakeTreeViewItem(newFeed.Name);
                    
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                    try
                    {
                        parent.Items.Add(feedViewItem);
                    }
                    catch (InvalidOperationException)
                    {
                        ((TreeViewItem)parent.Parent)?.Items.Add(feedViewItem);
                    }

                    var subscriptionParent = _subscriptions.SearchForChannel(parent.Header.ToString());

                    subscriptionParent?.AddFeed(newFeed);
                }
            }
        }

        /// <summary>
        /// Method to remove an item from the visual tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RemoveMenuOption(object sender, RoutedEventArgs e)
        {
            var item = TreeView.SelectedItem as TreeViewItem;

            if (item != null)
            {
                var headerText = item.Header.ToString();

                var toRemove = Search(headerText);

                if (toRemove != null)
                {
                    _subscriptions.RemoveFeed(toRemove);
                    //check if the parent was a treeview Item by attempting a cast
                    var parent = item.Parent as TreeViewItem;

                    if (parent == null)
                    {       
                        TreeView.Items.Remove(item);
                    }
                    else
                    {
                        parent.Items.Remove(item);
                    }                                                      
                }
                else
                {
                    var channelToRemove = SearchForChannel(headerText);

                    if (channelToRemove != null)
                    {
                        if (_subscriptions.RemoveChannel(channelToRemove))
                        {
                            TreeView.Items.Remove(item);
                        }
                    }
                }   
            }
        }

        /// <summary>
        /// Provides an ability to rename a tree view item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RenameMenuOption(object sender, RoutedEventArgs e)
        {
            var parent = TreeView.SelectedItem as TreeViewItem;

            if (parent != null)
            {
                var renameWindow = new HelperWindows.RenameWindow();
                renameWindow.ShowDialog();

                if (renameWindow.DialogResult == true)
                {
                    var treeItemToRename =_subscriptions.Search(parent.Header.ToString());

                    if (treeItemToRename != null)
                    {
                        treeItemToRename.Name = renameWindow.newName;
                        parent.Header = renameWindow.newName;
                    } 
                }
            }
        }

        private static ContextMenu MakeTreeViewContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItemToParent));
            contextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItemToParent));
            return contextMenu;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddFeedItemToParent(object sender, RoutedEventArgs e)
        {
            // Get user input
            var addFeedWindow = new HelperWindows.AddFeedWindow();
            addFeedWindow.ShowDialog();

            if (addFeedWindow.DialogResult == true)
            {
                // Create feed and and add it to the engine
                Feed newFeed = new Feed(addFeedWindow.Feed.Item1, addFeedWindow.Feed.Item2);
                _subscriptions.AddFeed(newFeed);

                TreeViewItem feedViewItem = MakeTreeViewItem(newFeed.Name);
                
                feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));

                // Add to UI
                TreeView.Items.Add(feedViewItem);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddChannelItemToParent(object sender, RoutedEventArgs e)
        {
            // Get user input
            var addChannelWindow = new HelperWindows.NameChannelWindow();
            addChannelWindow.ShowDialog();

            if (addChannelWindow.DialogResult == true)
            {
                Channel newChannel = new Channel(addChannelWindow.Channel);
                _subscriptions.AddChannel(newChannel);

                TreeViewItem channelViewItem = MakeTreeViewItem(newChannel.ChannelName);

                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                channelViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));

                // Add to UI
                TreeView.Items.Add(channelViewItem);
            }
        }

        public static void EditFavorites(Article myArticle)
        {
            if(_subscriptions.Favorites.Articles.Contains(myArticle))
            {
                _subscriptions.Favorites.Articles.Remove(myArticle);
            }
            else
            {
                _subscriptions.Favorites.Articles.Add(myArticle);
            }
        }

        public static List<Feed> GetFeedsInSubscriptions()
        {
            return _subscriptions.ItemList;
        }
        #endregion
    }
}
