using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyMusic.Views
{
    public class DataGroup
    {
        public DataGroup(String UniqueId, String title, String description, String imagePath)
        {
            this.UniqueId = UniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
        }

        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
    public sealed partial class MainPage : Page
    {
        private TracksViewModel trkView = new TracksViewModel();
        private RadioStreamsViewModel rdoView = new RadioStreamsViewModel();
        
        private readonly NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        

        public MainPage()
        {
            InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);

            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            //lstOptions.SelectedIndex = -1;
            List<DataGroup> groups = new List<DataGroup>();
            DataGroup sg = new DataGroup("Stream", "Streaming", "music streaming", "ms-appx:///Assets/music.jpg" );
            groups.Add(sg);
            DataGroup sg1 = new DataGroup("Collection", "Collection", "music collection", "ms-appx:///Assets/music3.jpg");
            groups.Add(sg1);
            DataGroup sg2 = new DataGroup("Radio", "Online Radio", "online radio streaming", "ms-appx:///Assets/radio.jpg");
            groups.Add(sg2);
            Hub.DataContext = groups;
            //await trkView.SyncDB();
            Task.Run(async delegate()
            {
                await trkView.SyncDB();
            });
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((DataGroup)e.ClickedItem).UniqueId;
            switch (itemId)
            {
                case "Stream":
                    this.Frame.Navigate(typeof(Streaming));
                    break;
                case "Collection":
                    this.Frame.Navigate(typeof(Collection));
                    break;
                case "Radio":
                    //RadioStream rs = new RadioStream { RadioUrl = "apples" };     
                    //this.Frame.Navigate(typeof(NowPlaying), rs);
                    this.Frame.Navigate(typeof(RadioStreams));
                    break;
            }
        }

        private void lstOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedIndex != -1)
            {
                string title = ((ListBoxItem)lb.SelectedItem).Tag.ToString();
                switch (title)
                {
                    case "Stream":
                        this.Frame.Navigate(typeof(Streaming));
                        break;
                    case "Collection":
                        this.Frame.Navigate(typeof(Collection));
                        break;
                    case "Radio":
                        //RadioStream rs = new RadioStream { RadioUrl = "apples" };     
                        //this.Frame.Navigate(typeof(NowPlaying), rs);
                        this.Frame.Navigate(typeof(RadioStreams));
                        break;
                }
            }
        }

       

        private void btnNowPlaying_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NowPlaying));
        }

        private void FillDbButton_Click(object sender, RoutedEventArgs e)
        {
            trkView.fillDB();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //rdoView.AddRadios();
            //trkView.DropDB();
        }

        private void ShortCutButton_Click(object sender, RoutedEventArgs e)
        {
            var tester = trkView.GetThisArtist("132");
        }

        #region NavigationHelper

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            //readLog();
        }

    }
}
