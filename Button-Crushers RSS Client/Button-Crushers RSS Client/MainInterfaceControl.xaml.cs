<<<<<<< HEAD
﻿using System.Windows;
using RSSEngine;
=======
﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RSSEngine;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
>>>>>>> topic_interface

namespace Button_Crushers_RSS_Client
{
  /// <summary>
  ///   Interaction logic for MainInterfaceControl.xaml
  /// </summary>
<<<<<<< HEAD
    public partial class MainInterfaceControl
=======
  public partial class MainInterfaceControl
  {
    private const string AddFeedGestureText = "Add Feed";
    private const string AddChannelGestureText = "Add Channel";
    private const string RemoveGestureText = "Remove item";

    public MainInterfaceControl()
>>>>>>> topic_interface
    {
        public MainInterfaceControl()
        {
          InitializeComponent();
        }

<<<<<<< HEAD
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Window settings = new SettingsWindow();
        }

        public void LoadArticle(Article article)
        {
            MainInterfaceWebBrowser.Navigate(article.Link);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadArticle(new Article("a", "hilary", "1111", "http://www.cnn.com/2015/10/07/asia/doctors-without-borders-afghanistan-airstrike/index.html?eref=rss_topstories"));
        }
=======
    private void SettingButton_Click(object sender, RoutedEventArgs e)
    {
      Window settings = new SettingsWindow();
      settings.ShowDialog();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gestureText"></param>
    /// <param name="eventHandler"></param>
    /// <returns></returns>
    private MenuItem MakeContextMenuItem(string gestureText, RoutedEventHandler eventHandler)
    {
      MenuItem addItem = new MenuItem { InputGestureText = gestureText };
      addItem.Click += eventHandler;
      return addItem;
    }

    /// <summary>
    /// The call at the ui level
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddFeedMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
      // Get user input
      AddFeedWindow addFeedWindow = new AddFeedWindow();
      addFeedWindow.ShowDialog();

      if (addFeedWindow.DialogResult == true)
      {
        // Create feed and and add it to the engine
        Feed newFeed = new Feed(addFeedWindow.Feed.Item1, addFeedWindow.Feed.Item2, UpdateListCallBack, TaskScheduler.FromCurrentSynchronizationContext());
        Subscriptions.AddFeed(newFeed);

        newFeed.treeViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));

        // Add to UI
        ContentTreeView.Items.Add(newFeed.treeViewItem);
      }
    }

    /// <summary>
    /// this is the method to be used by the 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddChannelMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
      // Get user input
      NameChannelWindow createChannelWindow = new NameChannelWindow();
      createChannelWindow.ShowDialog();

      if (createChannelWindow.DialogResult == true)
      {
        // Create feed and add it
        Channel channel = new Channel(createChannelWindow.Channel);
        channel.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
        channel.ContextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItem));
        channel.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
        Subscriptions.AddChannel(channel);
        ContentTreeView.Items.Add(channel);
      }
    }

    private void RemoveMenuOption(object sender, RoutedEventArgs e)
    {
      var parent = ContentTreeView.SelectedItem;

      if (parent != null)
      {
        //todo need a generic remove in the rss
        //right now this will cause an exception because it is trying to change focus to something that doesn't exist
        ContentTreeView.Items.Remove(parent);
      }

    }

    /// <summary>
    /// This is the method to be used by other tree view items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddChannelItem(object sender, RoutedEventArgs e)
    {
      // Get input
      NameChannelWindow newChannel = new NameChannelWindow();
      newChannel.ShowDialog();

      TreeViewItem parent = ContentTreeView.SelectedItem as TreeViewItem;

      if (parent != null)
      {
        if (ContentTreeView.SelectedItem != null && true == newChannel.DialogResult)
        {
          // create channel and add
          Channel channel = new Channel(newChannel.Channel);
          channel.ContextMenu.Items.Add(MakeContextMenuItem(AddFeedGestureText, AddFeedItem));
          channel.ContextMenu.Items.Add(MakeContextMenuItem(AddChannelGestureText, AddChannelItem));
          channel.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
          parent.Items.Add(channel);
        }
      }
    }

    private void AddFeedItem(object sender, RoutedEventArgs e)
    {
      //this is NOT a good way to do this 
      TreeViewItem parent = ContentTreeView.SelectedItem as TreeViewItem;

      if (parent != null)
      {
        AddFeedWindow addFeedWindow = new AddFeedWindow();
        addFeedWindow.ShowDialog();

        if (true == addFeedWindow.DialogResult)
        {
          Feed newFeed = new Feed(addFeedWindow.Feed.Item1, addFeedWindow.Feed.Item2, UpdateListCallBack, TaskScheduler.FromCurrentSynchronizationContext());
          newFeed.treeViewItem.ContextMenu.Items.Add(MakeContextMenuItem(RemoveGestureText, RemoveMenuOption));
          parent.Items.Add(newFeed);
        }
      }
    }

    private void UpdateListCallBack(List<Article> newArticles, TreeViewItem feed)
    {
      try
      {
        //need to check this, otherwise an update to a different 
        //list will overwrite the currently selected one
        if (Equals(ContentTreeView.SelectedItem, feed.Header))
        {
          DataGrid.ItemsSource = newArticles;
        }
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.Message);
      }
    }

    private void ContentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      TreeView tree = sender as TreeView;

      if (tree != null)
      {
        List<Article> articleList = Subscriptions.GetArticles(((TreeViewItem)e.NewValue).Header.ToString());

        if (articleList != null)
        {
          DataGrid.ItemsSource = articleList;
        }
      }
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
        MainInterfaceWebBrowser.Navigate(link);
      }
      catch (UriFormatException)
      {
        MainInterfaceWebBrowser.NavigateToString("<h1>URL DOESN'T WORK!</h1>");
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
      LoadArticle((DataGrid.SelectedItem as Article).Link);
    }

    private void ContentTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      //find the original object that raised the event
      if (e.OriginalSource != null)
      {
        UIElement clickedItem = VisualTreeHelper.GetParent(e.OriginalSource as UIElement) as UIElement;

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
>>>>>>> topic_interface
    }
}