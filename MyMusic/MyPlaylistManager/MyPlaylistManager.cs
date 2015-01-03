

using MyMusic;
using MyPlaylistManager.Models;
using MyPlaylistManager.ShoutCast;
using SQLiteBase;
//using SQLiteBase;
//using SQLiteWinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace MyPlaylistManager
{
   
    public sealed class MyPlaylistMgr
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

        private MyPlaylistManager.ShoutCast.RadioStream rdStream;
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
        private string[] playTracks;
        int CurrentTrackId = -1;
        private MediaPlayer mediaPlayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);
        private static string DBPath = string.Empty;

        internal MyPlaylist()
        {                      
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
            DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");         
        }
        private static readonly string _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        
        void lookHere()
        {
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tracks.s3db");
            string nme = "";
            using (var db = new SQLiteConnection(dbPath))
            {
                db.RunInTransaction(() =>
                {
                    var tr = db.Table<Track>().FirstOrDefault();
                    nme = tr.Name;
                });
            }       
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
                if (CurrentTrackId < playTracks.Length)
                {
                    string fullUrl = playTracks[CurrentTrackId].Split(',')[2];

                    return fullUrl;
                }
                else
                    throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
            }
        }
        //public string CurrentSongId
        //{
        //    get
        //    {
        //        if (CurrentTrackId == -1)
        //        {
        //            return String.Empty;
        //        }
        //        if (CurrentTrackId < playTracks.Length)
        //        {
        //            string trackId = playTracks[CurrentTrackId].Split(',')[0];
                    
        //            return trackId;
        //        }
        //        else
        //            throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
        //    }
        //}

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
            if (CurrentTrackId >= playTracks.Length -1)
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
            Debug.WriteLine("media player failed with error code " + args.ExtendedErrorCode.ToString());
        }

        #endregion

        #region Playlist command handlers
        
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
            //mediaPlayer.SetUriSource(new Uri("http://stream190b.grooveshark.com/stream.php?streamKey=ec0ecb2381dd84254d4c6c3840cdb480f2867493_549bfc73_155a009_1161af9_16af82439_8_0"));
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
                rdStream = new MyPlaylistManager.ShoutCast.RadioStream(_ri);

                rdStream.NoSetUp += (obj, e) =>
                {
                    bool failed = (bool)obj;
                    sendNoPlayMessage(failed);                    
                };  

                rdStream.setUpped += async (obj, e) =>
                {
                    ri = (radioItems)obj;
                    await Task.Delay(TimeSpan.FromSeconds(6));
                    InitializeMediaStreamSource();                   
                };                          
            }
            catch (Exception ex) 
            { 
                string t = ex.Message; 
            }
            
        }

        public void PlayGSTrack(string gsUrl)
        {
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetUriSource(new Uri(gsUrl));
        }

        private void sendNoPlayMessage(bool failed)
        {
            ValueSet message = new ValueSet();
            message.Add(Constants.PlayRadioFailed, failed);
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        #region MSS stuff

        public void InitializeMediaStreamSource()
        {
            byteOffset = 0;
            timeOffset = new TimeSpan(0);

            List<string> encodingPropertiesToRetrieve = new List<string>();

            encodingPropertiesToRetrieve.Add("System.Audio.SampleRate");
            encodingPropertiesToRetrieve.Add("System.Audio.ChannelCount");
            encodingPropertiesToRetrieve.Add("System.Audio.EncodingBitrate");
                                                                                    // sets up MSS with parameters gotten from Radiostream class
            AudioEncodingProperties audioProps = AudioEncodingProperties.CreateMp3(ri.samplePerSec, ri.channels, ri.agvBytes);

            AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(audioProps);
         
            MSS = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
            MSS.CanSeek = true;
            MSS.MusicProperties.Title = "Title";
            MSS.Duration = new TimeSpan(10, 0, 0, 0);

            MSS.Starting += MSS_Starting;               // MSS handlers
            MSS.SampleRequested += MSS_SampleRequested;
            MSS.Closed += MSS_Closed;
            mediaPlayer.SetMediaSource(MSS);
        }



        //async private void InitializeMediaStreamSource2()
        //{          
        //    byteOffset = 0;
        //    timeOffset = new TimeSpan(0);

        //    List<string> encodingPropertiesToRetrieve = new List<string>();

        //    encodingPropertiesToRetrieve.Add("System.Audio.SampleRate");
        //    encodingPropertiesToRetrieve.Add("System.Audio.ChannelCount");
        //    encodingPropertiesToRetrieve.Add("System.Audio.EncodingBitrate");

        //    IDictionary<string, object> encodingProperties = await inputMP3File.Properties.RetrievePropertiesAsync(encodingPropertiesToRetrieve);

        //    uint sampleRate = (uint)encodingProperties["System.Audio.SampleRate"];
        //    uint channelCount = (uint)encodingProperties["System.Audio.ChannelCount"];
        //    uint bitRate = (uint)encodingProperties["System.Audio.EncodingBitrate"];

        //    MusicProperties mp3FileProperties = await inputMP3File.Properties.GetMusicPropertiesAsync();
        //    songDuration = mp3FileProperties.Duration;

        //    AudioEncodingProperties audioProps = AudioEncodingProperties.CreateMp3(sampleRate, channelCount, bitRate);
        //    AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(audioProps);

        //    MSS = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
        //    MSS.CanSeek = true;
        //    MSS.MusicProperties.Title = mp3FileProperties.Title;
        //    MSS.Duration = songDuration;

        //    MSS.Starting += MSS_Starting;
        //    MSS.SampleRequested += MSS_SampleRequested;
        //    MSS.Closed += MSS_Closed;
        //    mediaPlayer.SetMediaSource(MSS);
        //}

        //async void MSS_SampleRequested2(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        //{
        //    MediaStreamSourceSampleRequest request = args.Request;

        //    // check if the sample requested byte offset is within the file size

        //    if (byteOffset + sampleSize <= mssStream.Size)
        //    {
        //        MediaStreamSourceSampleRequestDeferral deferal = request.GetDeferral();
        //        inputStream = mssStream.GetInputStreamAt(byteOffset);

        //        // create the MediaStreamSample and assign to the request object. 
        //        // You could also create the MediaStreamSample using createFromBuffer(...)

        //        MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, sampleSize, timeOffset);
        //        sample.Duration = sampleDuration;
        //        sample.KeyFrame = true;

        //        // increment the time and byte offset

        //        byteOffset += sampleSize;
        //        timeOffset = timeOffset.Add(sampleDuration);
        //        request.Sample = sample;
        //        deferal.Complete();
        //    }
        //}

        //async void MSS_Starting2(Windows.Media.Core.MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        //{
        //    MediaStreamSourceStartingRequest request = args.Request;
        //    if ((request.StartPosition != null) && request.StartPosition.Value <= MSS.Duration)
        //    {
        //        UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
        //        timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
        //        byteOffset = sampleOffset * sampleSize;
        //    }

        //    // create the RandomAccessStream for the input file for the first time 

        //    if (mssStream == null)
        //    {
        //        MediaStreamSourceStartingRequestDeferral deferal = request.GetDeferral();
        //        try
        //        {
        //            mssStream = await inputMP3File.OpenAsync(FileAccessMode.Read);
        //            request.SetActualStartPosition(timeOffset);
        //            deferal.Complete();
        //        }
        //        catch (Exception)
        //        {
        //            MSS.NotifyError(MediaStreamSourceErrorStatus.FailedToOpenFile);
        //            deferal.Complete();
        //        }
        //    }
        //    else
        //    {
        //        request.SetActualStartPosition(timeOffset);
        //    }
        //}






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

                MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, ri.sampleSize, timeOffset);
                sample.Duration = sampleDuration;
                sample.KeyFrame = true;

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
            if ((request.StartPosition != null) && request.StartPosition.Value <= MSS.Duration)     // pauses if MSS caught up with buffered stream
            {
                UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
                timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
                byteOffset = sampleOffset * ri.sampleSize;
            }

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
        
    }

}

