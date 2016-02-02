using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using RSSEngine;
using System.Collections.Generic;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Input;
using System.Speech.Recognition;

namespace Button_Crushers_RSS_Client
{
    public static partial class InterfaceDataSource
    {
        /// <summary>
        /// Delegate for the content controls to know when to update
        /// </summary>
        public static event PropertyChangedEventHandler DataPropertyChanged = delegate { };

        /// <summary>
        /// Delegate for the map to know when to add/remove pushpins
        /// </summary>
        public static event PropertyChangedEventHandler CheckBoxClicked = delegate { };

        /// <summary>
        /// Delegate Pushpin DoubleClicked
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void PushpinDoubleClickEventHandler(object o, MouseButtonEventArgs e);

        /// <summary>
        /// Main Window will use this to change it's Content view and MainInterface to load the web browser.
        /// </summary>
        public static event PushpinDoubleClickEventHandler PushpinDoubleClick = delegate{};

        /// <summary>
        /// Delegate SelectedItemChanged
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void TreeViewSelectedChanged(object o, RoutedPropertyChangedEventArgs<object> e);

        /// <summary>
        /// This will be what the datagrids subscribes to know when an item was selected
        /// </summary>
        public static event TreeViewSelectedChanged SelectedItemChanged = delegate { };

        #region Attributes

        /// <summary>
        /// the singleton data backing for the treeview
        /// </summary>
        private static Subscriptions _subscriptions = new Subscriptions();

        private static string _refrestRate = "30";

        public static Builder<TreeViewItem> Builder = new PlainBuilder();

        #endregion

        #region Properties

        public static bool Filter { get; set; }

        public static bool UnreadOnly { get; set; }

        public static string Days { get; set; }

        public static string Hours { get; set; }

        public static string RefreshRate
        {
            get { return _refrestRate; }
            set
            {
                double rateOut;
                if (double.TryParse(value, out rateOut))
                {
                    _subscriptions.SetRefreshRate(rateOut);
                }
                _refrestRate = value;
            }
        }




        [XmlIgnore]
        public static TreeView TreeView { get; private set; } = new TreeView();

        [XmlIgnore]
        public static InterfaceDataSourceTopicTree TopicTreeView { get; } = new InterfaceDataSourceTopicTree();

        [XmlIgnore]
        public static object SelectedTreeViewItem => TreeView.SelectedItem;

        [XmlIgnore]
        private static TreeViewItem _favoriteTreeViewItem = new TreeViewItem();

        [XmlIgnore]
        public static Favorites Favorite => _subscriptions.Favorites;

        [XmlIgnore]
        public static List<Pushpin> Pushpins = new List<Pushpin>();
      
        #endregion

        #region CTOR
        static InterfaceDataSource()
        {
            Filter = false;
            UnreadOnly = false;
            Days = "1";
            Hours = "1";
            _subscriptions.FeedPropertyChanged += UpdateArticles;
            TreeView.MouseRightButtonDown += ContentTreeView_MouseRightButtonDown;
            TopicTreeView.topicTreeView.MouseRightButtonDown += ContentTreeView_MouseRightButtonDown;
            TopicTreeView.topicTreeView.SelectedItemChanged += (sender, args) => SelectedItemChanged?.Invoke(sender, args);
            TreeView.SelectedItemChanged += (sender, args) => SelectedItemChanged?.Invoke(sender, args);
            TreeView.ContextMenu = MakeTreeViewContextMenu();
            InitializeFavorites();

        }

        #endregion
    }
}
