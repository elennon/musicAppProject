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
using System.Runtime.InteropServices.WindowsRuntime;

namespace BackgroundTask.RadioStreaming
{
    public sealed class RadioItems
    {
        public string Uri { get; set; }
        public uint samplePerSec { get; set; }
        public uint channels { get; set; }
        public uint agvBytes { get; set; }
        public uint sampleSize { get; set; }
       // public IAsyncOperation<MediaEncodingProfile> Descripter { get; set; }
    }

    public sealed class RadioStream
    {        
        public IRandomAccessStream bufferStream { get; set; }
               
        private HttpClient httpClient;
        private IInputStream response;       
        private MediaEncodingProfile MP = new MediaEncodingProfile();    
        private bool playRadio = true;


        public event EventHandler<object> setUpped;

        public RadioStream(string uri)
        {            
            SetUpStream(uri);            
        }
      
        private async void SetUpStream(string Url)      // gets an iinput stream from url, reads a chunk to get audio properties, then feeds into a buffer stream
        {
            bufferStream = new InMemoryRandomAccessStream();
            httpClient = new HttpClient();
            Debug.WriteLine("starting http web stream to feed buffer");
            var url = new Uri(Url);
            try
            {
                response = await httpClient.GetInputStreamAsync(url);   // start actual stream to feed to buffer stream
                MP = await getAudioProps();
              
                setUpped.Invoke(MP, new System.EventArgs()); 
                
                using (Stream feedStream = response.AsStreamForRead())
                {
                    byte read;
                    while (playRadio)
                    {
                        read = (byte)feedStream.ReadByte();
                        bufferStream.AsStreamForRead().WriteByte(read);     // MSS reads from bufferStream
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<MediaEncodingProfile> getAudioProps()    // gets audio properties for the stream
        {
            try
            {
                byte[] buff = new byte[1024 * 24];
                int bytRead = 0;
                bytRead = await (response.AsStreamForRead()).ReadAsync(buff, 0, buff.Length);
                await bufferStream.WriteAsync(buff.AsBuffer());
                bufferStream.Seek(0);
                
                var t = await MediaEncodingProfile.CreateFromStreamAsync(bufferStream);

                return t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BK: failed getting audio properties. Error: " + ex.InnerException);
                CloseMedia();
                return null;
            }
        }
     
        public void CloseMedia()
        {
            if (response != null) response.Dispose();
            if (bufferStream != null) bufferStream.Dispose();
            //GC.SuppressFinalize(this);
        }
    }
}



