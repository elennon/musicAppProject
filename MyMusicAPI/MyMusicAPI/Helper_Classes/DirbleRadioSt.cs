using MyMusicAPI.DAL;
using MyMusicAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MyMusicAPI.Helper_Classes
{
    public class DirbleRadioSt
    {
        private IMusicCentralRepo cd;
        private HttpClient client = new HttpClient();

        public DirbleRadioSt(IMusicCentralRepo repo)
        {
            cd = repo;
        }

        private async Task<List<RadioStream>> GetStations(string id, string genreName)
        {
            int gId = Convert.ToInt32(id);
            List<RadioStream> stationList = new List<RadioStream>();
            try
            {
                client = new HttpClient();
                List<RadioStream> _stations = new List<RadioStream>();
                client.BaseAddress = new Uri("http://api.dirble.com/v1/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "stations/apikey/2525c6b82d53ec0a6f9b58ea5efc7d27aa1109ea/id/" + gId;
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<RootObject>>(resp);
                
                foreach (var m in result)
                {
                    RadioStream st = new RadioStream
                    {
                        RadioName = m.name,
                        RadioUrl = m.streamurl,
                        StatnType = RootObject.StationType.Dirble.ToString(),
                        RadioGenreName = genreName,
                        Status = (short)m.status
                    };
                    stationList.Add(st);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return stationList;
        }

        private async Task<List<RadioStream>> GetStstationsByCountry(string id, int gId)
        {
            List<RadioStream> stationList = new List<RadioStream>();
            try
            {
                client = new HttpClient();
                List<RadioStream> _stations = new List<RadioStream>();
                client.BaseAddress = new Uri("http://api.dirble.com/v1/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "country/apikey/2525c6b82d53ec0a6f9b58ea5efc7d27aa1109ea/country/" + id;
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<RootObject>>(resp);

                foreach (var m in result)
                {
                    RadioStream st = new RadioStream
                    {
                        RadioName = m.name,
                        RadioUrl = m.streamurl,
                        StatnType = RootObject.StationType.Dirble.ToString(),
                        RadioGenreName = "Irish",
                        RadioGenreId = gId,
                        Status = (short)m.status
                    };
                    stationList.Add(st);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return stationList;
        }

        private async Task<List<RadioGenre>> GetGenres()
        {
            List<RadioGenre> genreList = new List<RadioGenre>();
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("http://api.dirble.com/v1/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string resp = await client.GetStringAsync("primaryCategories/apikey/2525c6b82d53ec0a6f9b58ea5efc7d27aa1109ea");
                var result = JsonConvert.DeserializeObject<List<RootObject>>(resp);

                foreach (var m in result)
                {
                    RadioGenre st = new RadioGenre
                    {
                        RadioGenreName = m.name,
                        RadioGenreKey = m.id.ToString()
                    };
                    genreList.Add(st);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return genreList;
        }

        public async void AddDirbleStationsToDB()
        {
            try
            {
                await AddIrish();
                var genres = await GetGenres();
                foreach (var gn in genres)
                {
                    //cd.AddGenreKey(gn.RadioGenreName, gn.RadioGenreKey);
                    if (cd.CheckGenre(gn.RadioGenreName))
                    {
                        string genreImage = "";
                        switch (gn.RadioGenreName)
                        {
                            case "Altenrative":
                                genreImage = "ms-appx:///Assets/rock.jpg";
                                break;
                            case "Blues":
                                genreImage = "ms-appx:///Assets/jazz2.jpg";
                                break;
                            case "Comedy":
                                genreImage = "ms-appx:///Assets/comedy.jpg";
                                break;
                            case "Country":
                                genreImage = "ms-appx:///Assets/country2.jpg";
                                break;
                            case "Electronic":
                                genreImage = "ms-appx:///Assets/dance2.jpg";
                                break;
                            case "Folk":
                                genreImage = "ms-appx:///Assets/folk.jpg";
                                break;
                            case "Irish":
                                genreImage = "ms-appx:///Assets/paddy.jpg";
                                break;
                            case "Jazz":
                                genreImage = "ms-appx:///Assets/jazz2.jpg";
                                break;
                            case "Pop":
                                genreImage = "ms-appx:///Assets/pop.jpg";
                                break;
                            case "R&B/Urban":
                                genreImage = "ms-appx:///Assets/r&b.jpg";
                                break;
                            case "Reggae":
                                genreImage = "ms-appx:///Assets/reggae.jpg";
                                break;
                            case "Rock":
                                genreImage = "ms-appx:///Assets/floyd.jpg";
                                break;
                            case "Rap":
                                genreImage = "ms-appx:///Assets/rap.jpg";
                                break;
                            case "Misc":
                                genreImage = "ms-appx:///Assets/misc.png";
                                break;
                            case "International":
                                genreImage = "ms-appx:///Assets/inter.jpg";  
                                break;
                            case "Classical":
                                genreImage = "ms-appx:///Assets/classical.jpg";  
                                break;
                            case "Decades":
                                genreImage = "ms-appx:///Assets/decades.jpg";  
                                break;
                        }
                        RadioGenre rdg = new RadioGenre { RadioGenreName = gn.RadioGenreName, RadioImage = genreImage, RadioGenreKey = gn.RadioGenreKey };
                        cd.insertGenre(rdg);
                    }                    
                }
                var gns = cd.GetAllStationGenres().ToList();
                var justDirble = gns.Where(a => string.IsNullOrEmpty(a.RadioGenreKey) == false).ToList();
                foreach (var gn in justDirble)
                {
                    if (gn.RadioGenreName != "Irish")
                    {
                        var stations = await GetStations(gn.RadioGenreKey, gn.RadioGenreName);                       
                        foreach (var item in stations)
                        {
                            cd.insertRadioStation(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddIrish()
        {
            RadioGenre rg = new RadioGenre { RadioGenreKey = "666", RadioGenreName = "Irish", RadioImage = "ms-appx:///Assets/paddy.jpg" };
            cd.insertGenre(rg);
            var gen = cd.GetGenre(rg.RadioGenreName);
            var irishSts = await GetStstationsByCountry("ie", gen.Id );
            foreach (var item in irishSts)
            {
                cd.insertRadioStation(item);
            }
        }

        public void DeletThis(string gName)
        {
            cd.DeleteGenreAndSts(gName);
        }


       
    }

    public class RootObject
    {
        public enum StationType { Dirble, Tunin, StreamFinder}

        public int id { get; set; }
        public string name { get; set; }
        public string website { get; set; }
        public string streamurl { get; set; }
        public string country { get; set; }
        public string bitrate { get; set; }
        public int status { get; set; }
    }
}


//public async void AddDirbleStationsToDB()
//{
//    try
//    {
//        var rds = await GetStations();
//        foreach (var rd in rds)
//        {
//            if (cd.CheckGenre(rd.RadioGenreName))
//            {
//                string genreImage = "";
//                switch (rd.RadioGenreName)
//                {
//                    case "Altenrative":
//                        genreImage = "ms-appx:///Assets/rock.jpg";
//                        break;
//                    case "Blues":
//                        genreImage = "ms-appx:///Assets/jazz2.jpg";
//                        break;
//                    case "Comedy":
//                        genreImage = "ms-appx:///Assets/comedy.jpg";
//                        break;
//                    case "Country":
//                        genreImage = "ms-appx:///Assets/country2.jpg";
//                        break;
//                    case "Dance":
//                        genreImage = "ms-appx:///Assets/dance.jpg";
//                        break;
//                    case "Folk":
//                        genreImage = "ms-appx:///Assets/folk.jpg";
//                        break;
//                    case "Irish":
//                        genreImage = "ms-appx:///Assets/paddy.jpg";
//                        break;
//                    case "Jazz":
//                        genreImage = "ms-appx:///Assets/jazz2.jpg";
//                        break;
//                    case "Pop":
//                        genreImage = "ms-appx:///Assets/pop.jpg";
//                        break;
//                    case "R and B":
//                        genreImage = "ms-appx:///Assets/r&b.jpg";
//                        break;
//                    case "Reggae":
//                        genreImage = "ms-appx:///Assets/reggae.jpg";
//                        break;
//                    case "Rock":
//                        genreImage = "ms-appx:///Assets/floyd.jpg";
//                        break;
//                }
//                RadioGenre rdg = new RadioGenre { RadioGenreName = rd.RadioGenreName, RadioImage = genreImage };
//                cd.insertGenre(rdg);
//            }
//            cd.insertRadioStation(rd);
//        }
//    }
//    catch (Exception ex)
//    {
//        throw ex;
//    }
//}