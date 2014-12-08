using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;


namespace MyMusic.Views
{
    
    public sealed partial class NowPlaying : Page
    {
        #region Private Fields and Properties

        //private HttpBaseProtocolFilter filter;
        //private HttpClient httpClient;
        //private CancellationTokenSource cts;
        private readonly NavigationHelper navigationHelper;

        private TracksViewModel trkView = new TracksViewModel();
        private string[] orders;
        private bool isPlayRadio = false;
        private AutoResetEvent SererInitialized;
        private bool backGroundIsRunning = false;

        private string CurrentTrack
        {
            get
            {
                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                if (value != null)
                {
                    return (String)value;
                }
                else
                    return String.Empty;
            }
        }

        private static string SkippedTrackName { get; set; }
        
        #endregion

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public NowPlaying()
        {
            this.InitializeComponent();
            SererInitialized = new AutoResetEvent(false);
            App.Current.Resuming += Current_Resuming;

            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            
            //filter = new HttpBaseProtocolFilter();
            //httpClient = new HttpClient(filter);
            //cts = new CancellationTokenSource();
        }

        void Current_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive); 
            AddMediaPlayerEventHandlers();
            Debug.WriteLine("in resume");
            bool bkrunning = false;
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.BackgroundTaskState))
            {
                Debug.WriteLine("FG  null returned");
            }
            else
            {
                var value = ApplicationData.Current.LocalSettings.Values[Constants.BackgroundTaskState];
                Debug.WriteLine("FG  bkrunning found " + value.ToString());
                bkrunning = ((String)value).Equals(Constants.BackgroundTaskRunning);
            }
            if (bkrunning)
            {
                ValueSet messageDictionary = new ValueSet();
                messageDictionary.Add(Constants.AppResumed, DateTime.Now.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);

                string pic = "";
                object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                if (value1 == null) { tbkSongName.Text = "current Track null"; }
                if (value1 != null)
                {
                    tbkSongName.Text = (string)value1 + "  resuming";
                }

                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackOrderNo);
                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
                else
                {
                    string trackId = (string)value2;
                    pic = (trkView.GetThisTrack(trackId)).ImageUri;
                    if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
                    imgPlayingTrack.Source = new BitmapImage(new Uri(pic));
                }
            }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //MessageDialog msgbox = new MessageDialog("this is nav ");
            //await msgbox.ShowAsync();
            Debug.WriteLine("load state");
            
            var arg = e.NavigationParameter;
            if (arg != null)// && ((App)Application.Current).isResumingFromTermination == false)
            {
                if (arg.ToString().Contains("shuffle")) { orders = shuffleAll(); }

                else if (arg.ToString().Contains("allTracks"))
                {
                    int trackNumber = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    orders = GetListToPlay(trackNumber);
                }
                else if (arg.ToString().Contains("albumTracks"))
                {
                    int albumId = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    orders = GetSongsAllInAlbum(albumId);
                }
                else if (arg.ToString().Contains("albTracksFromThisOn"))
                {
                    int trackIndex = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    int albumId = Convert.ToInt32((arg.ToString().Split(','))[2]);
                    orders = GetSongsInAlbumFromThis(trackIndex, albumId);
                }
                else if (arg.ToString().Contains("artistTracks"))
                {
                    int artistId = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    orders = GetSongsByThisArtist(artistId);
                }
                else if (arg.ToString().Contains("radio"))
                {
                    orders = new string[1];
                    orders[0] = (arg.ToString().Split(','))[1];
                    isPlayRadio = true;
                }
                //if (((App)Application.Current).IsMyBackgroundTaskRunning)
                //{
                    if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                    {
                        StartBackgroundAudioTask();
                    }
                //}
                else
                {
                    StartBackgroundAudioTask();
                }
            }
            else //if (backGroundIsRunning) 
            {
                string pic = "";
                object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                if (value1 == null) { tbkSongName.Text = "current Track null"; }
                if (value1 != null)
                {
                    tbkSongName.Text = (string)value1 + "  not restore";
                }

                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackOrderNo);
                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
                else
                {
                    string trackId = (string)value2;
                    pic = (trkView.GetThisTrack(trackId)).ImageUri;
                    if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
                    imgPlayingTrack.Source = new BitmapImage(new Uri(pic));
                }
            }                       
        }
       
        #region playlist managing

        private string[] shuffleAll()
        {
            List<int> trks = new List<int>();
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            shuffled = trkView.GetShuffleTracks();
            string[] trkks = new string[shuffled.Count];
            for (int i = 0; i < shuffled.Count; i++)
            {
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
            }
            return trkks;
        }

        private string[] GetListToPlay(int orderNo) // orders all songs that come after selected song (+ selected) into a string[]
        {
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            var trks = (trkView.GetTracks()).Where(a => a.OrderNo >= orderNo).ToList(); // get all tracks listed after selected one
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].Artist + "," + trks[i].Name + ",shuffle";
            }
            return trkArray;
        }

        private string[] GetSongsAllInAlbum(int albumId) // orders all songs in album into a string[]
        {
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            var trks = (trkView.GetTracksByAlbum(albumId.ToString())).ToList();         // get all tracks in given album
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].Artist + "," + trks[i].Name + ",notShuffle";
            }
            return trkArray;
        }

        private string[] GetSongsInAlbumFromThis(int trackIndex, int albumId) // orders all songs in album into a string[]
        {
            List<string> tracks = new List<string>();
            var trks = (trkView.GetTracksByAlbum(albumId.ToString())).ToList();         // get all tracks in given album    trkView.GetTracksByAlbum(para.ToString());
            
            for (int i = trackIndex; i < trks.Count; i++)
            {
                tracks.Add(trks[i].TrackId.ToString() + "," + trks[i].Artist + "," + trks[i].Name + ",notShuffle");
            }
            string[] trkArray = new string[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                trkArray[i] = tracks[i];
            }
            return trkArray;
        }

        private string[] GetSongsByThisArtist(int id)
        {
            ObservableCollection<TrackViewModel> tracks = trkView.GetTracksByArtist(id);
            string[] trks = new string[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                trks[i] = tracks[i].TrackId.ToString() + "," + tracks[i].Artist + "," + tracks[i].Name + ",notshuffle";
            }
            return trks;
        }

        #endregion

        #region Background messages

        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            Debug.WriteLine("message recieved loud and clear");
            string artist = "", title = "", trackId = "";
            string[] currentTrack = (e.Data.Values.FirstOrDefault().ToString()).Split(',');     // current track will be a comma seperated string with name, artist...
            if (currentTrack[0] != string.Empty)
            {
                if (currentTrack.Length > 1)
                {
                    trackId = currentTrack[0];
                    artist = currentTrack[1];
                    title = currentTrack[2];
                    if (currentTrack[3].Contains("shuffle")) //  if its a track from random shuffle
                    {
                        trkView.AddRandomPlay(artist, title);
                    }
                    else if (currentTrack[3].Contains("notShuffle")) 
                    {
                        trkView.AddPlay(artist, title); // if it was chosen specifically
                    }
                    if (currentTrack.Count() > 4) //  if its got the extra word, its a skipped track
                    {
                        //AddSkipped();
                        trkView.AddSkip(trackId);
                    }
                }
                else
                {
                    artist = currentTrack[0];   // this is a radio station
                }
            }
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            //imgPlayingTrack.Source = await Task.Run(() => getPic(trkName));
                            //showPic(artist, title);
                            string pic = (trkView.GetThisTrack(trackId)).ImageUri;
                            if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
                            imgPlayingTrack.Source = new BitmapImage(new Uri(pic));    // get the image for this song
                            tbkSongName.Text = artist + "-" + title;
                            SkippedTrackName = artist + "-" + title;

                            //var message = new ValueSet();
                            //message.Add(Constants.CurrentTrackImg, pic);
                            //BackgroundMediaPlayer.SendMessageToBackground(message);
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        SererInitialized.Set();
                        break;
                }
            }

        }

        //private async void testApi()
        //{
        //    Uri resourceUri;
        //    string secHalf = "http://localhost:59436/api/connection";
        //    if (!Helpers.TryGetUri(secHalf, out resourceUri))
        //    {
        //        //rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
        //        return;
        //    }
        //    try
        //    {
        //        HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
        //        var xmlString = response.Content.ReadAsStringAsync().GetResults();
        //        XDocument doc = XDocument.Parse(xmlString);

        //        if (doc.Root.FirstAttribute.Value == "failed")
        //        {
        //            //imgPlayingTrack.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri("Assets/PicPlaceholder.png", UriKind.Relative));
        //            imgPlayingTrack.Source = new BitmapImage(new Uri("ms-appx:///Assets/radio672.png", UriKind.Absolute));
        //        }
        //        else
        //        {
        //            string picc = (from el in doc.Descendants("image")
        //                           where (string)el.Attribute("size") == "large"
        //                           select el).First().Value;
        //            if (!string.IsNullOrEmpty(picc))
        //            { imgPlayingTrack.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri(picc)); }

        //        }

        //    }
        //    catch (Exception exx) { string error = exx.Message; }
        //    //imgPlayingTrack.Source = await getPic(trkName);
        //}        

        #endregion

        #region Button Clicks

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            var value = new ValueSet();
            value.Add(Constants.SkipPrevious, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            //if (((App)Application.Current).IsMyBackgroundTaskRunning)
            //{
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Play();
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
            //}
            else
            {
                StartBackgroundAudioTask();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var value = new ValueSet();
            value.Add(Constants.SkipNext, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        #endregion Button Click Event Handlers

        #region setup/start background task

        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        public void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void StartBackgroundAudioTask()             // starts background---sends mesaage (tracks to play) from here
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = SererInitialized.WaitOne(2000);
                //bool result = true;
                if (result == true)
                {
                    if (isPlayRadio == true)
                    {
                        var message = new ValueSet();
                        message.Add(Constants.PlayRadio, orders);
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                    else
                    {
                        var message = new ValueSet();
                        message.Add(Constants.StartPlayback, orders);
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            }
            );
            backgroundtaskinitializationresult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }

        #endregion
       
        #region Navigation

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            Debug.WriteLine("nav to");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((App)Application.Current).isResumingFromTermination = false;
            this.navigationHelper.OnNavigatedFrom(e);
            RemoveMediaPlayerEventHandlers();
        }

        #endregion
    }
}



