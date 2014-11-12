using SampleBackgroundAudio.MyPlaylistManager;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;
using BackgroundAudioPlayerCS;
using Windows.Foundation.Collections;
using Windows.Storage;
using System.Collections.Generic;

namespace SampleBackgroundAudioTask
{
    
    enum ForegroundAppStatus
    {
        Active,
        Suspended,
        Unknown
    }

    public sealed class MyBackgroundAudioTask : IBackgroundTask
    {

        #region Private fields, properties
        private SystemMediaTransportControls systemmediatransportcontrol;
        private MyPlaylistManager playlistManager;
        private BackgroundTaskDeferral deferral; // Used to keep task alive
        private ForegroundAppStatus foregroundAppState = ForegroundAppStatus.Unknown; 
        private AutoResetEvent BackgroundTaskStarted = new AutoResetEvent(false);
        private bool backgroundtaskrunning = false;
        private List<int> trks = new List<int>();
        private string[] trksToPlay;// = "";//new string[1];
        
        private MyPlaylist Playlist
        {
            get
            {
                if (null == playlistManager)
                {
                    playlistManager = new MyPlaylistManager();
                }
                return playlistManager.Current;
            }
        }
        #endregion

        #region IBackgroundTask and IBackgroundTaskInstance Interface Members and handlers
        /// <summary>
        /// The Run method is the entry point of a background task. 
        /// </summary>
        /// <param name="taskInstance"></param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background Audio Task " + taskInstance.Task.Name + " starting...");
            // Initialize SMTC object to talk with UVC. 
            //Note that, this is intended to run after app is paused and 
            //hence all the logic must be written to run in background process
            systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
            systemmediatransportcontrol.PropertyChanged += systemmediatransportcontrol_PropertyChanged;
            systemmediatransportcontrol.IsEnabled = true;
            systemmediatransportcontrol.IsPauseEnabled = true;
            systemmediatransportcontrol.IsPlayEnabled = true;
            systemmediatransportcontrol.IsNextEnabled = true;
            systemmediatransportcontrol.IsPreviousEnabled = true;

            // Associate a cancellation and completed handlers with the background task.
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            taskInstance.Task.Completed += Taskcompleted;

            var value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.AppState);
            if (value == null)
                foregroundAppState = ForegroundAppStatus.Unknown;
            else
                foregroundAppState = (ForegroundAppStatus)Enum.Parse(typeof(ForegroundAppStatus), value.ToString());

            //Add handlers for MediaPlayer
            BackgroundMediaPlayer.Current.CurrentStateChanged += Current_CurrentStateChanged;

            //Add handlers for playlist trackchanged
            Playlist.TrackChanged += playList_TrackChanged;

            //Initialize message channel 
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
            
            //Send information to foreground that background task has been started if app is active
            if (foregroundAppState != ForegroundAppStatus.Suspended)
            {
                ValueSet message = new ValueSet();
                message.Add(Constants.BackgroundTaskStarted, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
            BackgroundTaskStarted.Set();
            backgroundtaskrunning = true;

            ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, Constants.BackgroundTaskRunning);
            deferral = taskInstance.GetDeferral();           
        }

