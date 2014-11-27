using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace SampleBackgroundAudio.MyPlaylistManager.ShoutCast
{
    public sealed class MP3Frame
    {
        private int _headerSize = 4; //header is 4 bytes
        private int _bitsPrByte = 8; //8 bits pr. byte

        public int Version { get; private set; }
        public short Id { get; set; }
        public int Layer { get; private set; }
        public int FrameSize { get; private set; }
        public bool IsProtected { get; private set; } //CRC check. 16 bits will follow the header
        public int BitrateIndex { get; private set; } //bit index in the mpeg specification
        public int SamplingRateIndex { get; private set; }
        public int Padding { get; private set; } //additional bits in the frame
        public int Bitrate { get; private set; }
        public int SamplingRate { get; private set; }
        public int Channels { get; private set; }
        public byte[] Header { get; private set; }

        private static int[,] bitrateTable = new int[,]
        {   
            { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },
            { 0, 32, 48, 56, 64,  80,  96,  112, 128, 160, 192, 224, 256, 320, 384 },
            { 0, 32, 40, 48, 56,  64,  80,  96,  112, 128, 160, 192, 224, 256, 320 },
            { 0, 32, 48, 56, 64,  80,  96,  112, 128, 144, 160, 176, 192, 224, 256 },
            { 0, 8,  16, 24, 32,  40,  48,  56,  64,  80,  96,  112, 128, 144, 160 }
        };

        private static int[,] samplingRateTable = new int[,]
        {   
            { 44100, 48000, 32000 },
            { 22050, 24000, 16000 },
            { 11025, 12000, 8000 }
        };

        //public enum Channel
        //{
        //    //Stereo = 0,
        //    //JointStereo,
        //    //DualChannel,
        //    //SingleChannel,           
        //}

        public MP3Frame([ReadOnlyArray()]byte[] header)
        {
            Header = header;
            if (header.Length < _headerSize) //not able to read 4 byte header
                throw new Exception("Unable to retrieve header");

            SetLayer();

            this.BitrateIndex = MaskBits(16, 4);

            Version = 1;
            SetBitrate();

            this.IsProtected = MaskBits(15, 1) == 1 ? false : true;
            this.SamplingRateIndex = MaskBits(20, 2);
            this.Padding = MaskBits(22, 1);

            SetSamplingRate();
            SetFrameSize();
            SetChannels();
        }

        /// <summary>
        /// Sets up the samplingrate.
        /// </summary>
        private void SetSamplingRate()
        {
            switch (this.Version)
            {
                case 1: // MPEG 1
                    SamplingRate = samplingRateTable[0, this.SamplingRateIndex];
                    break;

                case 2: // MPEG 2
                    SamplingRate = samplingRateTable[1, this.SamplingRateIndex];
                    break;

                case 3: // MPEG 2.5
                    SamplingRate = samplingRateTable[2, this.SamplingRateIndex];
                    break;

                default:
                    SamplingRate = -1;
                    break;
            }
        }

        /// <summary>
        /// Sets the bitrate.
        /// </summary>
        private void SetBitrate()
        {
            switch (this.Version)
            {
                case 1: // Version 1.0
                    switch (Layer)
                    {
                        case 1: // MPEG 1 Layer 1
                            Bitrate = bitrateTable[0, this.BitrateIndex] * 1000;
                            break;
                        case 2: // MPEG 1 Layer 2'
                            Bitrate = bitrateTable[1, this.BitrateIndex] * 1000;
                            break;
                        case 3: // MPEG 1 Layer 3 (MP3)
                            Bitrate = bitrateTable[2, this.BitrateIndex] * 1000;
                            break;
                        default: // MPEG 1 LAYER ERROR
                            Bitrate = -2;
                            break;
                    }
                    break;

                case 2: // Version 2.0
                case 3: // Version 2.5 in reality
                    switch (this.Layer)
                    {
                        case 1: // MPEG 2 or 2.5 Layer 1
                            Bitrate = bitrateTable[3, this.BitrateIndex] * 1000;
                            break;
                        case 2: // MPEG 2 or 2.5 Layer 2
                        case 3: // MPEG 2 or 2.5 Layer 3
                            Bitrate = bitrateTable[4, this.BitrateIndex] * 1000;
                            break;
                        default: // Mpeg 2 LAYER ERROR
                            Bitrate = -2;
                            break;
                    }
                    break;

                default: // VERSION ERROR
                    Bitrate = -2;
                    break;
            }
        }

        /// <summary>
        /// Sets the framesize
        /// </summary>
        private void SetFrameSize()
        {
            switch (this.Layer)
            {
                case 1:
                    FrameSize = ((12 * Bitrate / SamplingRate) + Padding) * 4;
                    break;

                case 2:
                case 3:
                    FrameSize = (144 * Bitrate / SamplingRate) + Padding;
                    break;

                default:
                    FrameSize = -1;
                    break;
            }
        }

        private void SetLayer()
        {
            int layer = MaskBits(13, 2);
            switch (layer)
            {
                case 3:
                    Layer = 1;
                    break;

                case 2:
                    Layer = 2;
                    break;

                case 1:
                    Layer = 3;
                    break;

                default:
                    Layer = -1; //indicate error
                    break;
            }
        }

        /// <summary>
        /// Sets the channels.
        /// </summary>
        private void SetChannels()
        {
            int channelValue = MaskBits(24, 2);

            switch (channelValue)
            {
                case 3:
                    Channels = 1;
                    break;
                case 2:
                    Channels = 2;
                    break;
                case 1:
                    Channels = 2;
                    break;
                case 0:
                    Channels = 3;
                    break;
                default:
                    Channels = 1; // ERROR CASE
                    break;
            }
        }


        /// <summary>
        /// Formats the data required for the CodecData in hexformat.
        /// </summary>
        /// <returns></returns>
        public string ToHexString()
        {
            string s = "";
            s += ToLittleEndianString(string.Format("{0:X4}", 1)); //Id
            s += ToLittleEndianString(string.Format("{0:X8}", Padding));
            s += ToLittleEndianString(string.Format("{0:X4}", (short)FrameSize));
            s += ToLittleEndianString(string.Format("{0:X4}", 1)); //frames pr. block
            s += ToLittleEndianString(string.Format("{0:X4}", 0)); //codecdelay
            return s;
        }

        /// <summary>
        /// Converts the inputstring to the little-endian format.
        /// </summary>
        /// <param name="bigEndianString"></param>
        /// <returns></returns>
        public static string ToLittleEndianString(string bigEndianString)
        {
            if (bigEndianString == null) { return ""; }

            char[] bigEndianChars = bigEndianString.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0) { return ""; }

            int i, ai, bi, ci, di;
            char a, b, c, d;
            for (i = 0; i < bigEndianChars.Length / 2; i += 2)
            {
                // front byte
                ai = i;
                bi = i + 1;

                // back byte
                ci = bigEndianChars.Length - 2 - i;
                di = bigEndianChars.Length - 1 - i;

                a = bigEndianChars[ai];
                b = bigEndianChars[bi];
                c = bigEndianChars[ci];
                d = bigEndianChars[di];

                bigEndianChars[ci] = a;
                bigEndianChars[di] = b;
                bigEndianChars[ai] = c;
                bigEndianChars[bi] = d;
            }

            return new string(bigEndianChars);
        }

        /// <summary>
        /// Returns an integer mask ( 32bit = 4 byte )
        /// </summary>
        /// <param name="firstBit">The starting bit to mask</param>
        /// <param name="maskSize">The masksize in bits</param>
        /// <returns></returns>
        private int MaskBits(int firstBit, int maskSize)
        {
            int mask = 0;
            long returnValue = 0;

            int startByte = firstBit / _bitsPrByte; //the byte containing the starting bit
            int endByte = (firstBit + maskSize - 1) / _bitsPrByte; //the byte containing the end of the mask.

            for (int i = 0; i < maskSize; i++) //build the mask
            {
                mask |= 1 << i;
            }

            long temp;
            int shiftAmount;
            for (int bi = startByte; bi <= endByte; bi++)
            {
                temp = Header[bi];

                shiftAmount = (endByte - bi) * _bitsPrByte;
                temp = temp << shiftAmount;

                returnValue = returnValue | temp;
            }

            returnValue = returnValue >> (((_bitsPrByte * (endByte + 1)) - (firstBit + maskSize)) % 8); // shift the bits to the right to make an int
            returnValue = returnValue & mask; // mask out the appropriate bits

            return (int)returnValue;
        }

    }
}
