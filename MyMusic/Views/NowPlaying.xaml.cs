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

        private HttpBaseProtocolFilter filter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;
        private readonly NavigationHelper navigationHelper;

        private TracksViewModel trkView = new TracksViewModel();       
        private string[] orders;
        private bool isPlayRadio = false;
        private AutoResetEvent SererInitialized;

        private bool isMyBackgroundTaskRunning = false;     
        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (isMyBackgroundTaskRunning)
                    return true;

                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.BackgroundTaskState);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    isMyBackgroundTaskRunning = ((String)value).Equals(Constants.BackgroundTaskRunning);
                    return isMyBackgroundTaskRunning;
                }
            }
        }

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

        private string CurrentTrackImg
        {
            get
            {
                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrackImg);
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

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //testApi();
            filter = new HttpBaseProtocolFilter();
            httpClient = new HttpClient(filter);
            cts = new CancellationTokenSource();
            App.Current.Suspending += ForegroundApp_Suspending;
            App.Current.Resuming += ForegroundApp_Resuming;
            

            this.navigationHelper.OnNavigatedTo(e);

            var arg = e.Parameter;
            if (arg != null)
            {
                if (arg.GetType() == typeof(string[]))
                {
                    orders = (string[])arg;
                }
                else if (arg.GetType() == typeof(RadioStream))
                {
                    orders = new string[1];
                    orders[0] = ((RadioStream)arg).RadioUrl;
                    isPlayRadio = true;
                }
            }
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    var message = new ValueSet();
                    message.Add(Constants.StartPlayback, orders);
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                    var message = new ValueSet();
                    message.Add(Constants.StartPlayback, orders);
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
            }
            else
            {
                StartBackgroundAudioTask();
            }
        }

        #region Background MediaPlayer messages

        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
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
                    else
                    {
                        trkView.AddPlay(artist, title); // if it was chosen specifically
                    }
                    if(currentTrack.Count() > 4) //  if its got the extra word, its a skipped track
                    {
                        AddSkipped();
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

                            var message = new ValueSet();
                            message.Add(Constants.CurrentTrackImg, pic);
                            BackgroundMediaPlayer.SendMessageToBackground(message);
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        SererInitialized.Set();
                        break;
                }
            }
            
        }

        private void AddSkipped()
        {  
            string[] songToSkip = SkippedTrackName.Split('-');  // should get the name of the previously playing track that was skipped
            trkView.AddSkip(songToSkip[0], songToSkip[1]);
        }

        private async void testApi()
        {
            Uri resourceUri;
            string secHalf = "http://localhost:59436/api/connection";
            if (!Helpers.TryGetUri(secHalf, out resourceUri))
            {
                //rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage);
                return;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();
                XDocument doc = XDocument.Parse(xmlString);

                if (doc.Root.FirstAttribute.Value == "failed")
                {
                    //imgPlayingTrack.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri("Assets/PicPlaceholder.png", UriKind.Relative));
                    imgPlayingTrack.Source = new BitmapImage(new Uri("ms-appx:///Assets/radio672.png", UriKind.Absolute));
                }
                else
                {
                    string picc = (from el in doc.Descendants("image")
                                   where (string)el.Attribute("size") == "large"
                                   select el).First().Value;
                    if (!string.IsNullOrEmpty(picc))
                    { imgPlayingTrack.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri(picc)); }

                }

            }
            catch (Exception exx) { string error = exx.Message; }
            //imgPlayingTrack.Source = await getPic(trkName);
        }        

        private async void showPic(string artist, string title)
        {
            Uri resourceUri;
            string address =string.Format("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
            if (!Helpers.TryGetUri(address, out resourceUri))
            {
                return;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();
                XDocument doc = XDocument.Parse(xmlString);

                string picc = "";

                if (doc.Root.FirstAttribute.Value == "failed")
                {
                    picc = "ms-appx:///Assets/radio672.png";
                    imgPlayingTrack.Source = new BitmapImage(new Uri(picc, UriKind.Absolute));                    
                }
                else
                {
                    picc = (from el in doc.Descendants("image")
                                   where (string)el.Attribute("size") == "large"
                                   select el).First().Value;
                    if (!string.IsNullOrEmpty(picc))
                    { imgPlayingTrack.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri(picc)); }
                }
                
                var message = new ValueSet();
                message.Add(Constants.CurrentTrackImg, picc);
                BackgroundMediaPlayer.SendMessageToBackground(message);

                //Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;                
                //roamingSettings.Values["CurrentImg"] = picc;
            }
            catch (Exception exx) { string error = exx.Message; }
        }        

        public async Task<BitmapImage> getPic(string trkName)
        {

            BitmapImage albumArtImage = new BitmapImage();

            if (trkName != null)
            {
                StorageFolder folder = KnownFolders.MusicLibrary;
                IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync();
                foreach (var item in lf)
                {
                    var song = await item.Properties.GetMusicPropertiesAsync();
                    if (song.Title == trkName)
                    {
                        StorageItemThumbnail img = await item.GetThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);
                        albumArtImage.SetSource(img);
                    }
                }
            }
            return albumArtImage;
        }

        #endregion

        #region Button Click Events
        
        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            var value = new ValueSet();
            value.Add(Constants.SkipPrevious, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);

            //prevButton.IsEnabled = false;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsMyBackgroundTaskRunning)
            {
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
            }
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

            //nextButton.IsEnabled = false;
        }

        #endregion Button Click Event Handlers

        #region Media Playback Helper methods
       
        private void RemoveMediaPlayerEventHandlers()
        {
            //BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void AddMediaPlayerEventHandlers()
        {
            //BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void StartBackgroundAudioTask()             // starts background---sends mesaage (tracks to play) from here
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = SererInitialized.WaitOne(2000);
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

        #region Foreground App Lifecycle Handlers

        void ForegroundApp_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);

            // Verify if the task was running before
            if (IsMyBackgroundTaskRunning)
            {
                //if yes, reconnect to media play handlers
                AddMediaPlayerEventHandlers();

                //send message to background task that app is resumed, so it can start sending notifications
                ValueSet messageDictionary = new ValueSet();
                messageDictionary.Add(Constants.AppResumed, DateTime.Now.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);

                tbkSongName.Text = CurrentTrack;
                imgPlayingTrack.Source = new BitmapImage(new Uri(CurrentTrackImg));
            }
            else
            {
                tbkSongName.Text = "";
            }

        }

        void ForegroundApp_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            ValueSet messageDictionary = new ValueSet();
            messageDictionary.Add(Constants.AppSuspended, DateTime.Now.ToString());
            BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
            RemoveMediaPlayerEventHandlers();
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppSuspended);
            deferral.Complete();
        }

        #endregion

        #region NavigationHelper 

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
            if (value != null)
            {
                tbkSongName.Text = (string)value;              
            }

            object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrackImg);
            if (value2 != null)
            {
                string pic = (string)value2;
                imgPlayingTrack.Source = new BitmapImage(new Uri(pic)); 
            }           
        }      

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}











