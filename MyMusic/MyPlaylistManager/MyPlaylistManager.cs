using SampleBackgroundAudio.MyPlaylistManager.ShoutCast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Core;
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

        private RadioStream rdStream;
        private IRandomAccessStream mssStream;
        private Windows.Media.Core.MediaStreamSource MSS = null;

        private IInputStream inputStream;
        private UInt64 byteOffset;
        private TimeSpan timeOffset;
        private TimeSpan songDuration;
        private TimeSpan sampleDuration = new TimeSpan(0, 0, 0, 0, 70);

        private uint samplePerSec = 0, channels = 0, agvBytes = 0, sampleSize = 0;
        private radioItems ri = new radioItems();

        private List<StorageFile> tracks = new List<StorageFile>();        
        int CurrentTrackId = -1;
        private MediaPlayer mediaPlayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);

        internal MyPlaylist()
        {                      
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
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

        #region Public properties

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
        
        void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {            
            sender.Play();
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
                radioItems _ri = new radioItems
                {
                    agvBytes = agvBytes,
                    channels = channels,
                    samplePerSec = samplePerSec,
                    sampleSize = sampleSize,
                    Uri = rdoUrl
                };
                rdStream = new RadioStream(_ri);
                
                rdStream.setUpped += async (obj, e) =>
                {
                    ri = (radioItems)obj;
                    await Task.Delay(TimeSpan.FromSeconds(6));
                    InitializeMediaStreamSource();                   
                };                          
            }
            catch (Exception ex) { string t = ex.Message; }
        }

        #region MSS stuff

        public void InitializeMediaStreamSource()
        {
            // initialize Parsing Variables
            byteOffset = 0;
            timeOffset = new TimeSpan(0);

            // get the encoding properties of the input MP3 file

            List<string> encodingPropertiesToRetrieve = new List<string>();

            encodingPropertiesToRetrieve.Add("System.Audio.SampleRate");
            encodingPropertiesToRetrieve.Add("System.Audio.ChannelCount");
            encodingPropertiesToRetrieve.Add("System.Audio.EncodingBitrate");

            AudioEncodingProperties audioProps = AudioEncodingProperties.CreateMp3(ri.samplePerSec, ri.channels, ri.agvBytes);

            AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(audioProps);

            // creating the MediaStreamSource for the MP3 file
            MSS = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
            MSS.CanSeek = true;
            MSS.MusicProperties.Title = "Title";
            MSS.Duration = new TimeSpan(0, 0, 6, 0);

            // hooking up the MediaStreamSource event handlers
            MSS.Starting += MSS_Starting;
            MSS.SampleRequested += MSS_SampleRequested;
            MSS.Closed += MSS_Closed;
            mediaPlayer.SetMediaSource(MSS);
        }

        async void MSS_SampleRequested(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
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

                //inputStream = (bufferStream.AsRandomAccessStream()).GetInputStreamAt(byteOffset);
                // create the MediaStreamSample and assign to the request object. 
                // You could also create the MediaStreamSample using createFromBuffer(...)

                MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, ri.sampleSize, timeOffset);
                sample.Duration = sampleDuration;
                sample.KeyFrame = true;

                // increment the time and byte offset

                byteOffset += ri.sampleSize;    // sampleSize;
                timeOffset = timeOffset.Add(sampleDuration);
                request.Sample = sample;

                deferal.Complete();
            }

        }

        void MSS_Starting(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            //await Task.Delay(TimeSpan.FromSeconds(10));
            // sampleSize = (uint)mp3Frame.FrameSize;
            MediaStreamSourceStartingRequest request = args.Request;
            if ((request.StartPosition != null) && request.StartPosition.Value <= MSS.Duration)
            {
                UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
                timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
                byteOffset = sampleOffset * ri.sampleSize;
            }

            // create the RandomAccessStream for the input file for the first time 

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
            if (mssStream != null)
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
        }

        #endregion
               
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

