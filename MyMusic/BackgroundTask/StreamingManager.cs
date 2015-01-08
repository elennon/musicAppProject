using MyMusic;
using MyPlaylistManager.ShoutCast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace BackgroundTask
{
    public sealed class StreamMgr
    {
        #region Private members
        private static StreamingManager instance;
        #endregion

        #region stream manager methods/properties
        public StreamingManager Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new StreamingManager();
                }
                return instance;
            }
        }

        #endregion
    }

    public sealed class StreamingManager
    {
        private MyPlaylistManager.ShoutCast.RadioStream rdStream;
        private IRandomAccessStream mssStream;
        private Windows.Media.Core.MediaStreamSource MSS = null;

        private uint samplePerSec = 0, channels = 0, agvBytes = 0, sampleSize = 0;
        private radioItems ri = new radioItems();
        private MediaPlayer mediaPlayer;


        private UInt64 byteOffset;
        private TimeSpan timeOffset;
        private IInputStream inputStream;
        private TimeSpan songDuration;
        private TimeSpan sampleDuration = new TimeSpan(0, 0, 0, 0, 70);

        internal StreamingManager()
        {
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
        }

        #region MediaPlayer Handlers

        void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            sender.Play();
            //TrackChanged.Invoke(this, CurrentTrackName);
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("media player failed with error code " + args.ExtendedErrorCode.ToString());
        }

        #endregion

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
               
    }
}
