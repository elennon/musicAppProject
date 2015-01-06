using MyMusic;
using MyPlaylistManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;

namespace BackgroundTask
{
   enum ForegroundAppStatus
    {
        Active,
        Suspended,
        Unknown
    }

    public sealed class BackGroundMusicTask : IBackgroundTask
    {
        #region Private properties

        private SystemMediaTransportControls systemmediatransportcontrol;
        private MyPlaylistMgr playlistManager;
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
                    playlistManager = new MyPlaylistMgr();
                }
                return playlistManager.Current;
            }
        }

        #endregion

        #region main task methods 
        
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

            ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, "BKRunning");
            deferral = taskInstance.GetDeferral();
            Debug.WriteLine("BK-- run completed");
            
        }
      
        void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("MyBackgroundAudioTask " + sender.TaskId + " Completed...");
            //Log("BK-- in task completed");
            deferral.Complete();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("BackgroundMusicTask " + sender.Task.TaskId + " Cancel Requested...");
            //Log("BK-- in task cancelled");
            try
            {
                //save state
                ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, Playlist.CurrentTrackName);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.TrackIdNo, Convert.ToInt32(trksToPlay[Playlist.CurrentTrackNumber].Split(',')[0])); //[0] is the track id
                ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, "BKCancelled");
                ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Enum.GetName(typeof(ForegroundAppStatus), foregroundAppState));
                backgroundtaskrunning = false;
                systemmediatransportcontrol.ButtonPressed -= systemmediatransportcontrol_ButtonPressed;                
                Playlist.TrackChanged -= playList_TrackChanged;
  
                playlistManager.ClearPlaylist();
                playlistManager = null;
                BackgroundMediaPlayer.Shutdown(); // shutdown media pipeline
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            deferral.Complete(); // signals task completion. 
            Debug.WriteLine("BackgroundTask Cancel complete...");
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

        #region Playlists 
        
        private void StartPlayback(string[] trks)
        {
            try
            {              
                Playlist.PlayAllTracks(trks); //start playing                                    
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        
        private void playRadio(string rdoUrl)
        {
            try
            {
                Playlist.PlayRadio(rdoUrl);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void playGSTrack(string Url)
        {
            Playlist.PlayGSTrack(Url);
        }

        void playList_TrackChanged(MyPlaylist sender, object args)
        {
            Debug.WriteLine("track changed ");
            UpdateUVCOnNewTrack();
            ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, sender.CurrentTrackName);

            int trkId = Convert.ToInt32(trksToPlay[sender.CurrentTrackNumber].Split(',')[0]);   // pull out the track id from comma seperated string
            ApplicationSettingsHelper.SaveSettingsValue(Constants.TrackIdNo, trkId);            // save no. for app to get image

            string currentTrack = "";
            if (Skipped)
            {
                currentTrack = trksToPlay[sender.CurrentTrackNumber] + ",skipped"; // skipped true so add skipped so the foreground knows
            }
            else { currentTrack = trksToPlay[sender.CurrentTrackNumber]; }

            Debug.WriteLine("in trackChanged. fg is " + foregroundAppState);
            
            if (foregroundAppState == ForegroundAppStatus.Active)
            {
                Debug.WriteLine("foregroundApp is active still ");
                //Log("BK-- message sent");
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
            //Log.GetLog().Write("BK: message recieved from FG");
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
                        Debug.WriteLine("bk App suspending"); // App is suspended
                        foregroundAppState = ForegroundAppStatus.Suspended;
                        
                        break;
                    case Constants.AppResumed:
                        Debug.WriteLine("bk App resuming");        // App is back on
                        foregroundAppState = ForegroundAppStatus.Active;
                        ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, Playlist.CurrentTrackName);
                        int trkId = Convert.ToInt32(trksToPlay[Playlist.CurrentTrackNumber].Split(',')[0]); // pull out the track id from comma seperated string
                        ApplicationSettingsHelper.SaveSettingsValue(Constants.TrackIdNo, trkId ); // save no. for app to get image
                        
                        break;
                    case Constants.StartPlayback:            //start playlist
                        Debug.WriteLine("Starting Playback");
                        StartPlayback(trksToPlay);
                        break;
                    case Constants.SkipNext:            // User has pressed next
                        Debug.WriteLine("Skipping to next");
                        SkipToNext();
                        break;
                    case Constants.SkipPrevious:         // User has pressed back
                        Debug.WriteLine("Skipping to previous");
                        SkipToPrevious();
                        break;
                    case Constants.PlayRadio:       // radio selected
                        playRadio(trksToPlay[0]);
                        break;
                    case Constants.PlayGSTrack:       // radio selected
                        playGSTrack(trksToPlay[0]);
                        break; 
                }
            }
        }

        
    }
}