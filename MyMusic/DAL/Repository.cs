using MyMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Search;

namespace MyMusic.DAL
{
    class Repository : IRepository
    {
        private HttpClient client = new HttpClient();

        public ObservableCollection<RadioGenre> GetRadioGenres()
        {
            ObservableCollection<RadioGenre> _radioGenres = new ObservableCollection<RadioGenre>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var rdos = db.Table<RadioGenre>().ToList();
                //SortGenreGroups(rdos);
                //var rdoss = db.Table<RadioGenre>().ToList();
                foreach (var tr in rdos)
                {
                    var rdo = new RadioGenre()
                    {
                        RadioGenreId = tr.RadioGenreId,
                        RadioGenreName = tr.RadioGenreName,
                        RadioGenreKey = tr.RadioGenreKey,
                        RadioImage = tr.RadioImage,
                        Group = tr.Group,
                        SectionNo = tr.SectionNo
                    };
                    _radioGenres.Add(rdo);
                }
            }
            return _radioGenres;
        }

        public ObservableCollection<RadioStream> GetRadioStations()
        {
            ObservableCollection<RadioStream> _radioStreams = new ObservableCollection<RadioStream>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var sts = db.Table<RadioStream>().ToList();
                foreach (var tr in sts)
                {
                    var rdo = new RadioStream()
                    {
                        RadioId = tr.RadioId,
                        RadioName = tr.RadioName,
                        RadioUrl = tr.RadioUrl,
                        Image = tr.Image,
                        RadioGenreId = tr.RadioGenreId
                    };
                    _radioStreams.Add(rdo);
                }
            }
            return _radioStreams;
        }

        public async void GetApiFillDB()
        {           
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //string resp = await client.GetStringAsync("api/Connect");
            string resp = await client.GetStringAsync("api/OnLineRadio");
            var result = JsonConvert.DeserializeObject<List<MyMusic.ViewModels.RootObject>>(resp);
            List<RadioStream> stationList = new List<RadioStream>();
            List<RadioGenre> genreList = new List<RadioGenre>();
            foreach (var m in result)
            {
                RadioStream st = new RadioStream { RadioGenreId = (int)m.RadioGenreId, RadioName = m.RadioName, RadioUrl = m.RadioUrl, RadioType = m.StatnType };
                stationList.Add(st);
            }

            string gens = await client.GetStringAsync("api/RadioGenre");
            var genList = JsonConvert.DeserializeObject<List<MyMusic.Models.RootObject>>(gens);

            int div = 1;
            int counter = 1;
            foreach (var item in genList)                   // will only work if no more than 15 stations
            {
                RadioGenre gn = new RadioGenre
                {
                    RadioGenreKey = item.Id.ToString(),
                    RadioGenreName = item.RadioGenreName,
                    RadioImage = item.RadioImage,
                    Group = div,
                    SectionNo = counter
                };
                genreList.Add(gn);

                counter++;
                if (counter == 6) { div++; counter = 1; }
            }

            using (var db = new SQLite.SQLiteConnection(App.DBPath))    //Data/Stations.xml
            {
                int cnt = db.Table<RadioStream>().Count();
                int count = db.Table<RadioGenre>().Count();
                db.DeleteAll<RadioStream>();
                db.DeleteAll<RadioGenre>();
                cnt = db.Table<RadioStream>().Count();
                count = db.Table<RadioGenre>().Count();
                try
                {
                    foreach (var item in genreList)
                    {
                        db.Insert(item);
                    }

                    foreach (var st in stationList)
                    {
                        db.Insert(st);
                    }

                }
                catch (Exception ex)
                {
                    string g = ex.InnerException.Message;
                }
                cnt = db.Table<RadioStream>().Count();
                count = db.Table<RadioGenre>().Count();
            }
        }

        public async void BackUpDb()
        {            
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var trks = db.Table<Track>();
                var arts = db.Table<Artist>();
                var albs = db.Table<Album>();
                var gens = db.Table<Genre>();
                BackUpDB bk = new BackUpDB() 
                { 
                    Name = "Back Up:" + (DateTime.Now).ToString() ,
                    Tracks = new List<Track>(trks),
                    Artists = new List<Artist>(arts),
                    Albums = new List<Album>(albs),
                    Genres = new List<Genre>(gens) 
                };
                //XmlSerializer serializer = new XmlSerializer(typeof(BackUpDB));
                var serializer = new DataContractSerializer(typeof(BackUpDB));

                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/BackUpDb.xml"));
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    //serializer.Serialize(fileStream, bk);
                    serializer.WriteObject(fileStream, bk);
                }

                var myStream = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/BackUpDb.xml"));
                var gh = await myStream.OpenStreamForReadAsync();
                using (StreamReader reader = new StreamReader(gh))
                  {
                      string content = await reader.ReadToEndAsync();
                  }
                

                BackUpDB bb = new BackUpDB();
                var fiile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/BackUpDb.xml"));
                using (var fileStream = await fiile.OpenStreamForReadAsync())
                {
                    //bb = (BackUpDB)serializer.Deserialize(fileStream);

                }
            }
            
        }

        public async void fillDB()
        {
            DropDB();
            
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {

                var cnt = db.Table<Track>().ToList();
                try
                {
                    List<StorageFile> sf = new List<StorageFile>();
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);

                    foreach (var item in lf)
                    {
                        var song = await item.Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.Artist = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.Artist = splitter[0];
                            }
                        }
                        else
                        { tr = new Track { Name = song.Title, Artist = song.Artist, Plays = 0, Skips = 0 }; }

                        tr.FileName = item.Name;
                        tr.InTheBin = false;
                        Artist ar = new Artist { Name = song.Artist };
                        Album al = new Album { Name = song.Album };

                        db.Insert(tr);

                        var checkArtist = (from a in db.Table<Artist>()
                                           where a.Name == ar.Name
                                           select a).ToList();
                        if (checkArtist.Count < 1) { db.Insert(ar); }

                        var checkAlb = (from a in db.Table<Album>()
                                        where a.Name == al.Name
                                        select a).ToList();
                        if (checkAlb.Count < 1) { db.Insert(al); }

                        var arty = db.Table<Artist>().Where(a => a.Name == song.Artist).FirstOrDefault();
                        tr.ArtistId = arty.ArtistId;
                        db.Update(tr);

                        var album = db.Table<Album>().Where(a => a.Name == song.Album).FirstOrDefault();
                        tr.AlbumId = album.AlbumId;
                        al.ArtistId = arty.ArtistId;
                        db.Update(tr);
                        db.Update(al);
                    }
                }
                catch (Exception ex)
                {
                    string g = ex.InnerException.Message;
                }
                var tcnt = db.Table<Track>().ToList();
                var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                for (int i = 0; i < trks.Count(); i++)
                {
                    trks[i].OrderNo = i;
                    db.Update(trks[i]);
                }              
            }
        }

        private void DropDB()
        {
            try
            {
                //StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                //StorageFile dbFile = await folder.GetFileAsync("tracks.s3db");


                // SQLiteConnection connection = new SQLite.SQLiteConnection(App.DBPath);
                //  connection.Dispose();
                //  connection.Close();
                //        SQLiteCommand.Dispose();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                //await dbFile.DeleteAsync();

                //using (var db = new SQLite.SQLiteConnection(App.DBPath))
                //{
                //    db.CreateTable<Track>();
                //    db.CreateTable<Album>();
                //    db.CreateTable<Artist>();
                //    db.CreateTable<Genre>();
                //    db.CreateTable<RadioStream>();
                //    db.CreateTable<RadioGenre>();
                //}
                //File.Delete(filename);

                using (var db = new SQLite.SQLiteConnection(App.DBPath))
                {
                    db.DeleteAll<Track>();
                    //db.CreateTable<Track>();
                    db.DeleteAll<Album>();
                    db.DeleteAll<Artist>();
                    db.DeleteAll<Genre>();
                    db.DeleteAll<RadioStream>();
                    db.DeleteAll<RadioGenre>();
                }
            }
            catch (Exception ex)
            {
                string g = ex.InnerException.Message;
            }
        }
    }

    [XmlRoot]
    public class BackUpDB
    {
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Album> Albums { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
