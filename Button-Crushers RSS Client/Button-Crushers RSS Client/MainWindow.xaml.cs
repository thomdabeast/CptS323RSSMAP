using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button_Crushers_RSS_Client.Map_Utility;
using RSSEngine;
using Microsoft.Maps.MapControl.WPF;
using System.Speech.Recognition;

namespace Button_Crushers_RSS_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Delegate for facades
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void FacadeDelegate (object o, string e);

        /// <summary>
        /// For parent windows to know when to demo
        /// </summary>
        public static event FacadeDelegate DemoInterface = delegate { };

        /// <summary>
        /// MainInterface will use this to reset it's web browser
        /// </summary>
        public static event RoutedEventHandler TabControlItemChanged = delegate { };

        public delegate void SpeechRecognized(object o, string e);
        public static event SpeechRecognized SpeechRead = delegate { };

        #region Attributes

        #endregion

        #region Properties

        public bool UnreadOnly
        {
            get { return InterfaceDataSource.UnreadOnly; }
            set
            {
                InterfaceDataSource.UnreadOnly = value;
            }
        }

        public bool Filter
        {
            get { return InterfaceDataSource.Filter; }
            set
            {
                InterfaceDataSource.Filter = value;
            }
        }

        public string Days
        {
            get { return InterfaceDataSource.Days; }
            set
            {

                InterfaceDataSource.Days = ComboBoxDays.Text;
            }
        }

        public string Hours
        {
            get { return InterfaceDataSource.Hours; }
            set
            {
                InterfaceDataSource.Hours = ComboBoxHours.Text;
            }
        }

        public string RefreshRate
        {
            get { return InterfaceDataSource.RefreshRate; }
            set
            {
                InterfaceDataSource.RefreshRate = RefrestComboBox.Text;
            }
        }

        public static SpeechRecognizer Speech;
        private Grammar Grammar;

        #endregion

        #region CTOR

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();

            Speech = new SpeechRecognizer();

            Choices commands = new Choices();
            commands.Add(new string[]
                {
                    "add feed", "add channel", "select article", "read article", "unread article", "next article", "previous article",
                    "view main", "view topic", "view map",
                    "save", "load", "exit"
                });
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commands);
            Grammar = new Grammar(gb);
            Speech.LoadGrammar(Grammar);
            Speech.SpeechRecognized += Speech_SpeechRecognized;
        }

        private void Speech_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "add feed")
            {
                InterfaceDataSource.AddFeedToTree();
            }
            else if (e.Result.Text == "add channel")
            {
                InterfaceDataSource.AddChannelToTree();
            }
            else if (e.Result.Text == "save")
            {
                var save = new HelperWindows.SaveWindow();
                save.ShowDialog();
            }
            else if (e.Result.Text == "load")
            {
                var load = new HelperWindows.LoadWindow();
                load.ShowDialog();
            }
            else if (e.Result.Text == "help")
            {
                var help = new HelperWindows.HelpWindow();
                help.ShowDialog();
            }
            else if (e.Result.Text == "about")
            {
                var about = new HelperWindows.AboutWindow();
                about.ShowDialog();
            }
            else if (e.Result.Text == "exit")
            {
                this.Close();
            }
            else
            {
                SpeechRead?.Invoke(sender, e.Result.Text);
            }
        }

        private void InterfaceDataSource_PushpinDoubleClick(object o, System.Windows.Input.MouseButtonEventArgs e)
        {
            TabControl.SelectedIndex = 0;
        }


        #endregion

        #region Methods

        #region Event Handlers

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((TabItem)e.AddedItems[0])?.Header.ToString() == MapTabItem?.Header.ToString())
                {
                    InterfaceDataSource.Builder = new CheckBoxBuilder();
                }
                else if (e.RemovedItems.Count > 0 &&
                    ((TabItem)e.RemovedItems[0])?.Header.ToString() == MapTabItem?.Header.ToString())
                {
                    InterfaceDataSource.Builder = new PlainBuilder();
                }
            }
            catch (Exception)
            {

            }
            TabControlItemChanged?.Invoke(sender, e);
        }

        private void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var save = new HelperWindows.SaveWindow();
            save.ShowDialog();
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var load = new HelperWindows.LoadWindow();
            load.ShowDialog();
        }

        private void HelpMenuOption_OnClick(object sender, RoutedEventArgs e)
        {
            var help = new HelperWindows.HelpWindow();
            help.ShowDialog();
        }

        private void AboutMenuOption_OnClick(object sender, RoutedEventArgs e)
        {
            var about = new HelperWindows.AboutWindow();
            about.ShowDialog();
        }

        private void DemoMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Facade();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                cb.Content = @"Speech is on";
                //InterfaceDataSource.Speech.Enabled = true;
            }
            else
            {
                cb.Content = @"Speech is off";
                //InterfaceDataSource.Speech.Enabled = false;
            }
        }

        #endregion


        public void Facade()
        {
            MainTabItem.IsSelected = true;
            DemoInterface?.Invoke(this, "Main");
            SearchTabItem.IsSelected = true;
            DemoInterface?.Invoke(this, "Topic");
            MapTabItem.IsSelected = true;
            DemoInterface?.Invoke(this, "Map");
        }

        private void InitializeUI()
        {
            MainContentControl.Content = new MainInterfaceControl();
            TopicContentControl.Content = new TopicInterfaceControl();
            MapContentControl.Content = new MapInterfaceControl();
            ComboBoxDays.ItemsSource = new List<int>(Enumerable.Range(0, 31));
            ComboBoxHours.ItemsSource = new List<int>(Enumerable.Range(0, 25));
            RefrestComboBox.ItemsSource = new List<int>(Enumerable.Range(15, 106));
            FilterCheckBox.DataContext = this;
            UnreadOnlyCheckBox.DataContext = this;
            ComboBoxDays.DataContext = this;
            ComboBoxHours.DataContext = this;
            RefrestComboBox.DataContext = this;         
        }


        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           Speech.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InterfaceDataSource.PushpinDoubleClick += InterfaceDataSource_PushpinDoubleClick;
        }
    }
}
