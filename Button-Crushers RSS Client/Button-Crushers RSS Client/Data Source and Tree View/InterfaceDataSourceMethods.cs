using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using RSSEngine;
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;
using System.Speech.Recognition;

namespace Button_Crushers_RSS_Client
{
    /// <summary>
    /// This partial class will contain the methods needed to interact with the tree view
    /// </summary>
    public static partial class InterfaceDataSource
    {
        #region Constants

        private const string AddFeedGestureText = @"Add Feed";
        private const string AddChannelGestureText = @"Add Channel";
        private const string RemoveGestureText = @"Remove item";
        private const string RenameGesureText = @"Rename";

        #endregion

        #region SubscriptionMethods

        /// <summary>
        /// Entry point for search
        /// </summary>
        /// <param name="topicsToSearch"></param>
        /// <returns></returns>
        public static List<Article> TopicSearch(List<string> topicsToSearch)
        {
            var articlesToSearch = new List<Article>();

            _subscriptions.ItemList.ForEach(x => articlesToSearch.AddRange(x.Articles));

            return articlesToSearch.Where(x => SearchArticle(x, topicsToSearch)).ToList();
        }

        /// <summary>
        /// Searchs an article to determine if a keyword exists in it
        /// </summary>
        /// <param name="article"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private static bool SearchArticle(Article article, List<string> keywords)
        {
            var found = false;
            var culture = new CultureInfo("en-US");

            keywords.ForEach(x =>
            {
                if (culture.CompareInfo.IndexOf(article.Title, x, CompareOptions.IgnoreCase) > 0 ||
                    culture.CompareInfo.IndexOf(article.Summary, x, CompareOptions.IgnoreCase) > 0)
                {
                    found = true;
                }
            });

            return found;
        }

        /// <summary>
        /// Determines if we should return filtered or normal
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        public static List<Article> GetFilteredArticles(Feed feed)
        {
            if (feed == null)
            {
                return null;
            }

            var articles =
                    Filter
                    ? (from article in feed.Articles.ToList()
                       where FilterArticlesByDate(article)
                       select article).ToList()
                    : feed.Articles.ToList();

            return UnreadOnly
                ? (from article in articles
                  where article.HasBeenRead == "unread"
                  select article).ToList()
                : articles;
        }

        /// <summary>
        /// predicate esque function that determines what to return
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        private static bool FilterArticlesByDate(Article article)
        {
            DateTime articleDate;
            double daysOut, hoursOut;

            DateTime.TryParse(article.Date, out articleDate);
            double.TryParse(Days, out daysOut);
            double.TryParse(Hours, out hoursOut);

            DateTime dt = DateTime.Now.AddDays(-daysOut).AddHours(-hoursOut);

            return dt < articleDate;
        }

        private static void InitializeFavorites()
        {
            _favoriteTreeViewItem = MakeTreeViewItem(_subscriptions.Favorites.Name);
            TreeView.Items.Add(_favoriteTreeViewItem);
        }

        /// <summary>
        /// Invokes DataProperty changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateArticles(object sender, EventArgs e)
        {
            DataPropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Subscriptions"));
        }

        /// <summary>
        /// saves to a file based on a user name
        /// </summary>
        /// <param name="userName"></param>
        public static void Save(string userName)
        {
            _subscriptions.SaveToFile(userName);
            TopicTreeView.SaveTree(userName);
        }


        /// <summary>
        /// Loads a file based on a passed in user name
        /// </summary>
        /// <param name="userName"></param>
        public static void Load(string userName)
        {
            TopicTreeView.LoadTree(userName);
            _subscriptions.LoadFromFile(userName);
            SetNames();
            _subscriptions.UpdateFeeds();
        }

