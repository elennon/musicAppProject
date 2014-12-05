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
using MyMusic;

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

        #region Private 

        private SystemMediaTransportControls systemmediatransportcontrol;
        private MyPlaylistManager playlistManager;
        private BackgroundTaskDeferral deferral; // Used to keep task alive
        private ForegroundAppStatus foregroundAppState = ForegroundAppStatus.Unknown; 
        private AutoResetEvent BackgroundTaskStarted = new AutoResetEvent(false);
        private bool backgroundtaskrunning = false, Skipped = false;
        private List<int> trks = new List<int>();
        private string[] trksToPlay;
        //private string radioUrl = "";
        
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

        #region IBackgroundTask 
        
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
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
      
        void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("MyBackgroundAudioTask " + sender.TaskId + " Completed...");
            deferral.Complete();
        }

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

        #region UVC functions 
        
        private void UpdateUVCOnNewTrack()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
            systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
            systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = Playlist.CurrentTrackName;
            systemmediatransportcontrol.DisplayUpdater.Update();
        }
       
        private void systemmediatransportcontrol_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {           
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:                     
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
                    SkipToNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:                    
                    SkipToPrevious();
                    break;
            }
        }
       
        #endregion

        #region Playlist management 
        
        private void StartPlayback(string[] trks)
        {
            try
            {
                if (Playlist.CurrentTrackName.Contains("http"))
                {
                    Playlist.PlayAllTracks(trks);
                }
                else
                {
                    Playlist.PlayAllTracks(trks); //start playback                      
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        
        private void playRadio(string rdoUrl)
        {
            Playlist.PlayRadio(rdoUrl);
        }

        void playList_TrackChanged(MyPlaylist sender, object args)
        {
            UpdateUVCOnNewTrack();
            ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, sender.CurrentTrackName);
            //ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrackImg, pic);
            string currentTrack = "";
            if (Skipped)
            {
                currentTrack = trksToPlay[sender.CurrentTrackNumber] + ",skipped"; // skipped true so add skipped so the foreground knows
            }
            else { currentTrack = trksToPlay[sender.CurrentTrackNumber]; }

            if (foregroundAppState == ForegroundAppStatus.Active)
            {
                ValueSet message = new ValueSet();
                message.Add(Constants.Trackchanged, currentTrack);
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
            Skipped = false;
        }

        private void SkipToPrevious()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Changing;
            Playlist.SkipToPrevious();
        }

        private void SkipToNext()
        {
            systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Changing;
            Skipped = true;
            Playlist.SkipToNext();
        }

        #endregion

        void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            string currentImge = "";
            foreach (var item in e.Data.Values)
            {
                if (item.GetType() == typeof(string[]))
                {
                    trksToPlay = (string[])item;
                }
                else if (item.GetType() == typeof(string))
                {
                    currentImge = (string)item;
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
                    case Constants.PlayRadio:
                        playRadio(trksToPlay[0]);
                        break;
                    case Constants.CurrentTrackImg:
                        SaveCurrentImage(currentImge);
                        break;
                }
            }
        }
       
        private void SaveCurrentImage(string picc)
        {
            ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrackImg, picc);
        }
    }
}
