using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.HelperClasses
{
    public class GrooveShark
    {
        private string sessionID = "885891ce6963035c0c5cfc578abbbca4", userName = "kilmaced", key = "edu_lennon3";
        private string secret = "3c5667b78a1b5343def5797e882b2b58", baseServiceUrl = "https://api.grooveshark.com";
        private bool useHttps = false;
        private object country;

        private string GetSessionId()
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "startSession";
            requestParameters.header.Add("wsKey", this.key);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;

            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            string sId = gh["sessionID"].ToString();

            return sId;
        }

        private void auth()
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "authenticateEx";
            requestParameters.parameters.Add("login", "kilmaced");
            requestParameters.parameters.Add("password", "Rhiabit1");
            //requestParameters.parameters.Add("query", "nofx");
            //requestParameters.parameters.Add("limit", "50");

            requestParameters.header.Add("wsKey", this.key);
            requestParameters.header.Add("sessionID", sessionID);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);
            string hy = "";

        }

        private void go_Click(object sender, RoutedEventArgs e)
        {

            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getSongSearchResults";
            requestParameters.parameters.Add("query", "nofx bob");
            requestParameters.parameters.Add("country", country);
            requestParameters.parameters.Add("limit", "1");

            requestParameters.header.Add("wsKey", this.key);
            requestParameters.header.Add("sessionID", sessionID);

            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
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
            foreach (Dictionary<string, Object> item in tf)
            {
                var sId = item.ToString();
                songId = item["SongID"].ToString();
            }
            Stream st = getSongStream(songId, country);


        }

        private Stream getSongStream(string songId, object country)
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getSubscriberStreamKey";
            requestParameters.parameters.Add("songID", songId);
            requestParameters.parameters.Add("country", country);

            requestParameters.header.Add("wsKey", this.key);
            requestParameters.header.Add("sessionID", sessionID);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
            }
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("/ws3.php?sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);

            Dictionary<string, Object> gh = responseParameters.result;
            var servId = gh["StreamServerID"].ToString();
            var urel = gh["url"].ToString();
            var streamKey = gh["StreamKey"].ToString();
            var secs = gh["uSecs"].ToString();

            return HttpGet(urel);
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

        private object getSong(string url)
        {
            RequestParameters requestParameters = new RequestParameters();

            requestParameters.header.Add("wsKey", this.key);
            requestParameters.header.Add("sessionID", sessionID);
            requestParameters.parameters.Add("streamKey", url);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
            }
            //serviceUrl = "http://stream124c-he.grooveshark.com/stream.php?";
            serviceUrl = "http://8388608/stream.php?";
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(String.Format("sig={0}", encryptedJson.ToLower()), Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(requestParameters);

            RestResponse response = (RestResponse)client.Execute(request);

            ResponseParameters responseParameters = jsonSerializer.Deserialize<ResponseParameters>(response.Content);


            Dictionary<string, Object> gh = responseParameters.result;
            object obj = gh;

            return obj;

        }

        private object getCountry()
        {
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.method = "getCountry";

            requestParameters.header.Add("wsKey", this.key);
            requestParameters.header.Add("sessionID", sessionID);


            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(requestParameters);
            string encryptedJson = Encryptor.Md5Encrypt(json, this.secret);
            string serviceUrl = this.baseServiceUrl;
            if (useHttps)
            {
                serviceUrl = this.baseServiceUrl.Replace("http://", "https://");
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
        /// <summary>
        /// Encrypts string using MD5s encrypt.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>encrypted md5 hash</returns>
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