        public static void SetNames()
        {
            TreeView = new TreeView();
            InitializeFavorites();
            var feedHS = new HashSet<string>();

            foreach (var channel in _subscriptions.ChannelList)
            {
                var channelToAdd = MakeTreeViewItem(channel.ChannelName);
                channelToAdd.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
                channelToAdd.ContextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItem));
                channelToAdd.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                channelToAdd.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));

                TreeView.Items.Add(channelToAdd);

                foreach (var chan in channel.Channels)
                {
                    AddChannelRecursively(ref feedHS, ref channelToAdd, chan);
                }

                foreach (var feed in channel.Feeds)
                {
                    feedHS.Add(feed.Name);
                    var feedViewItem = MakeTreeViewItem(feed.Name);

                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));
                    // Add to UI
                    channelToAdd.Items.Add(feedViewItem);
                }
            }

            //add all feeds not in a channel, just must be in the root level
            foreach (var feed  in _subscriptions.ItemList)
            {
                if (!feedHS.Contains(feed.Name))
                { 
                    var feedViewItem = MakeTreeViewItem(feed.Name);
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));
                    // Add to UI
                    TreeView.Items.Add(feedViewItem);
                }                
            }
        }

        private static void AddChannelRecursively(ref HashSet<string> feedsEncountered, ref TreeViewItem parent, Channel channel)
        {            
            foreach (var chan in channel.Channels)
            {
                var chanViewItem = MakeTreeViewItem(chan.ChannelName);
                AddChannelRecursively(ref feedsEncountered, ref chanViewItem, chan);
            }

            foreach (var feed in channel.Feeds)
            {
                if (!feedsEncountered.Contains(feed.Name))
                {
                    feedsEncountered.Add(feed.Name);
                    var feedViewItem = MakeTreeViewItem(feed.Name);
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
                    feedViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RenameGesureText, RenameMenuOption));
                    // Add to UI
                    parent.Items.Add(feedViewItem);
                }
            }
        }

        public static void AddCheckboxesToTree()
        {
            AddCheckboxes(TreeView.Items);
        }

        private static void AddCheckboxes(IEnumerable items)
        {
            if (items != null)
            {
                foreach (TreeViewItem tvi in items)
                {
                    if (tvi.Items.Count > 0)
                    {
                        AddCheckboxes(tvi.Items);
                    }
                    CheckBox cb = new CheckBox();
                    cb.Content = tvi.Header.ToString();
                    cb.Checked += Cb_Checked;
                    tvi.Header = cb;
                }
            }
        }

        public static void PushPin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Article a = (Article)(sender as Pushpin)?.Tag;
            a.HasBeenRead = "read";

            PushpinDoubleClick?.Invoke(sender, e);
        }

        public static void Cb_Checked(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)(sender as CheckBox)?.Parent;
            if (tvi.Items.Count > 0)
            { // update children if needed
                updateCheckboxes(tvi.Items, (sender as CheckBox).IsChecked);
            }
            CheckBoxClicked?.Invoke(sender, new PropertyChangedEventArgs("click"));
        }

        private static void updateCheckboxes(IEnumerable cbs, bool? check)
        {
            foreach (TreeViewItem child in cbs)
            {
                if (child.Items.Count > 0)
                {
                    updateCheckboxes(child.Items, check);
                }
                CheckBox cb = (CheckBox)child.Header;
                cb.IsChecked = check;
            }
        }

        public static void RemoveCheckboxesfromTree()
        {
            RemoveCheckboxes(TreeView.Items);
        }

        private static void RemoveCheckboxes(IEnumerable items)
        {
            if (items != null)
            {
                foreach (TreeViewItem tvi in items)
                {
                    if (tvi.Items.Count > 0)
                    {
                        RemoveCheckboxes(tvi.Items);
                    }
                    try
                    {
                        CheckBox cb = (CheckBox)tvi.Header;
                        cb.Click -= Cb_Checked;
                        tvi.Header = ((CheckBox)tvi.Header)?.Content?.ToString();

                    }
                    catch (Exception) {}
                }
            }
        }

        /// <summary>
        /// Adds a feed
        /// </summary>
        /// <param name="feed"></param>
        public static void AddFeed(Feed feed)
        {
            _subscriptions.AddFeed(feed);
        }

        /// <summary>
        /// Adds a channel
        /// </summary>
        /// <param name="channel"></param>
        public static void AddChannelToSubscriptions(Channel channel)
        {
            _subscriptions.AddChannel(channel);
        }

        /// <summary>
        /// Searches for a field
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static Feed Search(string queryString)
        {
            return _subscriptions.Search(queryString);
        }

        /// <summary>
        /// Searches for a channel
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static Channel SearchForChannel(string queryString)
        {
            return _subscriptions.SearchForChannel(queryString);
        }

        

        #endregion
    }
}
