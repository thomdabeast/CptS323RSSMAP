using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RSSEngine;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Maps.MapControl.WPF;

namespace Button_Crushers_RSS_Client
{
    /// <summary>
    ///   Interaction logic for MainInterfaceControl.xaml
    /// </summary>
    public partial class MainInterfaceControl
    {
        #region Attributes

        private readonly TaskScheduler _context = TaskScheduler.FromCurrentSynchronizationContext();

        #endregion

        #region CTOR

        public MainInterfaceControl()
        {
            const string readHeader = "Read/Unread";
            const string favoriteHeader = "Favorite/Unfavorite";
            InitializeComponent();
            MainInterfaceWebBrowser.Navigated += MainInterfaceWebBrowser_Navigated;
            var read = new MenuItem();
            read.Click += Read_Click;
            read.Header = readHeader;
            var favorite = new MenuItem();
            favorite.Click += Favorite_Click;
            favorite.Header = favoriteHeader;
            ArticleDataGrid.ContextMenu = new ContextMenu
            { Items = { read, favorite } };
            MainWindow.DemoInterface += (sender, arg) => Facade(arg);
            MainWindow.SpeechRead += (sender, arg) => Facade(arg.ToString());
        }

        private void MainWindow_TabControlItemChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as TabControl)?.SelectedIndex != 0)
            {
                MainInterfaceWebBrowser.Source = null;
            }
        }

        private void InterfaceDataSource_PushpinDoubleClick(object o, MouseButtonEventArgs e)
        {
            Pushpin pp = o as Pushpin;
            Article a = pp.Tag as Article;
            LoadArticle(a.Link.ToString());
        }

        #endregion

        #region Methods

        public void Facade(string parameter)
        {
            if (parameter == "Main" || parameter == "demo")
            {
                var parent = InterfaceDataSource.AddChannelToTree();

                InterfaceDataSource.AddFeedToTree(parent);
            }
            else if (parameter == "select article")
            {
                ArticleDataGrid.SelectedIndex = 0;
            }
            else if (parameter == "next article")
            {
                if (ArticleDataGrid.SelectedIndex == ArticleDataGrid.Items.Count)
                {
                    ArticleDataGrid.SelectedIndex = 0;
                }
                else
                {
                    ArticleDataGrid.SelectedIndex++;

                }
            }
            else if (parameter == "previous article")
            {
                if (ArticleDataGrid.SelectedIndex == 0)
                {
                    ArticleDataGrid.SelectedIndex = ArticleDataGrid.Items.Count;
                }
                else
                {
                    ArticleDataGrid.SelectedIndex--;

                }
            }
            else if (parameter == "read article")
            {
                Article a = (ArticleDataGrid.SelectedItem as Article);
                a.HasBeenRead = "read";
                MainInterfaceWebBrowser.IsEnabled = true;
                LoadArticle(a.Link);
            }
            else if (parameter == "unread article")
            {
                Article a = (ArticleDataGrid.SelectedItem as Article);
                a.HasBeenRead = "unread";
            }
        }


        #region Event Handlers

        /// <summary>
        /// Update the header of an article on a click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Read_Click(object sender, RoutedEventArgs e)
        {
            ((Article)ArticleDataGrid.SelectedItem).HasBeenRead =
                (((Article)ArticleDataGrid.SelectedItem)?.HasBeenRead == "read")
                ? "unread"
                : "read";
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            //do operations based on favorite or not
            var myArticle = (Article)ArticleDataGrid.SelectedItem;
            InterfaceDataSource.EditFavorites(myArticle);             
        }

        /// <summary>
        /// On loaded, update the reference the content presenter holds and subscribe to events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InterfaceDataSource.PushpinDoubleClick += InterfaceDataSource_PushpinDoubleClick;
            MainWindow.TabControlItemChanged += MainWindow_TabControlItemChanged;
            InterfaceDataSource.RemoveCheckboxesfromTree();
            ContentTreeView.Content = InterfaceDataSource.TreeView;
            InterfaceDataSource.DataPropertyChanged += UpdateItemSource;
            InterfaceDataSource.SelectedItemChanged += ContentTreeView_SelectedItemChanged;
        }

        /// <summary>
        /// On unloaded clear the reference the content presenter holds and unsubscribe to events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //MUST DO THIS, can only have one ref
            ContentTreeView.Content = null;
            InterfaceDataSource.DataPropertyChanged -= UpdateItemSource;
            InterfaceDataSource.SelectedItemChanged -= ContentTreeView_SelectedItemChanged;
        }

        /// <summary>
        /// On the ContentTreeviews selected change update the item source for the data grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var arg = (e.NewValue as TreeViewItem)?.Header.ToString();

            if (arg == "Favorites")
            {
                ArticleDataGrid.ItemsSource = InterfaceDataSource.Favorite.Articles;
            }
            else
            {
                TreeView tree = sender as TreeView;

                if (tree != null)
                {
                    ArticleDataGrid.ItemsSource =
                        InterfaceDataSource.GetFilteredArticles(
                            InterfaceDataSource.Search((e.NewValue as TreeViewItem)?.Header.ToString()));
                }
            }
        }

        /// <summary>
        /// Event handler: For each DataGridRow's "Link" column
        /// When the HyperlinkColumn is clicked this will feed the articles link to the web browser control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ArticleDataGrid.SelectedItem as Article;

            if (selectedItem != null)
            {
                selectedItem.HasBeenRead = "read";
                LoadArticle(selectedItem.Link);
            }
        }

        /// <summary>
        /// Supress any JS error on navigation in the web browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainInterfaceWebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            HideScriptErrors(MainInterfaceWebBrowser, true);
        }

        #endregion

        /// <summary>
        /// hide stupid js errors on the stupid browser
        /// </summary>
        /// <remarks>
        /// taken from 
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/4f686de1-8884-4a8d-8ec5-ae4eff8ce6db/new-wpf-webbrowser-how-do-i-suppress-script-errors?forum=wpf
        /// </remarks>
        /// <param name="wb"></param>
        /// <param name="hide"></param>
        private static void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            var objComWebBrowser = fiComWebBrowser?.GetValue(wb);

            objComWebBrowser?.GetType().InvokeMember(
                "Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }


        /// <summary>
        /// Load an article
        /// </summary>
        /// <param name="link"></param>
        /// <remarks>
        /// On a uriformat exception catch it and display a dummy page
        /// </remarks>
        private void LoadArticle(string link)
        {
            try
            {
                if (link != null)
                {
                    MainInterfaceWebBrowser.Navigate(link);
                }
            }
            catch (UriFormatException)
            {
                MainInterfaceWebBrowser.NavigateToString("<h1>URL DOESN'T WORK!</h1>");
            }
        }

        /// <summary>
        /// Updates an items ItemSource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateItemSource(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var feedToUpdate = sender as Feed;
                var currentName = (InterfaceDataSource.SelectedTreeViewItem as TreeViewItem)?.Header.ToString();

                if (currentName == feedToUpdate?.Name)
                {
                    ArticleDataGrid.ItemsSource = InterfaceDataSource.GetFilteredArticles(feedToUpdate);
                }
            }, Task.Factory.CancellationToken, TaskCreationOptions.None, _context);
        }
        
        

        #endregion
    }
}