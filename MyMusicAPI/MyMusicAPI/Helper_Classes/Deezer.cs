using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MyMusicAPI.Helper_Classes
{
    public static class Deezer
    {
        private static HttpClient client = new HttpClient();

        public async static Task<KeyValuePair<string, List<string>>> GetArtistTop(string artist, int top)
        {
            client = new HttpClient();
            List<string> list = new List<string>();
            client.BaseAddress = new Uri("http://api.deezer.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = string.Format("/search/artist/?q={0}&index=0&limit=1", artist);
            string resp = await client.GetStringAsync(url);
            var arttist = JsonConvert.DeserializeObject<ArtySearchRootObject>(resp);
            var aId = arttist.data.FirstOrDefault();
            if(aId != null)
            {
                url = string.Format("/artist/{0}/top?limit={1}", aId.id, top);      // http://api.deezer.com/artist/941/top?limit=50
                resp = await client.GetStringAsync(url);
                var tops = JsonConvert.DeserializeObject<TracksRootObject>(resp);
                list = tops.data.OrderByDescending(b => b.rank).Select(a => a.title).ToList();    
            }
            return new KeyValuePair<string, List<string>>(artist, list);
        }

        public async static Task<string> getPic(string artist, string title)
        {
            string pic = "";
            client = new HttpClient();
            client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            string url = string.Format("?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
            string xmlString = await client.GetStringAsync(url);
            XDocument doc = XDocument.Parse(xmlString);

            if (doc.Root.FirstAttribute.Value == "failed")
            {
                pic = "ms-appx:///Assets/radio672.png";
            }
            else
            {
                if (doc.Descendants("image").Any())
                {
                    pic = (from el in doc.Descendants("image")
                           where (string)el.Attribute("size") == "large"
                           select el).First().Value;
                }
            }            
            return pic;
        }
    }

    public class TrackDatum
    {
        public int id { get; set; }
        public bool readable { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public int duration { get; set; }
        public int rank { get; set; }
        public bool explicit_lyrics { get; set; }
        public string preview { get; set; }       
        public string type { get; set; }
    }

    public class TracksRootObject
    {
        public List<TrackDatum> data { get; set; }
        public int total { get; set; }
    }


    public class ArtDatum
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public int nb_album { get; set; }
        public int nb_fan { get; set; }
        public bool radio { get; set; }
        public string tracklist { get; set; }
        public string type { get; set; }
    }

    public class ArtySearchRootObject
    {
        public List<ArtDatum> data { get; set; }
        public int total { get; set; }
        public string next { get; set; }
    }
}