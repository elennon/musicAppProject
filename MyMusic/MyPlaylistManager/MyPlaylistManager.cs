using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace SampleBackgroundAudio.MyPlaylistManager
{
   
    public sealed class MyPlaylistManager
    {
        #region Private members
        private static MyPlaylist instance; 
        #endregion

        #region Playlist management methods/properties
        public MyPlaylist Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyPlaylist();
                }
                return instance;
            }
        }

        public void ClearPlaylist()
        {
            instance = null;
        } 
        #endregion
    }

    public sealed class MyPlaylist  
    {
        #region Private members

        private List<StorageFile> tracks = new List<StorageFile>();
        
        int CurrentTrackId = -1;
        private MediaPlayer mediaPlayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);

        internal MyPlaylist()
        {                      
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
        }

        private IAsyncOperation<List<StorageFile>> getFTracks(string[] trks)
        {
            return this.getTracks(trks).AsAsyncOperation();
        }

        private async Task<List<StorageFile>> getTracks(string[] str)
        {
            List<StorageFile> sf = new List<StorageFile>();
            try
            {               
                StorageFolder folder = KnownFolders.MusicLibrary;
                IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                // List<int> trks = new List<int>();

                for (int i = 0; i < str.Length; i++)
                {
                    int num = Convert.ToInt32((str[i].Split(','))[0])-1;    // pulls out the track number to use as an index # for music library
                    sf.Add(lf[num]);
                }
            }
            catch(Exception egg)
            {
                string problem = egg.Message;
            }
            return sf;
        } 

        #endregion

        #region Public properties, events and handlers

        public string CurrentTrackName
        {
            get
            {
                if (CurrentTrackId == -1)
                {
                    return String.Empty;
                }
                if (CurrentTrackId < tracks.Count)
                {
                    string fullUrl = tracks[CurrentTrackId].DisplayName;
                    
                    return fullUrl;
                }
                else
                    throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
            }
        }

        public int CurrentTrackNumber
        {
            get
            {
                if (CurrentTrackId == -1)
                {
                    return 0;
                }
                if (CurrentTrackId < tracks.Count)
                {
                    return CurrentTrackId;
                }
                else
                    throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
            }
        }

        public event TypedEventHandler<MyPlaylist, object> TrackChanged;
        #endregion

        #region MediaPlayer Handlers
        
        void mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing && startPosition != TimeSpan.FromSeconds(0))
            {
                // if the start position is other than 0, then set it now
                sender.Position = startPosition;
                sender.Volume = 1.0;
                startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {            
            sender.Play();
            Debug.WriteLine("New Track" + this.CurrentTrackName);
            TrackChanged.Invoke(this, CurrentTrackName);
        }
        
        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            if (CurrentTrackId >= tracks.Count -1)
            {
                //tracks.Clear();
                CurrentTrackId = -1;
                return;
            }
            else
            {
                SkipToNext(); 
            }                    
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("Failed with error code " + args.ExtendedErrorCode.ToString());
        }
        #endregion

        #region Playlist command handlers
        
        private void StartTrackAt(int id)
        {          
            CurrentTrackId = id;            
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetFileSource(tracks[id]);
        }

        public void PlayRadio(string rdoUrl)
        {
            mediaPlayer.AutoPlay = false;
            try
            {
                mediaPlayer.SetUriSource(new Uri(rdoUrl, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex) { string t = ex.Message; }
        }
               
        public async void PlayAllTracks([ReadOnlyArray()]string[] trks)
        {            
            tracks = await getFTracks(trks);
            StartTrackAt(0);
        }

        public void SkipToNext()
        {
            if(CurrentTrackId < tracks.Count -1)    
            {
                StartTrackAt((CurrentTrackId + 1));
            }   
            else
            {
                CurrentTrackId = 0;             // if we are at the end of the list, play again from the start
                StartTrackAt(CurrentTrackId);
            }
        }
     
        public void SkipToPrevious()
        {
            if (CurrentTrackId == 0)
            {
                StartTrackAt(CurrentTrackId);
            }
            else
            {
                StartTrackAt(CurrentTrackId - 1);
            }
        }
        
        #endregion
        
    }

}

