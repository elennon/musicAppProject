using MyMusicAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace MyMusicAPI.Helper_Classes
{
    public static class LastFm
    {
        private static HttpClient client = new HttpClient();

        public async static Task<List<LfmArtist>> GetSimilarArtists(string artist, int num)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
            string url = string.Format("?method=artist.getsimilar&artist={0}&api_key=6101eb7c600c8a81166ec8c5c3249dd4&format=json&limit=6", artist,num);
            string resp = await client.GetStringAsync(url);
            
            var r = JsonConvert.DeserializeObject<LfmArtists>(resp);
            return r.similarartists.artist;
        }

        public async static Task<List<TpTrack>> GetArtistsTop(string artist, int top)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
            string url = string.Format("?method=artist.gettoptracks&artist={0}&limit={1}&api_key=6101eb7c600c8a81166ec8c5c3249dd4&format=json", artist, top);
            string resp = await client.GetStringAsync(url);
            var r = JsonConvert.DeserializeObject<RootObjectToptracks>(resp);
            foreach (var item in r.toptracks.track)
            {
                item.OneImage = await getPic(item.artist.name, item.name);
            }
            return r.toptracks.track;
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
    public class Image
    {
        public string text { get; set; }
        public string size { get; set; }
    }

    public class LfmArtist
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public string match { get; set; }
        public string url { get; set; }
        public List<Image> image { get; set; }
        public string streamable { get; set; }
    }

    public class Attr
    {
        public string artist { get; set; }
    }

    public class Similarartists
    {
        public List<LfmArtist> artist { get; set; }
        //   public Attr __invalid_name__@attr { get; set; }
    }

    public class LfmArtists
    {
        public Similarartists similarartists { get; set; }
    }



   
    public class TpArtist
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
    }

    public class TpImage
    {
        public string text { get; set; }
        public string size { get; set; }
    }

    public class TpAttr
    {
        public string rank { get; set; }
    }

    public class TpTrack
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string playcount { get; set; }
        public string listeners { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }       
        public TpArtist artist { get; set; }
        public List<TpImage> image { get; set; }
        public string OneImage { get; set; }
        public TpAttr attr { get; set; }
    }

    public class TpAttr2
    {
        public string artist { get; set; }
        public string page { get; set; }
        public string perPage { get; set; }
        public string totalPages { get; set; }
        public string total { get; set; }
    }

    public class Toptracks
    {
        public List<TpTrack> track { get; set; }
        public TpAttr2 attr { get; set; }
    }

    public class RootObjectToptracks
    {
        public Toptracks toptracks { get; set; }
    }
}



