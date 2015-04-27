using BackgroundTask.RadioStreaming;
using MyMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
//using Windows.Web.Http;
using System.Net.Http;
using Windows.System.Threading;
using Newtonsoft.Json;
using BackgroundTask.Model;

namespace BackgroundTask
{
    public sealed class PlaylistManager
    {
        private static Playlist instance;

        public Playlist Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new Playlist();
                }
                return instance;
            }
        }

        public void ClearPlaylist()
        {
            instance = null;
        }
    }

    public sealed class Playlist
    {
        #region Private 

        private RadioStream rdStream { get; set; }
        private IRandomAccessStream mssStream;
        private Windows.Media.Core.MediaStreamSource MSS = null;
        private IInputStream inputStream;
    
        private UInt64 byteOffset;
        private TimeSpan timeOffset, sampleDuration = new TimeSpan(0, 0, 0, 0, 70);    
        private uint sampleSize = 800;
  
        private List<StorageFile> tracks = new List<StorageFile>();
        private string[] playTracks;       
        int CurrentTrackId = -1;
        private MediaPlayer mediaPlayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);

        private static string DBPath = string.Empty;
        private static readonly string _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        private bool onRadioChange = false;
       
        private ThreadPoolTimer timer;
        private HttpClient client;// = new HttpClient();
        private Track PlayingTrack = null;

        #endregion

        #region Public properties

        public PlayMode playMode { get; set; }

        public string CurrentTrackName
        {
            get
            {
                string tName = "";
                if (CurrentTrackId == -1)
                {
                    return String.Empty;
                }
                if (playMode == PlayMode.Collection || playMode == PlayMode.Streams )
                {
                    if (CurrentTrackId < playTracks.Length)
                    {
                        tName = playTracks[CurrentTrackId].Split(',')[1];

                        return tName;
                    }
                    else
                        throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
                }
                else if (playMode == PlayMode.Radio)
                {
                    tName = "Radio";
                    return tName;
                }
                else return "UnKnown";
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
                if (CurrentTrackId < playTracks.Length)
                {
                    return CurrentTrackId;
                }
                else
                    throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
            }
        }

        public event TypedEventHandler<Playlist, object> TrackChanged;

        #endregion

        internal Playlist()
        {
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
            mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
            //timer = new DispatcherTimer();
            //timer.Tick += timer_Tick;
            //timer.Interval = new TimeSpan(0, 0, 30);
        }
         
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
            TrackChanged.Invoke(this, CurrentTrackName);
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("PM: media ended" );

            if (playMode == PlayMode.Collection)
            {
                if (CurrentTrackId >= playTracks.Length - 1)
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
            else if (playMode == PlayMode.Streams )
            {
                string content = string.Format("MarkFinished,{0},{1},{2},{3}",PlayingTrack.GSSongId, PlayingTrack.GSSongKey, 
                                        PlayingTrack.GSServerId, PlayingTrack.GSSessionKey);
                MarkFinished(content);
                if (CurrentTrackId >= playTracks.Length - 1)
                {
                    //tracks.Clear();
                    CurrentTrackId = -1;
                    return;
                }
                else
                {
                    SkipToNextGS();
                }
            }
            else if (playMode == PlayMode.Radio)
            {
                if (rdStream != null)
                {
                    rdStream.CloseMedia();
                    sendNoPlayMessage();
                    rdStream = null;
                }
                onRadioChange = false;
            }
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("media player failed with error code " + args.ExtendedErrorCode.ToString());
        }

        #endregion

        #region Playlist 

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

                for (int i = 0; i < str.Length; i++)
                {
                    int num = Convert.ToInt32((str[i].Split(','))[0]) - 1;    // pulls out the track number to use as an index # for music library

                    sf.Add(lf[num]);
                }
            }
            catch (Exception egg)
            {
                string problem = egg.Message;
            }
            return sf;
        }

        private async void StartTrackAt(int id)
        {
            //lookHere();
            StorageFolder folder = KnownFolders.MusicLibrary;
            string jj = playTracks[id].Split(',')[1];
            StorageFile lf = await folder.GetFileAsync(jj);   // pass filename to get file
            CurrentTrackId = id;
            mediaPlayer.AutoPlay = false;
            //mediaPlayer.SetFileSource(tracks[id]);
            mediaPlayer.SetFileSource(lf);
        }

        public void PlayAllTracks([ReadOnlyArray()]string[] trks)
        {
            //tracks = await getFTracks(trks);
            playTracks = trks;
            StartTrackAt(0);
        }

        public void SkipToNext()
        {
            if (CurrentTrackId < playTracks.Length - 1)
            {
                if (playMode == PlayMode.Streams)
                {
                    StartGSTrackAt((CurrentTrackId + 1));
                }
                else
                { StartTrackAt((CurrentTrackId + 1)); }
            }
            else
            {
                CurrentTrackId = 0;             // if we are at the end of the list, play again from the start
                if (playMode == PlayMode.Streams)
                {
                    StartGSTrackAt(CurrentTrackId);
                }
                else
                { StartTrackAt(CurrentTrackId); }
            }
        }

        public void SkipToNextGS()
        {
            if (CurrentTrackId < playTracks.Length - 1)
            {
                StartGSTrackAt((CurrentTrackId + 1));
            }
            else
            {
                CurrentTrackId = 0;             // if we are at the end of the list, play again from the start
                StartGSTrackAt(CurrentTrackId);
            }
        }

        //void lookInHere()
        //{
        //    string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
        //    string nme = "";
        //    using (var db = new SQLiteConnection(dbPath))
        //    {
        //        db.RunInTransaction(() =>
        //        {
        //            var tr = db.Table<Track>().FirstOrDefault();
        //            nme = tr.Name;
        //        });
        //    }
        //}
       
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

        #region Streaming Radio and GS

        private async void StartGSTrackAt(int no)
        {
            var jj = playTracks[no].Split(',');
            string j = jj[0];
            PlayingTrack = new Track();
            PlayingTrack = await _GetGrooveSharkTrackUrl(jj[0], jj[1], jj[2]);  // 1;artist, 2;track, 3;sessionId
           
            CurrentTrackId = no;
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetUriSource(new Uri(PlayingTrack.GSSongKeyUrl));

            timer = ThreadPoolTimer.CreatePeriodicTimer(Mark30Handler, TimeSpan.FromSeconds(30));   // GrooveShark requirment to mark track as played > 30 seconds
        }

        public void PlayAllGSTracks([ReadOnlyArray()]string[] trks)
        {
            playTracks = trks;
            StartGSTrackAt(0);
        }

        private void Mark30Handler(ThreadPoolTimer timer)
        {
            string content = string.Format("Mark30,{0},{1},{2}", PlayingTrack.GSSongKey, PlayingTrack.GSServerId, PlayingTrack.GSSessionKey);
            Mark30(content);
        }
 
        public async void Mark30(string param)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var values = new Dictionary<string, string>
                {
                   { "value", param }                  
                };

                var content = new FormUrlEncodedContent(values);
                string url = string.Format("api/grooveshark");
                var resp = await client.PostAsync(url, content);
                var b = resp.IsSuccessStatusCode;
                timer.Cancel();
            }
            catch (HttpRequestException ex)
            {
                return;
            }
        }           // have to mark track played > 30 secs for GS

        public async void MarkFinished(string param)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var values = new Dictionary<string, string>
                {
                   { "value", param }                  
                };
                var content = new FormUrlEncodedContent(values);
                string url = string.Format("api/grooveshark");
                var resp = await client.PostAsync(url, content);
                var b = resp.IsSuccessStatusCode;               
            }
            catch (HttpRequestException ex)
            {
                return;
            }
        }       // have to mark track fully played for GS

        private IAsyncOperation<Track> _GetGrooveSharkTrackUrl(string artist, string track, string sessionId)
        {
            return GetGrooveSharkTrackUrl(artist, track, sessionId).AsAsyncOperation();
        }

        private async Task<Track> GetGrooveSharkTrackUrl(string artist, string track, string sessionId)
        {
            Track tr = new Track();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string url = string.Format("api/grooveshark?artist={0}&track={1}&sessionId={2}", artist, track, sessionId);
                string resp = await client.GetStringAsync(url);
                tr = JsonConvert.DeserializeObject<Track>(resp);
                tr.GSSessionKey = sessionId;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            return tr;
        }

        public void PlayRadio(string rdoUrl)
        {
            if (rdStream != null) { onRadioChange = true; }
            mediaPlayer.AutoPlay = false;
            rdStream = new RadioStream(rdoUrl);
            
            rdStream.setUpped += async (obj, e) =>
            {
                if(obj != null)
                {
                    var mp = (MediaEncodingProfile)obj;
                    await Task.Delay(TimeSpan.FromSeconds(6));
                    InitializeMediaStreamSource(mp);
                }
                else
                { sendNoPlayMessage(); }
            };
            
        }

        private void sendNoPlayMessage()
        {
            ValueSet message = new ValueSet();
            message.Add(Constants.PlayRadioFailed, true);
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }
       
        #region MSS stuff

        public void InitializeMediaStreamSource(MediaEncodingProfile mp)
        {
            byteOffset = 0;
            timeOffset = new TimeSpan(0);
            try
            {
                AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(mp.Audio);
                MSS = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            MSS.CanSeek = true;
            MSS.MusicProperties.Title = "Online Radio";
            MSS.Duration = new TimeSpan(0);
            //MSS.BufferTime = new TimeSpan(0,0,5);
            MSS.Starting += MSS_Starting;               // MSS handlers
            MSS.SampleRequested += MSS_SampleRequested;
            MSS.Closed += MSS_Closed;
            mediaPlayer.SetMediaSource(MSS);
        }

        async void MSS_SampleRequested(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            if (mssStream == Stream.Null) return;
            MediaStreamSourceSampleRequest request = args.Request;

            long progress = (long)byteOffset + (long)sampleSize;

            while ((progress) >= (long)mssStream.Size) //holds things up if not enoughh buffer
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
            if (byteOffset + (ulong)sampleSize <= mssStream.Size)
            {
                MediaStreamSourceSampleRequestDeferral deferal = request.GetDeferral();
                mssStream = rdStream.bufferStream;
                inputStream = mssStream.GetInputStreamAt(byteOffset);

                MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, sampleSize, timeOffset);
                sample.Duration = sampleDuration;
                sample.KeyFrame = true;

                byteOffset += sampleSize;    // sampleSize;
                timeOffset = timeOffset.Add(sampleDuration);
                request.Sample = sample;

                deferal.Complete();
            }
        }

        void MSS_Starting(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            MediaStreamSourceStartingRequest request = args.Request;

            UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
            timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
            byteOffset = sampleOffset * sampleSize;

            if (mssStream == null)
            {
                MediaStreamSourceStartingRequestDeferral deferal = request.GetDeferral();
                try
                {
                    mssStream = rdStream.bufferStream;
                    request.SetActualStartPosition(timeOffset);
                    deferal.Complete();
                }
                catch (Exception)
                {
                    MSS.NotifyError(MediaStreamSourceErrorStatus.FailedToOpenFile);
                    deferal.Complete();
                }
            }
            else
            {
                request.SetActualStartPosition(timeOffset);
            }
        }

        void MSS_Closed(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceClosedEventArgs args)
        {
            Debug.WriteLine("BK: MSS closed. Reason: " + args.Request.Reason);
            if (mssStream != null && onRadioChange == false)
            {
                mssStream.Dispose();
                mssStream = null;
            }
           
            sender.SampleRequested -= MSS_SampleRequested;
            sender.Starting -= MSS_Starting;
            sender.Closed -= MSS_Closed;

            if (sender == MSS)
            {
                MSS = null;
            }
            onRadioChange = false;
        }

        #endregion


        #endregion
        

    }
}



