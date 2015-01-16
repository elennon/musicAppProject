using BackgroundTask.RadioStreaming;
using MyMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using Windows.Web.Http;

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
                if (playMode == PlayMode.Collection)
                {
                    if (CurrentTrackId < playTracks.Length)
                    {
                        tName = playTracks[CurrentTrackId].Split(',')[2];

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
            DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
        }

        //void lookHere()
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
       
        #region MediaPlayer Handlers

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
            else if (playMode == PlayMode.Radio)
            {
                if (rdStream != null)
                {
                    rdStream.CloseMedia();
                //    sendNoPlayMessage();
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

        #region Streaming

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

        public void PlayGSTrack(string gsUrl)
        {
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetUriSource(new Uri(gsUrl));
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



