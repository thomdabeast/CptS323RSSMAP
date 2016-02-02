using System;
using System.Collections.Generic;
using System.ServiceModel.Security;

namespace RSSEngine
{
    /// <summary>
    /// A channel of feeds
    /// </summary>
    /// <remarks>
    /// note on channel, when declaring a channel in main, you cannot just
    /// declare new Channel(); you must add a name on declaration like
    /// new Channel("myChannel");
    /// </remarks>

    public class Channel
    {
        #region Attributes

        private string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Feeds contained in this channel
        /// </summary>
        public List<Feed> Feeds { get; set; } = new List<Feed>();

        /// <summary>
        /// The channels present in this channel
        /// </summary>
        public List<Channel> Channels { get; set; } = new List<Channel>();

        /// <summary>
        /// The name of the channel
        /// </summary>
        /// <remarks>
        /// Requires new because it hides an existing member
        /// </remarks>
        public string ChannelName
        {
            get
            {
                return _name ?? "NULL";
            }
            set
            {
                if (value != null)
                {
                    _name = value;
                }
            }
        }

        #endregion

        #region CTOR

        /// <summary>
        /// ctor for a chaneel
        /// </summary>
        /// <param name="newChannel"></param>
        public Channel(string newChannel)
        {
            ChannelName = newChannel;
        }

        public Channel()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// cant be already in the channel, have a null name, or have a bad url
        /// </summary>
        /// <param name="feed"></param>
        public void AddFeed(Feed feed)
        {
            if (!Feeds.Contains(feed) && feed!= null)
            {
                Feeds.Add(feed);
            }
        }

        /// <summary>
        /// Removes a feed
        /// </summary>
        /// <param name="feed"></param>
        public void RemoveFeed(Feed feed)
        {
            if ((_name != null) && Feeds.Contains(feed))
            {
                Feeds.Remove(feed);
            }
        }

        /// <summary>
        /// Adds a channel to the channel list
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(Channel channel)
        {
            if (!Channels.Contains(channel) && channel != null)
            {
                Channels.Add(channel);
            }
        }

        /// <summary>
        /// Removes a channel to the channel list
        /// </summary>
        /// <param name="channel"></param>
        public void RemoveChannel(Channel channel)
        {
            if (channel != null && Channels.Contains(channel))
            {
                Channels.Remove(channel);
            }
        }

        #endregion
    }
}
