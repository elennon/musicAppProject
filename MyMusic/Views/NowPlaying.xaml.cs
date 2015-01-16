using MyMusic.Common;
using MyMusic.HelperClasses;
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
        #region Properties

        private readonly NavigationHelper navigationHelper;

        private TracksViewModel trkView = new TracksViewModel();
        private string[] orders;
        private bool isPlayRadio = false, isPlayGSTrack = false;
        private AutoResetEvent SererInitialized;
        //private bool backGroundIsRunning = false;

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
            Debug.WriteLine("in now playing Constructor");
            this.InitializeComponent();
            SererInitialized = new AutoResetEvent(false);
            App.Current.Resuming += Current_Resuming;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        void Current_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive); 
            AddMediaPlayerEventHandlers();
            Debug.WriteLine("in fg Current_Resuming");
            
       //     Logger.GetLogger().logChannel.LogMessage("In FG Current_Resuming");

            bool bkrunning = false;
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.BackgroundTaskState))
            {
                Debug.WriteLine("FG: BK null returned");
            }
            else
            {
                var value = ApplicationData.Current.LocalSettings.Values[Constants.BackgroundTaskState];
                Debug.WriteLine("FG:  bk running " + value.ToString());
                if (value.ToString() == "BKRunning")
                    bkrunning = true;
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

                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
                else
                {
                    int trackId = (int)value2;
                    pic = (trkView.GetThisTrack(trackId)).ImageUri;
                    if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
                    imgPlayingTrack.Source = new BitmapImage(new Uri(pic));
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            this.navigationHelper.OnNavigatedTo(e);           
            Debug.WriteLine("in now playing nav to");
        
            var arg = e.Parameter;
            if (arg != null && ((App)Application.Current).isResumingFromTermination == false)
            {
                if (arg.ToString().Contains("shuffleAll")) { orders = trkView.shuffleAll(); }

                else if (arg.ToString().Contains("shuffleThese"))       // for a collection to shuffle play e.g. tracks in album or by genre
                {
                    string id = (arg.ToString().Split(',')[1]);
                    string type = (arg.ToString().Split(','))[2];
                    if (type.Contains("album"))
                    {
                        orders = trkView.ShuffleAlbum(id);
                    }
                    if (type.Contains("genre"))
                    {
                        orders = trkView.ShuffleGenre(id);
                    }
                    if (type.Contains("topplay"))
                    {
                        orders = trkView.ShuffleTopPlays();
                    }
                }
                else if (arg.ToString().Contains("allTracks"))          // track selected in all track list and top track list. playes selected and then all listed after
                {
                    int trackNumber = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    orders = trkView.GetListToPlay(trackNumber);
                }
                else if (arg.ToString().Contains("albumTracks"))
                {
                    int albumId = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    orders = GetSongsAllInAlbum(albumId);
                }
                else if (arg.ToString().Contains("albTracksFromThisOn"))
                {
                    int trackId = Convert.ToInt32((arg.ToString().Split(','))[1]);
                    int albumId = Convert.ToInt32((arg.ToString().Split(','))[2]);
                    orders = GetSongsInAlbumFromThis(trackId, albumId);
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
                else if (arg.ToString().Contains("gsStreamTrack"))
                {
                    orders = new string[1];
                    orders[0] = (arg.ToString().Split(','))[1];
                    isPlayGSTrack = true;
                }
                
                if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing )      //  if BK is running its a selection change
                {
                    Task.Run( delegate()
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
                    });                    
                }
                else
                {
                    StartBackgroundAudioTask();     // else start it up
                }
            }
            else 
            {
                string pic = "";
                object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                if (value1 == null) { tbkSongName.Text = "current Track null"; }
                if (value1 != null)
                {
                    tbkSongName.Text = (string)value1 + "  not restore";
                }

                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
                else
                {
                    int trackId = (int)value2;
                    pic = (trkView.GetThisTrack(trackId)).ImageUri;
                    if (string.IsNullOrEmpty(pic)) { pic = "ms-appx:///Assets/music3.png"; }
                    imgPlayingTrack.Source = new BitmapImage(new Uri(pic));
                    imgPlayingTrack.Width = 250;
                    imgPlayingTrack.Height = 250;
                }
            }
        }

        #region playlist managing
        
        private string[] GetSongsAllInAlbum(int albumId) // orders all songs in album into a string[]
        {
            var trks = (trkView.GetTracksByAlbum(albumId.ToString())).ToList();         // get all tracks in given album
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].FileName + "," + trks[i].Artist + ",notShuffle";
            }
            return trkArray;
        }

        private string[] GetSongsInAlbumFromThis(int trackId, int albumId) // orders all songs in album into a string[]
        {
            List<string> tracks = new List<string>();
            var trks = trkView.GetTracksByAlbum(albumId.ToString());    // get all tracks in given album
            bool yes = false;
            foreach (var item in trks)                                  // then only take the selected song and all after it in the list
            {
                if (item.TrackId == trackId) { yes = true; }
                if(yes)
                {
                    tracks.Add(item.TrackId.ToString() + "," + item.FileName + "," + item.Artist + ",notShuffle");
                }
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
                trks[i] = tracks[i].TrackId.ToString() + "," + tracks[i].FileName + "," + tracks[i].Artist + ",notshuffle";
            }
            return trks;
        }

        #endregion

        #region Background messages

        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            Debug.WriteLine("message recieved loud and clear  ");
            TrackViewModel tr = new TrackViewModel();
            //string artist = "", title = "";
            int trackId = 0;
            string[] currentTrack = (e.Data.Values.FirstOrDefault().ToString()).Split(',');     // current track will be a comma seperated string with name, artist...
            if (currentTrack[0] != string.Empty && currentTrack[0] != "True")
            {
                if (currentTrack.Length > 1)
                {
                    trackId = Convert.ToInt32(currentTrack[0]);
                    tr = trkView.GetThisTrack(trackId);
                    //artist = currentTrack[1];
                    //title = currentTrack[2];
                    if (currentTrack[3].Contains("shuffle")) //  if its a track from random shuffle
                    {
                        trkView.AddRandomPlay(tr.TrackId);
                    }
                    else if (currentTrack[3].Contains("notShuffle")) 
                    {
                        trkView.AddPlay(tr.TrackId); // if it was chosen specifically
                    }
                    if (currentTrack.Count() > 4) //  if its got the extra word, its a skipped track
                    {
                        trkView.AddSkip(trackId);
                    }
                }
                else
                {
                    tr.Artist = "Radio :";
                    tr.Name = currentTrack[0];
                    //tbkSongName.Text = currentTrack[0];   // this is a radio station
                }
            }
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {                           
                            string pic = tr.ImageUri;
                            if (string.IsNullOrEmpty(pic)) { pic = "ms-appx:///Assets/radio672.png"; }
                            imgPlayingTrack.Source = new BitmapImage(new Uri(pic));    // the image for this song

                            tbkSongName.Text = tr.Artist + "-" + tr.Name;
                            SkippedTrackName = tr.Artist + "-" + tr.Name;
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        SererInitialized.Set();
                        break;
                    case Constants.PlayRadioFailed:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            tbkSongName.Text = "radio don't play good";
                        }
                        );                      
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
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                        isPlayRadio = false;
                    }
                    else if (isPlayGSTrack == true)
                    {
                        var message = new ValueSet();
                        message.Add(Constants.PlayGSTrack, orders);
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                        isPlayGSTrack = false;
                    }
                    else
                    {
                        var message = new ValueSet();
                        message.Add(Constants.StartPlayback, orders);
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                }
                else
                {
                    //throw new Exception("Background Audio Task didn't start in expected time");
                    Task.Run(delegate()
                    {
                        if (isPlayRadio == true)
                        {
                            var message = new ValueSet();
                            message.Add(Constants.PlayRadio, orders);
                            Debug.WriteLine("FG: sending Message");
                            BackgroundMediaPlayer.SendMessageToBackground(message);
                            isPlayRadio = false;
                        }
                        else if (isPlayGSTrack == true)
                        {
                            var message = new ValueSet();
                            message.Add(Constants.PlayGSTrack, orders);
                            Debug.WriteLine("FG: sending Message");
                            BackgroundMediaPlayer.SendMessageToBackground(message);
                            isPlayGSTrack = false;
                        }
                        else
                        {
                            var message = new ValueSet();
                            message.Add(Constants.StartPlayback, orders);
                            Debug.WriteLine("FG: sending Message");
                            BackgroundMediaPlayer.SendMessageToBackground(message);
                        }
                    });
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

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            AddMediaPlayerEventHandlers();
            Debug.WriteLine("load state");            
        }
       
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((App)Application.Current).isResumingFromTermination = false;
            this.navigationHelper.OnNavigatedFrom(e);
            RemoveMediaPlayerEventHandlers();
            Debug.WriteLine("in nav from");
        }

        #endregion
    }
}



        //private async void Log(string read)
        //{
        //    read = read + Environment.NewLine;
        //    try
        //    {
        //        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/Media/debugFile.txt"));
        //        await Windows.Storage.FileIO.AppendTextAsync(file, read);
        //        string text = await Windows.Storage.FileIO.ReadTextAsync(file);                
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.Message;
        //    }           
        //}




