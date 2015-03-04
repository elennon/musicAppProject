
using Microsoft.Practices.Unity;
using MyMusic.Common;
using MyMusic.HelperClasses;
using MyMusic.Models;
using MyMusic.Utilities;
using MyMusic.ViewModels;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MyMusic
{
    sealed partial class App : Application
    {
        public static string DBPath = string.Empty;
        private TransitionCollection transitions;

        private TracksViewModel trkView = new TracksViewModel();
        
        private BKPlayer bkPlayer;
        public BKPlayer BkPlayer
        {
            get { return bkPlayer; }
            set { bkPlayer = value; }
        }
        
        private bool _isResuming = false;
        public bool isResumingFromTermination
        {
            get
            {
                return _isResuming;
            }
            set
            {
                _isResuming = value;
            }
        }

        private bool isMyBackgroundTaskRunning = false;
        public bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (isMyBackgroundTaskRunning)
                    return true;

                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.IsBackgroundActive);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    isMyBackgroundTaskRunning = (bool)value;  // ((String)value).Equals(Constants.BackgroundTaskRunning);
                    return isMyBackgroundTaskRunning;
                }
            }
        }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
            this.Resuming += App_Resuming;
            this.BkPlayer = new BKPlayer();
        }

        void App_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);
            BkPlayer.AddMediaPlayerEventHandlers();
            Debug.WriteLine("in FG Current_Resuming");
            //     Logger.GetLogger().logChannel.LogMessage("In FG Current_Resuming");            
        }        

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            bool test = IsMyBackgroundTaskRunning;
            //if (Log.CheckIfNull() == true)
            //{
            //    Log.GetLog().InitiateLog();
            //}
            //if (Logger.CheckIfNull() == true)
            //{
            //    Logger.GetLogger().InitiateLogger();
            //    Logger.GetLogger().Deletefile();
            //}

            //InitializeIocBindings();
            Frame rootFrame = Window.Current.Content as Frame;
           
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);     // set to active to let background task know to send messages

                rootFrame.CacheSize = 1;
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    Debug.WriteLine("app class after termination");
                    //Logger.GetLogger().logChannel.LogMessage("FG: app class after termination");

                    isResumingFromTermination = true;
                    if (IsMyBackgroundTaskRunning) { BkPlayer.AddMediaPlayerEventHandlers(); }

                    DBPath = Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
                    using (var db = new SQLite.SQLiteConnection(DBPath))
                    {
                        //db.DropTable<PlaylistTracks>();
                        db.CreateTable<Track>();
                        db.CreateTable<Album>();
                        db.CreateTable<Artist>();
                        db.CreateTable<Genre>();
                        db.CreateTable<Playlist>();
                        db.CreateTable<PlaylistTracks>();
                        db.CreateTable<RadioStream>();
                        db.CreateTable<RadioGenre>();
                    }                   
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException ex)
                    {
                        string error = ex.Message;
                    }                        
                }

                DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
                              
                using (var db = new SQLite.SQLiteConnection(DBPath))
                {
                    //db.DropTable<PlaylistTracks>();
                    db.CreateTable<Track>();
                    db.CreateTable<Album>();
                    db.CreateTable<Artist>();
                    db.CreateTable<Genre>();
                    db.CreateTable<Playlist>();
                    db.CreateTable<PlaylistTracks>();
                    db.CreateTable<RadioStream>();
                    db.CreateTable<RadioGenre>();
                }
                //rootFrame.CacheSize = 1;
                //ResetData();

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                if (!rootFrame.Navigate(typeof(Splash), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }
        
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
       
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Logger.GetLogger().logChannel.LogMessage("Unhandled exception: " + " Type:" +  sender.GetType() + "  Message: " + e.Message + "  exception: "+ e.Exception );
            //var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
            //var filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            //var logSave = Logger.GetLogger().logSession.SaveToFileAsync(folder, filename).AsTask();
            //logSave.Wait();
            //Log.GetLog().Write("Unhandled exception: " + " Type:" + sender.GetType() + "  Message: " + e.Message + "  exception: " + e.Exception);
            //Log.GetLog().SaveLogFile();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            Debug.WriteLine("FG: In on suspending");
            //Logger.GetLogger().logChannel.LogMessage("FG: In on suspending");
            //BkPlayer.RemoveMediaPlayerEventHandlers();
            await SuspensionManager.SaveAsync();
            ValueSet messageDictionary = new ValueSet();
            messageDictionary.Add(Constants.AppSuspended, DateTime.Now.ToString());
            BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppSuspended);
            deferral.Complete();
        }
    }
}



//private void InitializeIocBindings()
//        {
//            /// Register ViewModel interfaces and types (as singletons)

//            IocContainer.Container

//                .RegisterType(
//                    typeof(IMainViewModel),
//                    typeof(MainViewModel),
//                    null,
//                    new ContainerControlledLifetimeManager())
//                .RegisterType(
//                    typeof(IRadioViewModel),
//                    typeof(RadioViewModel),
//                    null,
//                    new ContainerControlledLifetimeManager());
//        }


//void App_Resuming(object sender, object e)
//        {
//            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);
//            BkPlayer.AddMediaPlayerEventHandlers();
//            Debug.WriteLine("in fg Current_Resuming");
//            //     Logger.GetLogger().logChannel.LogMessage("In FG Current_Resuming");

//            if (IsMyBackgroundTaskRunning)
//            {
//                ValueSet messageDictionary = new ValueSet();
//                messageDictionary.Add(Constants.AppResumed, DateTime.Now.ToString());
//                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);

//                string pic = "";
//                object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
//                var tName = np.FindName("tbkSongName") as TextBlock;
//                if (value1 == null)
//                {
//                    tName.Text = "current Track null ( in resuming )";
//                }
//                if (value1 != null)
//                {
//                    tName.Text = (string)value1 + "( in resuming )";
//                }

//                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
//                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
//                else
//                {
//                    int trackId = (int)value2;
//                    pic = (trkView.GetThisTrack(trackId)).ImageUri;
//                    if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
//                    var npImg = np.FindName("imgPlayingTrack") as Image;
//                    npImg.Source = new BitmapImage(new Uri(pic));
//                }
//            }
//        }        