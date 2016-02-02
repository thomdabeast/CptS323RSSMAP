using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using MessageBox = System.Windows.MessageBox;

namespace RSSEngine
{
    /// <summary>
    /// Delegate for a feed changed event
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public delegate void FeedChanged(object o, EventArgs e);

    /// <summary>
    /// Subscriptions
    /// </summary>
    /// <remarks>
    /// manages all subscriptions and channels
    /// has a list of every feed even if the feed is in a channel
    /// </remarks>
    public class Subscriptions
    {
        public event FeedChanged FeedPropertyChanged;

        #region Attributes

        private const string FileName = "UserSettings-config-";
        public List<Feed> ItemList = new List<Feed>();
        public List<Channel> ChannelList= new List<Channel>();
        public Favorites Favorites = new Favorites();

        private double _refreshRate = 30;

        #endregion

        #region Properties

        public double RefreshRate
        {
            get { return _refreshRate; }
            set
            {
                //negative refresh rate makes no sense
                if (value > 0)
                {
                    _refreshRate = value;
                    Debug.Assert(_refreshRate > 0);
                }
            }
        }

        #endregion

        #region Methods

        public void SetRefreshRate(double newRate)
        {
            RefreshRate = newRate;
            //only allow a global refresh rate, so the first is as good as any
            //if it is different, update
            if (ItemList.Count > 0 && newRate != ItemList.First()?.RefreshRate)
            {
                ItemList.ForEach(feed => feed.RefreshRate = RefreshRate);
            }
        }

        /// <summary>
        /// Save the item list to a file
        /// </summary>
        /// <param name="userName"></param>
        public void SaveToFile(string userName)
        {
            //all io should be in a try catch
            try
            {
                using (TextWriter writer = new StreamWriter((FileName + userName)))
                {
                    var xmlSer = new XmlSerializer(typeof(Subscriptions));
                    xmlSer.Serialize(writer, this);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// load the item list from a file
        /// </summary>
        /// <param name="userName"></param>
        public void LoadFromFile(string userName)
        {
            //clear the old list
            ItemList = new List<Feed>();
            ChannelList = new List<Channel>();
            Favorites = new Favorites();
            try
            {
                var serializer = new XmlSerializer(typeof(Subscriptions));
                using (Stream file = File.Open(FileName + userName, FileMode.Open))
                {
                    var reader = XmlReader.Create(file);
                    var subs = (Subscriptions)serializer.Deserialize(reader);
                    ItemList = subs.ItemList;
                    ChannelList = subs.ChannelList;
                    Favorites = subs.Favorites;
                    RefreshRate = subs.RefreshRate;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void UpdateFeeds()
        {
            ItemList.ForEach(feed => feed.Update());
        }

        public Feed Search(string name)
        {
            return ItemList.FirstOrDefault(feed => feed.Name == name);
        }

        public Channel SearchForChannel(string name)
        {
            return ChannelList.FirstOrDefault(channel => channel.ChannelName == name);
        }

        /// <param name="feed"></param>
        public void AddFeed(Feed feed)
        {
            if (!ItemList.Contains(feed))
            {
                feed.RefreshRate = RefreshRate;
                ItemList.Add(feed);
                feed.PropertyChanged += OnFeedChanged;
            }
        }


        public void OnFeedChanged(object sender, EventArgs e)
        {
            var feed = sender as Feed;

            var handler = FeedPropertyChanged;
            handler?.Invoke(feed, new EventArgs());
        }

        /// <summary>
        /// removes a feed
        /// </summary>
        /// <param name="feed"></param>
        public void RemoveFeed(Feed feed)
        {
            if (ItemList.Contains(feed))
            {
                ItemList.Remove(feed);
                feed.PropertyChanged -= OnFeedChanged;
            }
        }

        /// <summary>
        /// we will allow channels with bad names, just can't be accessed until named correctly
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(Channel channel)
        {
            if (channel != null && ! ChannelList.Contains(channel))
            {
                ChannelList.Add(channel);
            }
        }

        /// <summary>
        ///has option to save all the feeds in that channel and add them to feeds list
        ///or just delete the channel and all of its contents
        /// </summary>
        /// <param name="channel"></param>
        public bool RemoveChannel(Channel channel)
        {
            const string messageBoxPrompt = "This will delete all feeds and channels in this channel. Continue?";
            const string messageBoxCaption = "Remove Channel";
            bool result = false;

            if (ChannelList.Contains(channel))
            {
                if (channel.Feeds.Count > 0 || channel.Channels.Count > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show(messageBoxPrompt, messageBoxCaption,
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        ChannelList.Remove(channel);
                        result = true;
                    }

                }
                else
                {
                    ChannelList.Remove(channel);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a given feed from a given channel
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="channel"></param>
        public void RemoveFeedfromChannel(Feed feed, Channel channel)
        {
            channel.Feeds.Remove(feed);
        }


        /// <summary>
        /// Adds a given feed to a given channel
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="channel"></param>
        public void AddFeedtoChannel(Feed feed, Channel channel)
        {
            channel.Feeds.Add(feed);
        }


        /// <summary>
        /// Moves a feed to a channel
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="channel"></param>
        public void MoveToChannel(Feed feed, Channel channel)
        {
            channel.Feeds.Add(feed);
        }

        /// <summary>
        /// Move a feed from a channel
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="channel"></param>
        /// <remarks>
        /// If a feed exists in a channel, remove it from the channel and return the same feed, otherwise return null
        /// </remarks>
        public Feed MoveFromChannel(Feed feed, Channel channel)
        {
            Feed feedToRemove = null;

            if (channel.Feeds.Contains(feed))
            {
                channel.Feeds.Remove(feed);
                feedToRemove = feed;
            }

            return feedToRemove;
        }

        #endregion
    }
}