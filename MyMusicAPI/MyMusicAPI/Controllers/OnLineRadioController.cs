
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MyMusicAPI.Controllers
{
    public class OnLineRadioController : ApiController
    {    
        private HttpClient httpClient;
        private CancellationTokenSource cts;
        private string key = "tEsnhZWScj3FfzqtypxBSCx04DM1nCKWdfYQJl5W90OzUuFyqivtylJ3wD3vn5YIX3GkPNMO1HebqbFFOIT7pMCFevEmaXBueMyZhj5mHxy1UKM3V0hiG0yI4DLmCkvl";

        // GET api/onlineradio
        public async Task<IEnumerable<string>> Get()
        {
            IEnumerable<RadioStream> rs = await RadioStations();
            gogo(rs);

            return new string[] { "value1", "value2" };
        }

        private async Task<IEnumerable<RadioStream>> RadioStations()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
            List<RadioStream> raders = new List<RadioStream>();
            List<RadioGenre> geners = await genres();
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
                                raders.Add(new RadioStream { RadioName = _name.ToString(), RadioUrl = url.ToString(), RadioGenreName = item.RadioGenreName });
                            }
                        }
                    }
                }
                catch (Exception exx) { string error = exx.Message; }
            }           
            return raders;
        }

        private void gogo(IEnumerable<RadioStream> rdos)
        {
            Stations stations = new Stations();
            List<radioStation> _stations = new List<radioStation>();
            foreach (var file in rdos)
            {
                radioStation rdo = new radioStation();

                //rdo.Genre = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(file));
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
            Save(stations);
        }

        private void Save(Stations rdos)
        {
           
            string file = string.Format(@"C:\Users\ed\Documents\Visual Studio 2013\Projects\MyMusicAPI\MyMusicAPI\App_Data\Stations.xml");

            XmlSerializer xs = new XmlSerializer(typeof(Stations), new Type[] { typeof(radioStation), typeof(Urls) });

            using (Stream str = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xs.Serialize(str, rdos);
            }            
        }

        private async Task<List<RadioGenre>> genres()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
            List<RadioGenre> geners = new List<RadioGenre>();

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
                        geners.Add(new RadioGenre { RadioGenreName = _name.ToString(), RadioGenreKey = gid.ToString(), RadioImage = "ms-appx:///Assets/music3.jpg" });
                    }
                }
            }
            catch (Exception exx) { string error = exx.Message; }

            return geners;
        }

        // GET api/onlineradio/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/onlineradio
        public void Post([FromBody]string value)
        {
        }

        // PUT api/onlineradio/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/onlineradio/5
        public void Delete(int id)
        {
        }

        

    }
}
