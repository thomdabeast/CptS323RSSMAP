using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace Button_Crushers_RSS_Client
{
    public class InterfaceDataSourceTopicTree
    {
        private const string AddTopicText = "Add topic";
        private const string AddAggregateText = "Add topic Collection";
        private const string RemoveGestureText = @"Remove item";
        private const string RenameGesureText = @"Rename";

        public TreeView topicTreeView = new TreeView();

        public InterfaceDataSourceTopicTree()
        {
            topicTreeView.ContextMenu = MakeTopicTreeViewContextMenu();
        }

        private ContextMenu MakeTopicTreeViewContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(AddAggregateText, AddTopicContainerToParent));
            return contextMenu;
        }

        private void AddTopicContainerToParent(object sender, RoutedEventArgs e)
        {
            // Get user input
            var addChannelWindow = new HelperWindows.NameChannelWindow();
            addChannelWindow.ShowDialog();

            if (addChannelWindow.DialogResult == true)
            {
                TreeViewItem channelViewItem = InterfaceDataSource.MakeTreeViewItem(addChannelWindow.Channel);

                channelViewItem.ContextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(AddTopicText, AddTopicToContainer));
                channelViewItem.ContextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(RemoveGestureText, RemoveTopicOption));
                channelViewItem.ContextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(RenameGesureText, RenameTopicMenuOption));

                // Add to UI
                topicTreeView.Items.Add(channelViewItem);
            }
        }

        /// <summary>
        /// adds a topic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void AddTopicToContainer(object sender, RoutedEventArgs e)
        {
            var parent = topicTreeView.SelectedItem as TreeViewItem;

            if (parent != null)
            {
                // Get user input
                var addChannelWindow = new HelperWindows.NameChannelWindow();
                addChannelWindow.ShowDialog();

                if (addChannelWindow.DialogResult == true)
                {
                    TreeViewItem topicItem = InterfaceDataSource.MakeTreeViewItem(addChannelWindow.Channel);
                    topicItem.ContextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(RemoveGestureText, RemoveTopicOption));
                    topicItem.ContextMenu.Items.Add(InterfaceDataSource.MakeContextMenuItem(RenameGesureText, RenameTopicMenuOption));

                    // Add to UI
                    parent.Items.Add(topicItem);
                }
            }
        }

        /// <summary>
        /// Removes a topic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveTopicOption(object sender, RoutedEventArgs e)
        {
            var item = topicTreeView.SelectedItem as TreeViewItem;

            if (item != null)
            {
                var parent = item.Parent as TreeViewItem;

                if (parent == null)
                {
                    topicTreeView.Items.Remove(item);
                }
                else
                {
                    parent.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// renames a topic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameTopicMenuOption(object sender, RoutedEventArgs e)
        {
            var parent = topicTreeView.SelectedItem as TreeViewItem;

            if (parent != null)
            {
                var renameWindow = new HelperWindows.RenameWindow();
                renameWindow.ShowDialog();

                if (renameWindow.DialogResult == true)
                {
                    parent.Header = renameWindow.newName;
                }
            }
        }

        private const string Filename = @"topic-config-";


        public void SaveTree(string userName)
        {
            try
            {
                using (var writer = XmlWriter.Create(Filename+ userName+ ".xml"))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("TreeViewItems");
                    foreach (var treeNode in from object item in topicTreeView.Items select item as TreeViewItem)
                    {
                        writer.WriteStartElement("TreeItem");
                        WriteNode(writer, treeNode);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);          
            }
        }
        
        private void WriteNode(XmlWriter writer, TreeViewItem treeItem)
        {          
            writer.WriteElementString("Header", treeItem.Header.ToString());

            foreach (var treeNode in from object item in treeItem.Items select item as TreeViewItem)
            {
                writer.WriteStartElement("SubTreeItem");
                WriteNode(writer, treeNode);
                writer.WriteEndElement();
            }          
        }

        public void LoadTree(string userName)
        {
            //clear tree, can't just new it, otherwise we loose all the event subscribing
            foreach (var items in topicTreeView.Items)
            {
                topicTreeView.Items.Remove(items);
            }

            XDocument reader = XDocument.Load(XmlReader.Create(Filename + userName + ".xml"));

            foreach (var treeItems in reader.Descendants("TreeItem"))
            {
                var header = treeItems.Element("Header")?.Value;
                var treeItem = new TreeViewItem {Header = header};
                if (treeItems.HasElements)
                {
                    foreach (var subItem in treeItems.Elements("SubTreeItem"))
                    {
                        var subheader = subItem.Element("Header")?.Value;
                        var subTreeItem = new TreeViewItem {Header = subheader};
                        treeItem.Items.Add(subTreeItem);
                    }                                        
                }
                topicTreeView.Items.Add(treeItem);
            }
        }
    }
}
