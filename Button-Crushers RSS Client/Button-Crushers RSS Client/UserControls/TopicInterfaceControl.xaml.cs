using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using RSSEngine;

namespace Button_Crushers_RSS_Client
{
    /// <summary>
    /// Interaction logic for TopicInterfaceControl.xaml
    /// </summary>
    public partial class TopicInterfaceControl
    {
        #region CTOR

        public TopicInterfaceControl()
        {
            InitializeComponent();
            MainWindow.DemoInterface += (arg, e) => Facade(e);
        }

        #endregion

        #region Methods

        public void Facade(string parameter)
        {
            if (parameter == "Topic")
            {

            }
        }



        private void TopicInterfaceControl1_Loaded(object sender, RoutedEventArgs e)
        {
            InterfaceDataSource.RemoveCheckboxesfromTree();
            TopicContentTreeView.Content = InterfaceDataSource.TopicTreeView.topicTreeView;
            InterfaceDataSource.SelectedItemChanged += ContentTreeView_SelectedItemChanged;
        }


        private void ContentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = sender as TreeView;

            if (tree != null)
            {
                var searchTerms = GetSearchTerms(InterfaceDataSource.TopicTreeView.topicTreeView.SelectedItem as TreeViewItem);

                //make sure we have not spaces
                var sanatizedTerms = (from terms in searchTerms
                                      select terms.Trim()).ToList();

                var searchTask = Task<List<Article>>.Factory.StartNew(() => InterfaceDataSource.TopicSearch(sanatizedTerms));
                var taskResult = searchTask.Result;

                ListViewContentControl.ItemsSource = taskResult;
            }
        }

        private List<string> GetSearchTerms(TreeViewItem treeViewItem)
        {
            var searchTerms = new List<string>();

            if (treeViewItem != null)
            {
                //no items mean that it is a terminal
                if (treeViewItem.Items.Count == 0)
                {
                    searchTerms.Add(treeViewItem.Header.ToString());
                }

                else
                {
                    searchTerms.AddRange(from TreeViewItem item in treeViewItem.Items select item.Header.ToString());
                }
            }

            return searchTerms;
        }




        private void TopicInterfaceControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            //MUST DO THIS, can only have one ref
            TopicContentTreeView.Content = null;
        }
        #endregion
    }
}