//private void StartBackgroundAudioTask()             // starts background---sends mesaage (tracks to play) from here
//        {
//            //AddMediaPlayerEventHandlers();
//            //Task.Run(delegate()
//            //{
//            //    if (isPlayRadio == true)
//            //    {
//            //        var message = new ValueSet();
//            //        message.Add(Constants.PlayRadio, orders);
//            //        Debug.WriteLine("FG: sending Message");
//            //        BackgroundMediaPlayer.SendMessageToBackground(message);
//            //        isPlayRadio = false;
//            //    }
//            //    else if (isPlayGSTrack == true)
//            //    {
//            //        var message = new ValueSet();
//            //        message.Add(Constants.PlayGSTrack, orders);
//            //        Debug.WriteLine("FG: sending Message");
//            //        BackgroundMediaPlayer.SendMessageToBackground(message);
//            //        isPlayGSTrack = false;
//            //    }
//            //    else
//            //    {
//            //        var message = new ValueSet();
//            //        message.Add(Constants.StartPlayback, orders);
//            //        Debug.WriteLine("FG: sending Message");
//            //        BackgroundMediaPlayer.SendMessageToBackground(message);
//            //    }
//            //});


//            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
//                bool result = SererInitialized.WaitOne(2000);
//                //bool result = true;
//                if (result == true)
//                {
//                    if (isPlayRadio == true)
//                    {
//                        var message = new ValueSet();
//                        message.Add(Constants.PlayRadio, orders);
//                        Debug.WriteLine("FG: sending Message");
//                        BackgroundMediaPlayer.SendMessageToBackground(message);
//                        isPlayRadio = false;
//                    }
//                    else if (isPlayGSTrack == true)
//                    {
//                        var message = new ValueSet();
//                        message.Add(Constants.PlayGSTrack, orders);
//                        Debug.WriteLine("FG: sending Message");
//                        BackgroundMediaPlayer.SendMessageToBackground(message);
//                        isPlayGSTrack = false;
//                    }
//                    else
//                    {
//                        var message = new ValueSet();
//                        message.Add(Constants.StartPlayback, orders);
//                        Debug.WriteLine("FG: sending Message");
//                        BackgroundMediaPlayer.SendMessageToBackground(message);
//                    }
//                }
//                else
//                {
//                    //throw new Exception("Background Audio Task didn't start in expected time");
//                    Task.Run(delegate()
//                    {
//                        if (isPlayRadio == true)
//                        {
//                            var message = new ValueSet();
//                            message.Add(Constants.PlayRadio, orders);
//                            Debug.WriteLine("FG: sending Message");
//                            BackgroundMediaPlayer.SendMessageToBackground(message);
//                            isPlayRadio = false;
//                        }
//                        else if (isPlayGSTrack == true)
//                        {
//                            var message = new ValueSet();
//                            message.Add(Constants.PlayGSTrack, orders);
//                            Debug.WriteLine("FG: sending Message");
//                            BackgroundMediaPlayer.SendMessageToBackground(message);
//                            isPlayGSTrack = false;
//                        }
//                        else
//                        {
//                            var message = new ValueSet();
//                            message.Add(Constants.StartPlayback, orders);
//                            Debug.WriteLine("FG: sending Message");
//                            BackgroundMediaPlayer.SendMessageToBackground(message);
//                        }
//                    });
//                }
//            }
//            );
//            backgroundtaskinitializationresult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
            
//        }
