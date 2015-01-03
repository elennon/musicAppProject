using MyMusic.Common;
using MyMusic.HelperClasses;
using MyMusic.Models;
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
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MyMusic
{
    sealed partial class App : Application
    {
        public static string DBPath = string.Empty;
        private TransitionCollection transitions;
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
    
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }        

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
           // Logger.GetLogger().InitiateLogger();
          //  Logger.GetLogger().Deletefile();

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
                    isResumingFromTermination = true;
                    DBPath = Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
                    using (var db = new SQLite.SQLiteConnection(DBPath))
                    {
                        db.CreateTable<Track>();
                        db.CreateTable<Album>();
                        db.CreateTable<Artist>();
                        db.CreateTable<Genre>();
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
                    db.CreateTable<Track>();
                    db.CreateTable<Album>();
                    db.CreateTable<Artist>();
                    db.CreateTable<Genre>();
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

                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
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
     //       Logger.GetLogger().logChannel.LogMessage("Unhandled exception: " + " Type:" +  sender.GetType() + "  Message: " + e.Message + "  exception: "+ e.Exception );
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            ValueSet messageDictionary = new ValueSet();
            messageDictionary.Add(Constants.AppSuspended, DateTime.Now.ToString());
            BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppSuspended);
            deferral.Complete();
        }
    }
}