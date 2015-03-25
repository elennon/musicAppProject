using MyMusicAPI.Models;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Script.Serialization;

namespace MyMusicAPI.Helper_Classes
{
    public static class GrooveShark
    {
        private static string key = "edu_lennon3";     //userName = "kilmaced",   sessionID = "885891ce6963035c0c5cfc578abbbca4",
        private static string secret = "3c5667b78a1b5343def5797e882b2b58", baseServiceUrl = "https://api.grooveshark.com";
        private static bool useHttps = false;

        public static string GetSessionId(string userName, string password)
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "startSession";
            requestParameters.header.Add("wsKey", key);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;

            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            string sId = gh["sessionID"].ToString();
            auth(userName, password, sId);
            return sId;
        }

        private static void auth(string userName, string password, string sesId)
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "authenticateEx";
            requestParameters.parameters.Add("login", userName);
            requestParameters.parameters.Add("password", password);
 
            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sesId);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);
           
        }

        public static List<Artist> getSimilarArtists(string artist, string sessionId, int limit)
        {
            sessionId = GetSessionId("kilmaced", "Rhiabit1");
            int artistId = GetArtist(artist, sessionId);
            object country = getCountry(sessionId);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getSimilarArtists";
            requestParameters.parameters.Add("artistID", artistId);
            requestParameters.parameters.Add("country", country);
            requestParameters.parameters.Add("limit", limit);

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            List<object> artists = gh.Values.ToList();

            var b = artists[0];
            var g = b.GetType();
            Dictionary<string, Object> it = (Dictionary<string, Object>)b;
            var f = it["artists"];
                
            ArrayList tf = (ArrayList)f;      // pull out the first song id
            List<Artist> arts = new List<Artist>();
            foreach (Dictionary<string, Object> j in tf)
            {
                Artist art = new Artist{ Name = j["artistName"].ToString(), GSId = Convert.ToInt32(j["artistID"].ToString()) };
                arts.Add(art);
            }
            return arts;
        }

        private static int GetArtist(string artist, string sessionId)
        {
            sessionId = GetSessionId("kilmaced", "Rhiabit1");
            object country = getCountry(sessionId);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getArtistSearchResults";
            requestParameters.parameters.Add("query", artist);
            requestParameters.parameters.Add("country", country);
            requestParameters.parameters.Add("limit", "1");

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            var we = gh.Values.ToList();

            string artistId = "";
            System.Collections.ArrayList tf = (System.Collections.ArrayList)we[1];      // pull out the first song id
            foreach (Dictionary<string, Object> item in tf)
            {
                var sId = item.ToString();
                artistId = item["ArtistID"].ToString();
            }
            return Convert.ToInt32(artistId);
        }

        public static Track getTrack(string artist, string track, string sessionId)
        {
            sessionId = GetSessionId("kilmaced", "Rhiabit1");
            //var t = getSimilarArtists(artist, sessionId);
            object country = getCountry(sessionId);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getSongSearchResults";
            requestParameters.parameters.Add("query", artist + " " + track);
            requestParameters.parameters.Add("country", country);
            requestParameters.parameters.Add("limit", "1");

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);
            
            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            var we = gh.Values.ToList();

            string songId = "";
            System.Collections.ArrayList tf = (System.Collections.ArrayList)we[0];      // pull out the first song id
            Dictionary<string, Object> firstSong = (Dictionary<string, Object>)tf[0];
            songId = firstSong["SongID"].ToString();
            track = firstSong["SongName"].ToString();
            //foreach (Dictionary<string, Object> item in tf)
            //{
            //    //var sId = item.ToString();
            //    songId = item["SongID"].ToString();
            //    track = item["SongName"].ToString();
            //}
            //Stream st = getSongStream(songId, country, sessionId);
            //return st;
            Dictionary<string, Object> url = getSongStream(songId, country, sessionId);
            var servId = url["StreamServerID"].ToString();
            var urel = url["url"].ToString();
            var streamKey = url["StreamKey"].ToString();
            var secs = url["uSecs"].ToString();
            Track tr = new Track { 
                Name = track, 
                ArtistName = artist, 
                GSSongKey = streamKey,
                GSServerId = servId,
                GSSongKeyUrl = urel,
                GSSongId = songId
            };
            return tr;
        }

        private static Dictionary<string, Object> getSongStream(string songId, object country, string sessionId)
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getSubscriberStreamKey";
            requestParameters.parameters.Add("songID", songId);
            requestParameters.parameters.Add("country", country);

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
 
            return gh;
        }

        public static void Mark30Seconds(string streamKey, string streamServerID, string sessionId)
        {           
            object country = getCountry(sessionId);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "markStreamKeyOver30Secs";
            requestParameters.parameters.Add("streamKey", streamKey);
            requestParameters.parameters.Add("streamServerID", streamServerID);
           
            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

        }

        public static void MarkFinished(string songId, string streamKey, string streamServerID, string sessionId)
        {
            sessionId = GetSessionId("kilmaced", "Rhiabit1");
            object country = getCountry(sessionId);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "markSongComplete";
            requestParameters.parameters.Add("streamKey", streamKey);
            requestParameters.parameters.Add("streamServerID", streamServerID);
            requestParameters.parameters.Add("songID", songId);

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionId);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

        }

        static Stream HttpGet(string url)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            Stream reader;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                reader = resp.GetResponseStream();
                byte[] bty = ReadFully(reader);
            }
            return reader;
        }

        private static object getCountry(string sessionID)
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getCountry";

            requestParameters.header.Add("wsKey", key);
            requestParameters.header.Add("sessionID", sessionID);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, secret);
            string serviceUrl = baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);
            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            object obj = gh;

            return obj;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }

    public class RequestParameters
    {
        public RequestParameters()
        {
            parameters = new Dictionary<string, object>();
            header = new Dictionary<string, string>();
        }

        public string method { get; set; }

        public Dictionary<string, string> header { get; set; }

        public Dictionary<string, Object> parameters { get; set; }
    }

    public static class Encryptor
    {    
        public static string Md5Encrypt(string message, string secret = "")
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);

            HMACMD5 hmacmd5 = new HMACMD5(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacmd5.ComputeHash(messageBytes);
            string result = ByteToString(hashmessage);

            return result;
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2");
            }
            return (sbinary);
        }
    }

    public class ResponseParameters
    {
        public ResponseParameters()
        {
            result = new Dictionary<string, object>();
            header = new Dictionary<string, string>();
        }

        public Dictionary<string, string> header { get; set; }

        public Dictionary<string, Object> result { get; set; }
    }
}



       

        //private object getSong(string url)
        //{
        //    RequestParameters requestParameters = new RequestParameters();

        //    requestParameters.header.Add("wsKey", this.key);
        //    requestParameters.header.Add("sessionID", sessionID);
        //    requestParameters.parameters.Add("streamKey", url);
        //    var jsonSerializer = new JavaScriptSerializer();
        //    string json = jsonSerializer.Serialize(requestParameters);
        //    string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
        //    string serviceUrl = this.baseServiceUrl;
        //    if (useHttps)
        //    {
        //        serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
        //    }
        //    //serviceUrl = "http://stream124c-he.grooveshark.com/stream.php?";
        //    serviceUrl = "http://8388608/stream.php?";
        //    var client = new RestClient(serviceUrl);
        //    var request = new RestRequest(String.Format("sig={0}", encryptedJson.ToLower()), Method.POST);
        //    request.RequestFormat = RestSharp.DataFormat.Json;
        //    request.AddBody(requestParameters);

        //    RestResponse response = (RestResponse)client.Execute(request);

        //    ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);


        //    Dictionary<string, Object> gh = responseParameters.result;
        //    object obj = gh;

        //    return obj;

        //}
