using MyMusicAPI.DAL;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MyMusicAPI.Helper_Classes
{
    public class FilterStations
    {
        private IMusicCentralRepo cd;
        private HttpClient client; 
        public MemoryStream bufferStream { get; set; }
        private Stream response;    
       
        private bool playRadioOk = false;

        public delegate void StreamSetupCompleteHandler(object sender, System.EventArgs args);
        public event StreamSetupCompleteHandler StreamSetupComplete;

        public event EventHandler<object> setUpped;

        public FilterStations(IMusicCentralRepo repo)
        {
            cd = repo;
        }
        public string urii = "";

        public async Task<bool> RunFilter(string uri)   //  async Task<bool> 
        {
            Uri url;
            if (!TryGetUri(uri, out url))
            {
                return false;
            }
            
            client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            try
            {
                using (Stream feedStream = await client.GetStreamAsync(url))
                {
                    int badger = 0;
                    byte read;
                    while (badger < 66)
                    {
                        read = (byte)feedStream.ReadByte();
                        badger++;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal static bool TryGetUri(string uriString, out Uri uri)
        {
            if (!Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out uri))
            {
                return false;
            }

            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                return false;
            }

            return true;
        }
        
        public void CloseMedia()
        {
            if (response != null) response.Dispose();
            if (bufferStream != null) bufferStream.Dispose();
        }
        
    }
}



//public async void DownloadPageAsync()
//        {

//            Uri url = new Uri(urii);

//            client = new HttpClient();
//            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
//            using (Stream feedStream = await client.GetStreamAsync(url))
//            {
//                int badger = 0;
//                byte read;
//                while (badger < 99)
//                {
//                    read = (byte)feedStream.ReadByte();
//                    badger++;
//                }
//            }
//        }

//        //void GetShoutAsync(IAsyncResult res)
//        //{

//        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
//        //    request.AllowReadStreamBuffering = false;
//        //    request.Method = "GET";

//        //    //request.BeginGetResponse(new AsyncCallback(GetShoutAsync), request);    
//        //    HttpWebRequest request = (HttpWebRequest)res.AsyncState;
//        //    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(res);
//        //    Stream r = response.GetResponseStream();

//        //    byte[] data = new byte[4096];

//        //    int read;

//        //    while ((read = r.Read(data, 0, data.Length)) > 0)
//        //    {
//        //        string hj = read.ToString();
//        //    }

//        //} 
        

        




//using (var client = new HttpClient())
//            {
//                int badger = 0;
//                client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

//                var request = new HttpRequestMessage(HttpMethod.Get, uri);
//                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))                                     
//                {
//                    using (var body = await response.Content.ReadAsStreamAsync())
//                    using (var reader = new StreamReader(body))
//                        while (!reader.EndOfStream)
//                            badger++;
//                }
//            }
//Uri uriOk;
//            if (!TryGetUri(uri, out uriOk))
//            {
//                return false;
//            }

//            try
//            {
//                client = new HttpClient();
//                //httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
//                using (Stream feedStream = await client.GetStreamAsync(uriOk))
//                {
//                    int badger = 0;
//                    byte read;
//                    while (badger < 99)
//                    {
//                        read = (byte)feedStream.ReadByte();
//                        badger++;
//                    }
//                    return true;
//                }
//            }
            
//            catch (Exception ex)
//            {
//                return false;
//            }