using MyMusicAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xml2CSharp;

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
            string picNGenre = "";
            client = new HttpClient();
            try
            {          
                client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                string url = string.Format("?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
                string xmlString = await client.GetStringAsync(url);
                XDocument doc = XDocument.Parse(xmlString);
                XmlSerializer serializer = new XmlSerializer(typeof(lfm));
         
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
                lfm result = (lfm)serializer.Deserialize(memStream);

                if (doc.Root.FirstAttribute.Value == "failed")
                {
                    picNGenre = "ms-appx:///Assets/radio672.png";
                }
                else
                {
                    picNGenre = result.Track.Album.Image.Where(a => a.Size == "large").FirstOrDefault().Text;
                    picNGenre = picNGenre + "," + result.Track.Toptags.Tag.FirstOrDefault().Name;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return picNGenre;
        }

        public static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
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


    
namespace Xml2CSharp
{
//	[XmlRoot(ElementName="streamable")]
	public class Streamable {
		[XmlAttribute(AttributeName="fulltrack")]
		public string Fulltrack { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

//	[XmlRoot(ElementName="artist")]
	public class Artist {
		[XmlElement(ElementName="name")]
		public string Name { get; set; }
		[XmlElement(ElementName="mbid")]
		public string Mbid { get; set; }
		[XmlElement(ElementName="url")]
		public string Url { get; set; }
	}

//	[XmlRoot(ElementName="image")]
	public class Image {
		[XmlAttribute(AttributeName="size")]
		public string Size { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

//	[XmlRoot(ElementName="album")]
	public class Album {
		[XmlElement(ElementName="artist")]
		public string Artist { get; set; }
		[XmlElement(ElementName="title")]
		public string Title { get; set; }
		[XmlElement(ElementName="mbid")]
		public string Mbid { get; set; }
		[XmlElement(ElementName="url")]
		public string Url { get; set; }
		[XmlElement(ElementName="image")]
		public List<Image> Image { get; set; }
		[XmlAttribute(AttributeName="position")]
		public string Position { get; set; }
	}

//	[XmlRoot(ElementName="tag")]
	public class Tag {
		[XmlElement(ElementName="name")]
		public string Name { get; set; }
		[XmlElement(ElementName="url")]
		public string Url { get; set; }
	}

//	[XmlRoot(ElementName="toptags")]
	public class Toptags {
		[XmlElement(ElementName="tag")]
		public List<Tag> Tag { get; set; }
	}

//	[XmlRoot(ElementName="track")]
	public class Track {
		[XmlElement(ElementName="id")]
		public string Id { get; set; }
		[XmlElement(ElementName="name")]
		public string Name { get; set; }
		[XmlElement(ElementName="mbid")]
		public string Mbid { get; set; }
		[XmlElement(ElementName="url")]
		public string Url { get; set; }
		[XmlElement(ElementName="duration")]
		public string Duration { get; set; }
		[XmlElement(ElementName="streamable")]
		public Streamable Streamable { get; set; }
		[XmlElement(ElementName="listeners")]
		public string Listeners { get; set; }
		[XmlElement(ElementName="playcount")]
		public string Playcount { get; set; }
		[XmlElement(ElementName="artist")]
		public Artist Artist { get; set; }
		[XmlElement(ElementName="album")]
		public Album Album { get; set; }
		[XmlElement(ElementName="toptags")]
		public Toptags Toptags { get; set; }
	}

    [Serializable()]
    [XmlRoot(ElementName="lfm")]
	public class lfm {
		[XmlElement(ElementName="track")]
		public Track Track { get; set; }
		[XmlAttribute(AttributeName="status")]
		public string Status { get; set; }
	}

}





