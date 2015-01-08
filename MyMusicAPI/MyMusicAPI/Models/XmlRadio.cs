using MyMusicAPI.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MyMusicAPI.Models
{
    public class StreamFinderRadioStations
    {
        private IMusicCentralRepo cd;
        //private MusicCentralContext cd = new MusicCentralContext();
        public StreamFinderRadioStations(IMusicCentralRepo repo)
        {
            cd = repo;
        }

        private List<StreamFinderRadio> _streamFinderRadioStationColl;
        public List<StreamFinderRadio> StreamFinderRadioStationColl
        {
            get
            {
                return _streamFinderRadioStationColl;
            }

            set
            {
                _streamFinderRadioStationColl = value;
            }
        }

        private HttpClient httpClient;
        private CancellationTokenSource cts;
        private string key = "tEsnhZWScj3FfzqtypxBSCx04DM1nCKWdfYQJl5W90OzUuFyqivtylJ3wD3vn5YIX3GkPNMO1HebqbFFOIT7pMCFevEmaXBueMyZhj5mHxy1UKM3V0hiG0yI4DLmCkvl";

        public async Task<bool> Getstations()
        {
            bool allDone = false;
            var sts = await GetRadioStations();
            allDone = ParseStations(sts);

            return allDone;
        }

        private async Task<List<StreamFinderRadio>> GetRadioStations()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
            List<StreamFinderRadio> raders = new List<StreamFinderRadio>();
            List<StreamFinderRadioGenre> geners = await genres();
            foreach (var item in geners)
            {
                string gid = item.RadioGenreKey;

                string queryString = string.Format("http://streamfinder.com/api/index.php?api_codekey={0}&return_data_format=xml&do=genre_search&gid={1}&format=mp3", key, gid);
                try
                {
                    HttpClient http = new System.Net.Http.HttpClient();
                    HttpResponseMessage response = await http.GetAsync(new System.Uri(queryString));
                    var xmlString = response.Content.ReadAsStringAsync().Result;

                    XDocument doc = XDocument.Parse(xmlString);
                    List<XElement> rdoList = (from a in doc.Descendants("data")
                                              select a).ToList();
                    if (rdoList != null)
                    {
                        foreach (XElement data in rdoList)
                        {
                            var _name = data.Element("name").Value;
                            var url = (from a in data.Element("streams").Descendants("stream_url")
                                       select a).First().Value;
                            if ((from a in raders where a.RadioName == _name || a.RadioUrl == url select a).Count() < 1)
                            {
                                raders.Add(new StreamFinderRadio { RadioName = _name.ToString(), RadioUrl = url.ToString(), RadioGenreName = item.RadioGenreName });
                            }
                        }
                    }
                }
                catch (Exception exx) { string error = exx.Message; }
            }
            return raders;
        }

        private async Task<List<StreamFinderRadioGenre>> genres()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
            List<StreamFinderRadioGenre> geners = new List<StreamFinderRadioGenre>();

            string queryString = string.Format("http://streamfinder.com/api/index.php?api_codekey={0}&return_data_format=xml&do=get_genre_list", key);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(queryString));

                string xmlString = response.Content.ReadAsStringAsync().Result;

                XDocument doc = XDocument.Parse(xmlString);

                List<XElement> genList = (from a in doc.Descendants("genre")
                                          select a).ToList();
                if (genList != null)
                {
                    foreach (XElement item in genList)
                    {
                        var gid = item.Element("gid").Value;
                        var _name = item.Element("genre_name").Value;
                        geners.Add(new StreamFinderRadioGenre { RadioGenreName = _name.ToString(), RadioGenreKey = gid.ToString(), RadioImage = "ms-appx:///Assets/music3.jpg" });
                    }
                }
            }
            catch (Exception exx) { string error = exx.Message; }

            return geners;
        }

        private bool ParseStations(IEnumerable<StreamFinderRadio> rdos)
        {
            Stations stations = new Stations();                         // for saving to xml
            List<radioStation> _stations = new List<radioStation>();

            foreach (var file in rdos)
            {
                radioStation rdo = new radioStation();
                List<Urls> Urls = new List<Urls>();
                string contents = file.RadioUrl;

                



                string[] stringSeparators = new string[] { "\n" };    // "\r\n"
                string[] r = contents.Split(stringSeparators, StringSplitOptions.None);
                for (int i = 0; i < r.Count(); i++)
                {
                    if (r[i].Contains(")"))
                    {
                        string[] df = r[i].Split(')');
                        rdo.Name = df[df.Count() - 1];    // title should be on left sidr of () brackets 
                    }
                    if (r[i].Contains("http"))
                    {
                        Urls ur = new Urls { urlName = r[i] };
                        Urls.Add(ur);
                    }
                }
                rdo.Genre = file.RadioGenreName;
                rdo.Name = file.RadioName;
                rdo.Urls = Urls;
                _stations.Add(rdo);
            }
            stations.stations = _stations;
            return SaveToDB(stations);
        }

        private bool SaveToDB(Stations rdos)
        {
            bool allDone = true;
            List<RadioGenre> streamFinderCollection = new List<RadioGenre>();

            var results = rdos.stations.GroupBy(p => p.Genre, (key, g) => new { Genre = key, sts = g.ToList() });
                         
            foreach (var item in results)   // each group of genres
            {
                RadioGenre rg = new RadioGenre { RadioGenreName = item.Genre };
                rg.RadioColl = new List<RadioStream>();
                foreach (var station in item.sts)
                {
                    RadioStream rs = new RadioStream { RadioName = station.Name };  // add each station to its respective genre
                    rs.RadioUrl = new List<string>();
                    foreach (var uri in station.Urls)
                    {                        
                        rs.RadioUrl.Add(uri.urlName);
                    }
                    rg.RadioColl.Add(rs);
                }
                streamFinderCollection.Add(rg); // full collection by genre with stations for each
            }

            foreach (var item in streamFinderCollection)
            {
                cd.insertGenre(item);
                foreach (var radio in item.RadioColl)
                {
                    cd.insertRadioStation(radio);
                }
            }
            
            return allDone;
        }

        private void SaveToXml(Stations rdos)
        {

            string file = string.Format(@"C:\Users\ed\Documents\Visual Studio 2013\Projects\MyMusicAPI\MyMusicAPI\App_Data\Stations.xml");

            XmlSerializer xs = new XmlSerializer(typeof(Stations), new Type[] { typeof(radioStation), typeof(Urls) });

            using (Stream str = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xs.Serialize(str, rdos);
            }
        }

        

    }
    public class StreamFinderRadio
    {
        public int RadioId { get; set; }
        public string RadioName { get; set; }
        public int RadioGenreId { get; set; }
        public string RadioGenreName { get; set; }
        public string RadioUrl { get; set; }
        public string Image { get; set; }

    }

    public class StreamFinderRadioGenre
    {
        public int RadioGenreId { get; set; }
        public string RadioGenreKey { get; set; }
        public string RadioGenreName { get; set; }
        public string RadioImage { get; set; }
    }

    [XmlRoot(ElementName = "Stations")]
    public class Stations : radioStation
    {
        public List<radioStation> stations { get; set; }
    }

    public class radioStation
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Genre { get; set; }
        [XmlElement]
        public List<Urls> Urls { get; set; }
    }

    public class Urls
    {
        [XmlElement]
        public string urlName { get; set; }
        [XmlAttribute]
        public bool isOk { get; set; }
    }

    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Id
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
    }

    public class High
    {
        public string url { get; set; }
    }

    public class Thumbnails
    {
        public Default @default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
    }

    public class Snippet
    {
        public string publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string liveBroadcastContent { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Id id { get; set; }
        public Snippet snippet { get; set; }
    }

    public class RootObject
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }
}
