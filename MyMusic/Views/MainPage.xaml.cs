using BackgroundTask;
using MyMusic.Common;
using MyMusic.HelperClasses;
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
using Windows.Storage.Search;
using Windows.Storage.Streams;
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
        public DataGroup()
        { }

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

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //trkView.DoPercent();
            this.navigationHelper.OnNavigatedTo(e);
            LoadRadioList();
            LoadCollectionList();
            LoadStreamingList();

            //await trkView.SyncDB();
            //Task.Run(async delegate()
            //{
            //    await trkView.SyncDB();
            //});

            //Log.GetLog().Write("Main page nav tooooollooooooo");
            //Logger.GetLogger().logChannel.LogMessage("Main page nav to");
            //var ts = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
            //IReadOnlyList<StorageFile> lf = await ts.GetFilesAsync();
            // foreach (var item in lf)
            // {
            //     var ty = item.OpenReadAsync();
            //     string text = await Windows.Storage.FileIO.ReadTextAsync(item);               
            // }
        }
        
        private void LoadRadioList()
        {
            RadioSection.DataContext = rdoView.GetRadioGenres();
        }

        private void LoadCollectionList()
        {
            List<DataGroup> groups = new List<DataGroup>();
            DataGroup sg = new DataGroup { Title = "All Tracks", UniqueId = "All Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            sg = new DataGroup { Title = "Top Tracks", UniqueId = "Top Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            sg = new DataGroup { Title = "Artists", UniqueId = "Artist", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            sg = new DataGroup { Title = "Album", UniqueId = "Album", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            sg = new DataGroup { Title = "Genre", UniqueId = "Genre", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            sg = new DataGroup { Title = "Quick Pick", UniqueId = "QuickPick", ImagePath = "ms-appx:///Assets/music3.jpg" };
            groups.Add(sg);
            CollectionSection.DataContext = groups;
            //Hub.DataContext = groups;
        }

        private void LoadStreamingList()
        {
            List<DataGroup> groups = new List<DataGroup>();
            DataGroup sg = new DataGroup("Stream", "Search GrooveShark", "music streaming", "ms-appx:///Assets/music.jpg");
            groups.Add(sg);
            DataGroup sg1 = new DataGroup("Play Lists", "Play Lists", "music collection", "ms-appx:///Assets/music3.jpg");
            groups.Add(sg1);
            DataGroup sg2 = new DataGroup("Create PlayList", "Create PlayList", "online radio streaming", "ms-appx:///Assets/radio.jpg");
            groups.Add(sg2);            
            StreamingHubSection.DataContext = groups;
        }

        private void RadioStream_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rdoName = ((RadioGenreViewModel)e.ClickedItem).RadioGenreName;
            if (!Frame.Navigate(typeof(RadioStreams), rdoName))
            {
                Debug.WriteLine("navigation failed from main to radio lists ");
            }
        }

        private void Collection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((DataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(Collection), itemId))
            {
                Debug.WriteLine("navigation failed from main to collection ");
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
            //rdoView.AddGenre();
            rdoView.AddGenrePics();
        }

        private void ShortCutButton_Click(object sender, RoutedEventArgs e)
        {
            string rUrl = "radio,http://grooveshark.com/s/Outro+Eric+Melvin+Accordion+Solo/1ZlpLj?src=3";       //  http://tinysong.com/ieiB";
        
            if (!Frame.Navigate(typeof(NowPlaying), rUrl))
            {
                Debug.WriteLine("navigation failed from main to radio lists ");
            }
            //var tester = trkView.GetThisArtist("132");
            //this.Frame.Navigate(typeof(ShowAllTracks));
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            //readLog();
            //trkView.lookIn();
            //var getPicAndGenre = await trkView.getPic2("nofx", "bob");
            //var fr = getPicAndGenre.album.image.FirstOrDefault(); //Where(a => a.size == "large").FirstOrDefault();
            //var tyu = getPicAndGenre.toptags.tag.FirstOrDefault().name;
            //trkView.loadUpImagesAndGenre();
            //trkView.sortOrderNum();
            //var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
            //var filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            //var logSave = Logger.GetLogger().logSession.SaveToFileAsync(folder, filename).AsTask();
            //logSave.Wait();

            //string rUrl = "radio,http://37.58.75.163:9272/stream";

            //if (!Frame.Navigate(typeof(NowPlaying), rUrl))
            //{
            //    Debug.WriteLine("navigation failed from main to radio lists ");
            //}

            RadioStreamsViewModel rsvm = new RadioStreamsViewModel();
            var stst = rsvm.GetApiFillDB();
        }
       
        private void Streaming_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((DataGroup)e.ClickedItem).UniqueId;
            if(itemId == "Stream")
            if (!Frame.Navigate(typeof(Streaming)))
            {
                Debug.WriteLine("navigation failed from main to radio lists ");
            }
        }
        
    }    
}