        /// <summary>
        /// Indicate that the background task is completed.
        /// </summary>       
        void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("MyBackgroundAudioTask " + sender.TaskId + " Completed...");
            deferral.Complete();
        }

        /// <summary>
        /// Handles background task cancellation. Task cancellation happens due to :
        /// 1. Another Media app comes into foreground and starts playing music 
        /// 2. Resource pressure. Your task is consuming more CPU and memory than allowed.
        /// In either case, save state so that if foreground app resumes it can know where to start.
        /// </summary>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // You get some time here to save your state before process and resources are reclaimed
            Debug.WriteLine("MyBackgroundAudioTask " + sender.Task.TaskId + " Cancel Requested...");
            try
            {
                //save state
                ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, Playlist.CurrentTrackName);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.Position, BackgroundMediaPlayer.Current.Position.ToString());
                ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, Constants.BackgroundTaskCancelled);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Enum.GetName(typeof(ForegroundAppStatus), foregroundAppState));
                backgroundtaskrunning = false;
                //unsubscribe event handlers
                systemmediatransportcontrol.ButtonPressed -= systemmediatransportcontrol_ButtonPressed;
                systemmediatransportcontrol.PropertyChanged -= systemmediatransportcontrol_PropertyChanged;
                Playlist.TrackChanged -= playList_TrackChanged;
                
                //clear objects task cancellation can happen uninterrupted
                playlistManager.ClearPlaylist();
                playlistManager = null;
                BackgroundMediaPlayer.Shutdown(); // shutdown media pipeline
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            deferral.Complete(); // signals task completion. 
            Debug.WriteLine("MyBackgroundAudioTask Cancel complete...");
        }
        #endregion

        #region SysteMediaTransportControls related functions and handlers
        /// <summary>
        /// Update UVC using SystemMediaTransPortControl apis
        /// </summary>
        private void UpdateUVCOnNewTrack()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
            systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
            systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = Playlist.CurrentTrackName;
            systemmediatransportcontrol.DisplayUpdater.Update();
        }

        /// <summary>
        /// Fires when any SystemMediaTransportControl property is changed by system or user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void systemmediatransportcontrol_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            //TODO: If soundlevel turns to muted, app can choose to pause the music
        }

        /// <summary>
        /// This function controls the button events from UVC.
        /// This code if not run in background process, will not be able to handle button pressed events when app is suspended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void systemmediatransportcontrol_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {           
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play: 
                    Debug.WriteLine("UVC play button pressed");
                    // If music is in paused state, for a period of more than 5 minutes, 
                    //app will get task cancellation and it cannot run code. 
                    //However, user can still play music by pressing play via UVC unless a new app comes in clears UVC.
                    //When this happens, the task gets re-initialized and that is asynchronous and hence the wait
                    if (!backgroundtaskrunning)
                    {
                        bool result = BackgroundTaskStarted.WaitOne(2000);
                        if (!result)
                            throw new Exception("Background Task didnt initialize in time");
                    }
                    StartPlayback(trksToPlay);
                    break;
                case SystemMediaTransportControlsButton.Pause: 
                    Debug.WriteLine("UVC pause button pressed");
                    try
                    {
                        BackgroundMediaPlayer.Current.Pause();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    break;
                case SystemMediaTransportControlsButton.Next: 
                    Debug.WriteLine("UVC next button pressed");
                    SkipToNext();
                    break;
                case SystemMediaTransportControlsButton.Previous: 
                    Debug.WriteLine("UVC previous button pressed");
                    SkipToPrevious();
                    break;
            }
        }

        

        #endregion

        #region Playlist management functions and handlers
        
        private void StartPlayback(string[] trks)
        {
            try
            {
                if (Playlist.CurrentTrackName == string.Empty)
                {
                    //If the task was cancelled we would have saved the current track and its position. We will try playback from there
                    var currenttrackname = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                    var currenttrackposition = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.Position);
                    if (currenttrackname != null)
                    {

                        if (currenttrackposition == null)
                        {
                            // play from start if we dont have position
                            Playlist.StartTrackAt((string)currenttrackname, trks);
                        }
                        else
                        {
                            // play from exact position otherwise
                            Playlist.StartTrackAt((string)currenttrackname, TimeSpan.Parse((string)currenttrackposition), trks);
                        }
                    }
                    else
                    {
                        //If we dont have anything, play from beginning of playlist.
                        Playlist.PlayAllTracks(trks); //start playback
                    }
                }
                else
                {
                    BackgroundMediaPlayer.Current.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        
        void playList_TrackChanged(MyPlaylist sender, object args)
        {
            UpdateUVCOnNewTrack();
            ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, sender.CurrentTrackName);

            string currentTrack = trksToPlay[sender.CurrentTrackNumber];

            if (foregroundAppState == ForegroundAppStatus.Active)
            {
                //Message channel that can be used to send messages to foreground
                ValueSet message = new ValueSet();
                message.Add(Constants.Trackchanged, currentTrack);
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
        }

        private void SkipToPrevious()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Changing;
            Playlist.SkipToPrevious();
        }

        private void SkipToNext()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Changing;
            Playlist.SkipToNext();
        }

        #endregion

        #region Background Media Player Handlers
        void Current_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }

        void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {            
            foreach (var item in e.Data.Values)
            {
                if (item.GetType() == typeof(string[]))
                {
                    trksToPlay = (string[])item;
                }
            }
            foreach (string key in e.Data.Keys)
            {
                
                switch (key.ToLower())
                {
                    case Constants.AppSuspended:
                        Debug.WriteLine("App suspending"); // App is suspended, you can save your task state at this point
                        foregroundAppState = ForegroundAppStatus.Suspended;
                        ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, Playlist.CurrentTrackName);
                        break;
                    case Constants.AppResumed:
                        Debug.WriteLine("App resuming"); // App is resumed, now subscribe to message channel
                        foregroundAppState = ForegroundAppStatus.Active;
                        break;
                    case Constants.StartPlayback: //Foreground App process has signalled that it is ready for playback
                        Debug.WriteLine("Starting Playback");
                        StartPlayback(trksToPlay);
                        break;
                    case Constants.SkipNext: // User has chosen to skip track from app context.
                        Debug.WriteLine("Skipping to next");
                        SkipToNext();
                        break;
                    case Constants.SkipPrevious: // User has chosen to skip track from app context.
                        Debug.WriteLine("Skipping to previous");
                        SkipToPrevious();
                        break;
                }
            }
        }
        #endregion

    }
}