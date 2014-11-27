using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.Web.Http;

namespace SampleBackgroundAudio.MyPlaylistManager.ShoutCast
{
    public sealed class radioItems
    {
        public string Uri { get; set; }
        public uint samplePerSec { get; set; }
        public uint channels { get; set; }
        public uint agvBytes { get; set; }
        public uint sampleSize { get; set; }

    }

    public sealed class RadioStream
    {
        private const int MP3_HEADER_SIZE = 4; //bytes

        public IRandomAccessStream bufferStream { get; set; }
        private MP3Frame mp3Frame; //mp3 frame metadata
        private WaveFormatEx _wfx;
        private int _mp3FrameHeaderLength = 4;

        private long currentFrameStartPosition;
        private int totalRead = 0;
        private Stream r = null;

        public bool ContinueStreaming { get; set; }
        public short BitRate { get; set; }
        public int MetaInt { get; set; }
        public string Genre { get; private set; }
        public string StationName { get; private set; }

        private radioItems ri = new radioItems();
        private TimeSpan sampleDuration = new TimeSpan(0, 0, 0, 0, 30);

        public event EventHandler<object> setUpped;

        private HttpClient httpClient;
        
        public RadioStream(radioItems rdi)
        {
            ri = rdi;
            StartWebRequest(ri.Uri);           
        }

