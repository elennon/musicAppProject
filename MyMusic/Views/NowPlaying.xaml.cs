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
//using Microsoft.Xna.Framework;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlaying : Page
    {
        #region Private Fields and Properties

        private HttpBaseProtocolFilter filter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;

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
        #endregion
        
        public NowPlaying()
        {
            this.InitializeComponent();
            SererInitialized = new AutoResetEvent(false);
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            filter = new HttpBaseProtocolFilter();
            httpClient = new HttpClient(filter);
            cts = new CancellationTokenSource();

            App.Current.Suspending += ForegroundApp_Suspending;
            App.Current.Resuming += ForegroundApp_Resuming;
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);

            string arg = e.Parameter.ToString();

            if (arg == "shuffle")
            {
                orders = shuffleAll();
            }
            else
            {
                orders = new string[1];
                orders[0] = arg;
                isPlayRadio = true;
            }


            //switch (arg)
            //{
            //    case "shuffle":
            //        orders = shuffleAll();
            //        break;
            //}
            StartBackgroundAudioTask();
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

        private string[] radio()
        {            
            string[] trkks = new string[1];
            
            return trkks;
        }

        private string[] shuffleAll()
        {
            List<int> trks = new List<int>();
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            shuffled = trkView.GetShuffleTracks();
            string[] trkks = new string[shuffled.Count];
            for (int i = 0; i < shuffled.Count; i++)
            {
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name;
            }           
            return trkks;
        }

        

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

        #region Background MediaPlayer Event handlers

        async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Playing:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //playButton.Content = "| |";     // Change to pause button
                        //prevButton.IsEnabled = true;
                        //nextButton.IsEnabled = true;
                    }
                        );

                    break;
                case MediaPlayerState.Paused:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //playButton.Content = ">";     // Change to play button
                    }
                    );

                    break;
            }
        }

        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            string artist = "", title = "";
            string[] currentTrack = (e.Data.Values.FirstOrDefault().ToString()).Split(',');
            if (currentTrack[0] != string.Empty)
            {
                if (currentTrack.Length > 1)
                {
                    artist = currentTrack[1];
                    title = currentTrack[2];
                }
                else
                {
                    artist = currentTrack[0];
                }
            }

            foreach (string key in e.Data.Keys)
            {               
                switch (key)
                {
                    case Constants.Trackchanged:
                        //When foreground app is active change track based on background message
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            //imgPlayingTrack.Source = await Task.Run(() => getPic(trkName));
                            showPic(artist, title);
                            tbkSongName.Text = artist + "-" + title;
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        SererInitialized.Set();
                        break;
                }
            }
            
        }

        private async void showPic(string artist, string title)
        {
            Uri resourceUri;
            string secHalf =string.Format("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
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

        #region Button Click Event Handlers
        
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
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = SererInitialized.WaitOne(2000);
                //Send message to initiate playback
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

       
    }
}
