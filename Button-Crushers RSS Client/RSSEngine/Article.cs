using System.ComponentModel;

namespace RSSEngine
{
    public class Article : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Attributes

        private string _read;

        #endregion

        #region Properties
        public string HasBeenRead
        {
            get { return _read; }
            set
            {
                _read = value;
                NotifyPropertyChanged("HasBeenRead");
            }
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Date { get; set; }

        public string Link { get; set; }

        #endregion

        #region CTOR

        /// <summary>
        /// Make a new article
        /// </summary>
        /// <param name="title"></param>
        /// <param name="summary"></param>
        /// <param name="date"></param>
        /// <param name="link"></param>
        public Article(string title, string summary, string date, string link)
        {
            Title = title;
            Summary = summary;
            Date = date;
            Link = link;
            HasBeenRead = "unread";
        }

        public Article()
        { }

        #endregion

        #region Methods

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
