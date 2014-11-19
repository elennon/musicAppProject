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
        /// <summary>
        /// Handler for state changed event of Media Player
        /// </summary>
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
            //mediaPlayer.SetUriSource(new Uri("https://www.youtube.com/v/LyUIJIdwEuA", UriKind.RelativeOrAbsolute));
            //mediaPlayer.SetUriSource(new Uri("http://r15---sn-q0c7dn76.googlevideo.com/videoplayback?expire=1416194185&ipbits=0&ratebypass=yes&signature=2B61D02694F5CC728F0AB130FAFB0D6FE469A13C.158461870758058EC51421030093F41F4D2A6540&source=youtube&initcwndbps=2301250&sver=3&fexp=907259,927622,931996,932404,936117,943909,945079,945243,947209,947215,948124,948703,948807,950502,952302,952605,952901,953912,957103,957105,957201&ms=au&mv=m&mt=1416172454&upn=0QkPqWGeBjo&itag=18&key=yt5&ip=89.100.62.181&mime=video/mp4&sparams=id,initcwndbps,ip,ipbits,itag,mime,mm,ms,mv,ratebypass,source,upn,expire&id=o-AJH7k4qDVCkLBnm2bphlu_KWOBnXft4m_fS88Xx4p_N7&mm=31", UriKind.RelativeOrAbsolute));           
            //this one works
            //mediaPlayer.SetUriSource(new Uri("http://r15---sn-q0c7dn76.googlevideo.com/videoplayback?sver=3&itag=18&mm=31&ip=89.100.62.181&ratebypass=yes&ipbits=0&id=o-ALkBFYUpLADum6xLlczVYS8PFKndRvGKpcERBbrTxdxr&ms=au&mv=m&mt=1416265931&key=yt5&mime=video/mp4&source=youtube&expire=1416287601&upn=xUwtXTcCwSY&sparams=id,initcwndbps,ip,ipbits,itag,mime,mm,ms,mv,ratebypass,source,upn,expire&signature=205FA8F68376BF1D1BEE18C418F3DFEBD66E2720.B530AA37D56D4DC796554834799B8E612AEC2ED3&fexp=902547,907259,927622,932404,943909,947209,947215,948124,952302,952605,952901,953912,957103,957105,957201&initcwndbps=2193750", UriKind.RelativeOrAbsolute));
        }

        public void PlayRadio(string rdoUrl)
        {
            mediaPlayer.AutoPlay = false;
            try
            {
                mediaPlayer.SetUriSource(new Uri(rdoUrl, UriKind.RelativeOrAbsolute));
                //mediaPlayer.SetUriSource(new Uri("http://r18---sn-q0c7dn7k.googlevideo.com/videoplayback?expire=1416193469&upn=1cSsZ2EZQPg&ipbits=0&fexp=900237,907259,927606,927622,932404,938698,943909,946603,947209,947215,948124,948703,951912,952302,952605,952901,953912,957103,957105,957201&key=yt5&mime=video/mp4&sparams=id,initcwndbps,ip,ipbits,itag,mime,mm,ms,mv,ratebypass,source,upn,expire&initcwndbps=2238750&source=youtube&sver=3&mm=31&id=o-AMULTJzKZLS9L74UtNizW6zm9Wvb91sBHPT17iKh_c_h&mv=m&ratebypass=yes&mt=1416171801&ms=au&itag=18&ip=89.100.62.181&signature=B4139B806E8523E2EA9B6D946FF1E57A7E459FA7.A6231A6681666F1BA9875275C3198FC0CE4B8EC1C1", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex) { string t = ex.Message; }
        }
        
        public async void StartTrackAt(string TrackName, [ReadOnlyArray()]string[] trks)
        {
            if (tracks.Count == 0) { tracks = await getFTracks(trks); }
            for (int i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].DisplayName == (TrackName))
                {
                    CurrentTrackId = i;
                    mediaPlayer.AutoPlay = false;
                    mediaPlayer.SetFileSource(tracks[i]);                 
                }
            }            
        }

        /// Starts a given track by finding its name and at desired position
        public async void StartTrackAt(string TrackName, TimeSpan position, [ReadOnlyArray()]string[] trks)
        {
            if (tracks.Count == 0) { tracks = await getFTracks(trks); }
            for (int i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].DisplayName == TrackName)
                {
                    CurrentTrackId = i;
                    break;
                }
            }

            mediaPlayer.AutoPlay = false;
            mediaPlayer.Volume = 0;
            startPosition = position;
            mediaPlayer.SetFileSource(tracks[CurrentTrackId]);
            //mediaPlayer.SetUriSource(new Uri(@"http:\/\/tinysong.com\/lJGA"));
        }

        public async void PlayAllTracks([ReadOnlyArray()]string[] trks)
        {            
            tracks = await getFTracks(trks);
            StartTrackAt(0);
        }

        public void SkipToNext()
        {
            StartTrackAt((CurrentTrackId + 1));// % tracks.Count);
        }

        /// <summary>
        /// Skip to next track
        /// </summary>
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

