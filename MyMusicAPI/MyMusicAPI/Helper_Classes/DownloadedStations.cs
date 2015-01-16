using MyMusicAPI.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyMusicAPI.Models
{
    public class DownloadedStations
    {
        private IMusicCentralRepo cd;

        public DownloadedStations(IMusicCentralRepo repo)
        {
            cd = repo;
        }

        private List<RadioStream> GetFromFolder()
        {
            List<RadioStream> _stations = new List<RadioStream>();
            foreach (string file in Directory.EnumerateFiles("C:\\RadioStations", "*", SearchOption.AllDirectories))
            {
                RadioStream rdo = new RadioStream();

                rdo.RadioGenreName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(file));
                rdo.StatnType = Helper_Classes.RootObject.StationType.Tunin.ToString();
                List<Urls> Urls = new List<Urls>();
                string contents = File.ReadAllText(file);
                string[] stringSeparators = new string[] { "\n" };    // "\r\n"
                string[] r = contents.Split(stringSeparators, StringSplitOptions.None);
                for (int i = 0; i < r.Count(); i++)
                {
                    if (r[i].Contains(")"))
                    {
                        string[] df = r[i].Split(')');
                        rdo.RadioName = df[df.Count() - 1];    // title should be on left sidr of () brackets 
                    }
                    if (r[i].Contains("http"))
                    {
                        rdo.RadioUrl = (r[i]);
                        var b = r[i].Split('/')[2];
                        if (string.IsNullOrEmpty(b))
                            rdo.RadioName = "Unknown";
                        else
                            rdo.RadioName = b;
                    }
                }
                _stations.Add(rdo);
            }
            return _stations;
        }

        public void AddDownloadsToDB()
        {
            List<RadioStream> rds = GetFromFolder();
            foreach (var rd in rds)
            {
                if(cd.CheckGenre(rd.RadioGenreName))
                {
                    string genreImage = "";
                    switch (rd.RadioGenreName)
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
                        case "Dance":
                            genreImage = "ms-appx:///Assets/dance.jpg";
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
                        case "R and B":
                            genreImage = "ms-appx:///Assets/r&b.jpg";
                            break;
                        case "Reggae":
                            genreImage = "ms-appx:///Assets/reggae.jpg";
                            break;
                        case "Rock":
                            genreImage = "ms-appx:///Assets/floyd.jpg";
                            break;
                    }
                    RadioGenre rdg = new RadioGenre { RadioGenreName = rd.RadioGenreName, RadioImage = genreImage };
                    cd.insertGenre(rdg);
                }
                cd.insertRadioStation(rd);
            }
        }
    }
}