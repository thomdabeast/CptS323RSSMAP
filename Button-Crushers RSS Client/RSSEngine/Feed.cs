using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace RSSEngine
{
    public class Favorites : Feed
    {
        protected new ObservableCollection<Article> _articles = new ObservableCollection<Article>();

        public Favorites() : base("Favorites", null)
        {
            _timer.Enabled = false;
        }

        public void addArticle(Article myArticle)
        {
            _articles.Add(myArticle);
            Update();
        }

        public void removeArticle(Article myArticle)
        {
            _articles.Remove(myArticle);
            Update();
        }

        public override void Update()
        {
            OnPropertyChanged("Articles");
        }
    }
    /// <summary>
    /// Represents a feed of articles
    /// Contains the callback to update it in the form
    /// </summary>
    /// <remarks>
    /// Feeds and Channels will be given the name null if they cannot be created with set name.
    /// Rename must be used before you can perform ANY operations on channel or feed besides ones that
    ///Relate to renaming them.Therefore a feed or channel with null name will always be empty.You also cannot rename a
    ///feed or channel to null once it has a name that is not null.
    /// </remarks>
    [Serializable]
    public class Feed : INotifyPropertyChanged
    {
        //event for when a feed changes
        public event PropertyChangedEventHandler PropertyChanged;

        #region Attributes

        //no need to serialize articles, that is old news!
        protected ObservableCollection<Article> _articles = new ObservableCollection<Article>();
        //keeps track of names of all feeds so user cannot name two feeds same name.
        private static List<string> _namesList = new List<string>();

        protected Timer _timer = new Timer();
        public bool IsRead { get; set; }

        //default refresh rate user can define
        private static int _defaultRefreshRate = 30 * 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the state
        /// </summary>
        public bool Valid { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        /// <summary>
        /// Returns the list of srticles
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<Article> Articles => _articles;


        /// <summary>
        /// Feed's refresh rate
        /// </summary>
        public double RefreshRate
        {
            get
            {
                return _timer.Interval / 1000;
            }
            set
            {
                _timer.Interval = value * 1000;
            }
        }

        #endregion

        #region CTOR

        /// <summary>
        /// creates a feed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public Feed(string name, string url)
        {
            Valid = true;
            URL = url;
            Name = name;
            _namesList.Add(name);

            Update();
            CreateTimer();
        }

        public Feed() { }

        #endregion

        #region Methods

        #region Event Handlers

        /// <summary>
        /// Do work on the timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_tick(object sender, EventArgs e)
        {
            Update();
        }

        #endregion

        /// <summary>
        /// creates a timer that performs timer_tick every x seconds
        ///default refresh = 30 seconds
        /// </summary>
        public void CreateTimer()
        {
            _timer.Interval = _defaultRefreshRate;
            _timer.Elapsed += timer_tick;
            _timer.Enabled = true;
            _timer.Start();
        }

        /// <summary>
        /// checks if can change to name given
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NewName(string name)
        {
            return (!_namesList.Contains(name) && (name != null));
        }

        /// <summary>
        /// if feed is not a valid feed, we will not update. Ever.
        ///if feed doesnt have name, will not update. Prompt user for option to name.
        ///CAN HAVE: multiple feeds with the same url if the user wishes. Names must not be the same though.
        /// </summary>
        public virtual void Update()
        {
            //copies over articles before writing over them
            var oldArticles = _articles.ToList();

            _articles = new ObservableCollection<Article>();

            char[] delimiters = { '<' };
            try
            {
                using (var xr = XmlReader.Create(URL))
                {
                    var reader = SyndicationFeed.Load(xr);
                    if (reader?.Items != null)
                    {
                        foreach (var item in reader.Items)
                        {
                            var sum = item.Summary.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                            _articles.Add(new Article(item.Title.Text, sum[0], item.PublishDate.ToString(), item.Id));
                        }
                    }
                }

                //check which articles are new, then keep track of read and unread
                foreach(Article a in _articles)
                {
                    //if the article a has a that exists in the list of oldarticles, and the old article was indeed read
                    if(oldArticles.Exists(x => (x.Title == a.Title) && (x.HasBeenRead == "read")))
                    {
                        //set the new article to read
                        a.HasBeenRead = "read";
                    }
                }

                OnPropertyChanged("Articles");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Valid = false;
            }
        }

        //event handler for the property changed
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