        private void StartWebRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.BeginGetResponse(new AsyncCallback(FinishWebRequest2), request);
            ContinueStreaming = true;
        }
        private async void FinishWebRequest2(IAsyncResult result)
        {
            HttpWebResponse response1 = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            r = response1.GetResponseStream();

            httpClient = new HttpClient();
            var url = new Uri(ri.Uri);
            var response = await httpClient.GetInputStreamAsync(url);

            InitializeShoutcastHeader(r);
            var mp3HeaderData = await GetMp3Header(r);
            mp3Frame = new MP3Frame(mp3HeaderData);
            _wfx = new WaveFormatEx();
            _wfx.FormatTag = 85;
            _wfx.Channels = (short)(mp3Frame.Channels);
            _wfx.SamplesPerSec = mp3Frame.SamplingRate;
            _wfx.AvgBytesPerSec = mp3Frame.Bitrate / 8;
            _wfx.BlockAlign = 1;
            _wfx.BitsPerSample = 0;
            _wfx.Size = 12;
            ri.agvBytes = (uint)_wfx.AvgBytesPerSec;
            ri.channels = (uint)_wfx.Channels;
            ri.samplePerSec = (uint)_wfx.SamplesPerSec;
            ri.sampleSize = (uint)mp3Frame.FrameSize;

            setUpped.Invoke(ri, new System.EventArgs());

            using (Stream feedStream = response.AsStreamForRead())
            {
                int badger = 0;
                byte[] buffer = new byte[1024 * 24];
                int bytesRead = 0;
                while ((bytesRead = await feedStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    (bufferStream.AsStreamForRead()).Write(buffer, 0, bytesRead);
                    badger += bytesRead;
                }
            }
        }

        #region ShoutCast

        private void IncreaseRead(int readed)
        {
            //totalRead += readed;
            totalRead = (int)(r.Position - currentFrameStartPosition);

            if (this.MetadataAvailable)
                if (totalRead == MetaInt)
                {
                    SkipHeader(r);
                    currentFrameStartPosition = r.Position;
                    totalRead = 0;
                }
        }

        private void IncreaseRead()
        {
            IncreaseRead(1);
        }

        private void InitializeShoutcastHeader(Stream r)
        {
            MetaInt = 8192;
            MetadataAvailable = false;
            StreamReader headerReader = new StreamReader(r);
            bool headerIsDone = false;
            while (!headerIsDone)
            {
                string headerLine = headerReader.ReadLine();

                if (headerLine.StartsWith("icy-name:"))
                {
                    StationName = headerLine.Substring(9);
                }
                else if (headerLine.StartsWith("icy-genre:"))
                {
                    Genre = headerLine.Substring(10);
                }
                else if (headerLine.StartsWith("icy-br:"))
                {
                    BitRate = short.Parse(headerLine.Substring(7));
                }
                else if (headerLine.StartsWith("icy-metaint:"))
                {
                    MetaInt = int.Parse(headerLine.Substring(12));
                    MetadataAvailable = true;
                }
                else if (headerLine.Equals(""))
                    headerIsDone = true;
            }
        }

        private async Task<byte[]> GetMp3Header(Stream r)
        {
            byte syncBit = 0xFF, syncMask = 0xE0; // 1111 1111 1110 0000
            byte readByte;
            byte readByte2;
            byte readByte3;
            byte readByte4;

            while (true)
            {
                readByte = (byte)r.ReadByte();

                IncreaseRead();

                if (readByte == syncBit)
                {
                    readByte2 = (byte)r.ReadByte();

                    IncreaseRead();

                    if ((readByte2 & syncMask) == syncMask)
                    {
                        // AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM


                        // AAAB BCCD
                        // 0000 0000 -> reserved
                        // 0000 0010 -> 02
                        // 0000 0100 -> 04
                        // 0000 0110 -> 06
                        // 0000 1000 -> reserved
                        if ((readByte2 & 0x18) == 0x08 || !((readByte2 & 0x02) == 0x02 || (readByte2 & 0x04) == 0x04 || (readByte2 & 0x06) == 0x06))
                        {
                            // bufferStream.WriteByte(readByte2);

                            using (Stream stream = new MemoryStream(readByte2))
                            {
                                bufferStream = new InMemoryRandomAccessStream();
                                await stream.CopyToAsync(bufferStream.AsStreamForWrite());
                            }

                            continue;
                        }

                        // EEEE FFGH
                        // 1111 0000 -> F0, bad
                        // 0000 1100 -> 0C, reserved
                        readByte3 = (byte)r.ReadByte();
                        IncreaseRead();

                        if ((readByte3 & 0xF0) == 0xF0 || (readByte3 & 0x0C) == 0x0C)
                        {
                            //bufferStream.WriteByte(readByte3);
                            using (Stream stream = new MemoryStream(readByte3))
                            {
                                bufferStream = new InMemoryRandomAccessStream();
                                await stream.CopyToAsync(bufferStream.AsStreamForWrite());
                            }
                            continue;
                        }

                        readByte4 = (byte)r.ReadByte();
                        IncreaseRead();

                        break;
                    }
                    else
                    {
                        //bufferStream.WriteByte(readByte);
                        using (Stream stream = new MemoryStream(readByte))
                        {
                            bufferStream = new InMemoryRandomAccessStream();
                            await stream.CopyToAsync(bufferStream.AsStreamForWrite());
                        }
                    }
                }
                else
                {
                    //bufferStream.WriteByte(readByte);
                    using (Stream stream = new MemoryStream(readByte))
                    {
                        bufferStream = new InMemoryRandomAccessStream();
                        await stream.CopyToAsync(bufferStream.AsStreamForWrite());
                    }
                }
            }

            //SYNC byte found, build the header
            byte[] mp3HeaderData = new byte[_mp3FrameHeaderLength];

            mp3HeaderData[0] = readByte;
            mp3HeaderData[1] = readByte2;
            mp3HeaderData[2] = readByte3;
            mp3HeaderData[3] = readByte4;

            currentFrameStartPosition = r.Position;
            

            return mp3HeaderData;
        }

        public bool MetadataAvailable { get; set; }
        private void SkipHeader(Stream r)
        {
            if (MetadataAvailable)
            {
                var metadataLength = r.ReadByte() * 16;
                byte[] metadata = new byte[metadataLength];
                r.Read(metadata, 0, metadata.Length);
                //_radioStream.Write(metadata, 0, metadata.Length);
                string readData = System.Text.Encoding.UTF8.GetString(metadata, 0, metadata.Length);
            }
        }

        #endregion 
    }
}


