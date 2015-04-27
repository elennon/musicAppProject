using MyMusic.HelperClasses;
using MyMusic.Models;
using MyMusic.Utilities;
//using MyMusic.Utilities;
using Newtonsoft.Json;
//using Sqlite.WP81;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;
using Windows.UI.Xaml;


namespace MyMusic.DAL
{
    class Repository : IRepository
    {
        private HttpClient client = new HttpClient();
        private List<Track> preDbTracks = new List<Track>();

        public ObservableCollection<Track> GetAllTracks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var all = db.Table<Track>().Where(a => a.InTheBin == true).ToList();
                //var al = db.Table<Track>().Where(a => a.Name[0] == '1').ToList();

                var query = db.Table<Track>().OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/radio672.png"; }                    
                    _tracks.Add(tr);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetTracks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var all = db.Table<Track>().Where(a => a.InTheBin == true).ToList();
                //var al = db.Table<Track>().Where(a => a.Name[0] == '1').ToList();
                var query = db.Table<Track>().Where(a => a.InTheBin == false).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/radio672.png"; }                  
                    _tracks.Add(tr);
                }
            }
            return _tracks;
        }

        public Track GetThisTrack(int id)
        {
            Track trk = new Track();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == id).FirstOrDefault();
                if (tr != null)
                {
                    trk = tr;
                }
            }
            return trk;
        }

        public Track GetThisTrack(string artist, string name)
        {
            Track trk = new Track();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.ArtistName == artist && a.Name == name).FirstOrDefault();
                if (tr != null)
                {
                    trk = tr;
                }
            }
            return trk;
        }

        public ObservableCollection<Track> GetTracksLessQpicks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == false && a.InQuickPick == false).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/radio672.png"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        ImageUrl = tr.ImageUrl,
                        FileName = tr.FileName,
                        InTheBin = tr.InTheBin,
                        OrderNo = tr.OrderNo
                        //InEditMode = false
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetQuickPicks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var qp = db.Table<Track>().Where(a => a.InQuickPick == true && a.InTheBin == false).ToList();
                foreach (var tr in qp)
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/music3.jpg"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        RandomPlays = tr.RandomPlays,
                        Plays = tr.Plays,
                        Skips = tr.Skips,
                        ImageUrl = tr.ImageUrl,
                        OrderNo = tr.OrderNo,
                        FileName = tr.FileName
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetQuickPicksWithTempo()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var qp = db.Table<Track>().Where(a => a.InQuickPick == true && a.InTheBin == false && a.tempo != null).ToList();
                foreach (var tr in qp)
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/music3.jpg"; }                    
                    _tracks.Add(tr);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetTopTracks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            List<Track> trs = new List<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                //var topPlays = db.Table<Track>().Where(a => a.Plays > 0 && a.InTheBin == false).OrderByDescending(c => c.Plays).ToList();
                var topPlays = db.Table<Track>().Where(a => a.Plays > 0 && a.InTheBin == false).OrderByDescending(a => a.PerCentRate).Take(30).ToList();
                foreach (var tr in topPlays)        // gets all tracks that were intentionally selected
                {
                    if (string.IsNullOrEmpty(tr.ImageUrl)) { tr.ImageUrl = "ms-appx:///Assets/music3.jpg"; }                   
                    _tracks.Add(tr);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetBinnedTracks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == true).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        ImageUrl = tr.ImageUrl,
                        FileName = tr.FileName,
                        InTheBin = tr.InTheBin
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetTracksByArtist(int id)
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.ArtistId == id).ToList();
                foreach (var tr in tracks)
                {
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        FileName = tr.FileName,
                        ImageUrl = tr.ImageUrl
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetTracksByAlbum(string albId)
        {
            int id = Convert.ToInt32(albId);
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.AlbumId == id).ToList();
                var album = db.Table<Album>().Where(c => c.AlbumId == id).FirstOrDefault();
                foreach (var tr in tracks)
                {
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        FileName = tr.FileName,
                        ImageUrl = tr.ImageUrl,
                        Album = album.Name
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<Track> GetTracksByGenre(string genId)
        {
            int id = Convert.ToInt32(genId);
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.GenreId == id).ToList();
                var albums = db.Table<Album>();
                foreach (var tr in tracks)
                {
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        FileName = tr.FileName,
                        GenreId = tr.GenreId,
                        ImageUrl = tr.ImageUrl,
                        Album = albums.Where(a => a.AlbumId == tr.AlbumId).FirstOrDefault().Name
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public string[] GetListToPlay(int startPos) // orders all songs that come after selected song (+ selected) into a string[]
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var trks = db.Table<Track>().Where(a => a.InTheBin == false && a.OrderNo >= startPos).OrderBy(a => a.Name).ToList();
                string[] trkArray = new string[trks.Count];
                int counter = 0;
                foreach (var item in trks)
                {
                    trkArray[counter] = item.TrackId.ToString() + "," + item.FileName + "," + item.ArtistName + ",notShuffle";
                    counter++;
                }
                return trkArray;
            }
        }

        public void AddRandomPlay(int id)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                Track tr = db.Table<Track>().Where(a => a.TrackId == id).FirstOrDefault();
                if (tr != null)
                {
                    tr.RandomPlays = tr.RandomPlays + 1;
                    db.Update(tr);
                }
                DoPercent(tr);
            }
        }

        public void AddPlay(int id)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == id).FirstOrDefault();
                if (tr != null)
                {
                    tr.Plays++;
                    db.Update(tr);
                }
                DoPercent(tr);
            }
        }

        public void AddSkip(int trackId)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                //tr.Plays--;
                if (tr != null)
                {
                    tr.Skips++;
                    db.Update(tr);
                }
                DoPercent(tr);
            }
        }

        public void AddThisToQuickPick(int trackId)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                if (tr != null)
                {
                    tr.InQuickPick = true;
                    tr.DateAddedToQuickPick = DateTime.Now;
                    db.Update(tr);
                }
            }
        }

        public void OutFromQuickPick(int trackId)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                if (tr != null)
                {
                    tr.InQuickPick = false;
                    db.Update(tr);
                }
            }
        }

        public void BinThis(int trackId)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                if (tr != null)
                {
                    tr.InTheBin = true;
                    db.Update(tr);
                }
            }
        }

        public void BackIn(int trackId)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                if (tr != null)
                {
                    tr.InTheBin = false;
                    db.Update(tr);
                }
            }
        }

        public void AddLike(int trackId, bool ifLiked)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == trackId).FirstOrDefault();
                if (ifLiked)
                {
                    if (tr != null)
                    {
                        tr.Liked = true;
                        db.Update(tr);
                    }
                }
                else
                {
                    if (tr != null)
                    {
                        tr.DisLiked = true;
                        db.Update(tr);
                    }
                }
            }
        }

        public int DoPercent(Track tr)
        {
            int perCent = 0;
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var qp = GetQuickPicks();
               
                var trs = db.Table<Track>().ToList();
                if (trs.Count > 0)
                {
                    int highestPlay = (trs.OrderByDescending(a => a.Plays).Take(1)).FirstOrDefault().Plays;

                    double averageShuffles = 1.0;
                    var dd = trs.Where(n => n.RandomPlays > 0).ToList();                    

                    if (dd.Count > 0)
                    { 
                        averageShuffles = (dd).Select(a => a.RandomPlays).Average();
                    }
                    var bestAverageRate = (highestPlay * 3) + averageShuffles;      // based on plays * 3, +  shuffle * 1

                    var thisAverRate = (tr.Plays * 3) + tr.RandomPlays;
                    var playPerC = (thisAverRate / bestAverageRate) * 100;        // this track % of highest
                    double minusSkips = playPerC - (playPerC * (tr.Skips / 10));     // minus 10% for every skip
                    //tr.PerCentRate = (int)Math.Ceiling(minusSkips);
                    perCent = (int)Math.Ceiling(minusSkips);
                }
                return perCent;
            }
        }   // todo: add higher level shuffle(from )

        public void DoAllPercent()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var trs = db.Table<Track>().ToList();
                
                int highestPlay = (trs.OrderByDescending(a => a.Plays).Take(1)).FirstOrDefault().Plays;
                //int highestShuf = (trs.OrderByDescending(a => a.RandomPlays).Take(1)).FirstOrDefault().RandomPlays;
                //var highestSkip = (trs.OrderByDescending(a => a.Skips).Take(1)).FirstOrDefault();

                //var averagePlays = (trs.Where(n => n.Plays > 0).ToList()).Select(a => a.Plays).Average();
                var averageShuffles = (trs.Where(n => n.RandomPlays > 0).ToList()).Select(a => a.RandomPlays).Average();

                var bestAverageRate = (highestPlay * 3) + averageShuffles;      // based on plays * 3, +  shuffle * 1

                foreach (Track item in trs)
                {
                    var thisAverRate = (item.Plays * 3) + item.RandomPlays;
                    var playPerC = (thisAverRate / bestAverageRate) * 100;        // this track % of highest
                    double minusSkips = playPerC - (playPerC * (item.Skips / 10));     // minus 10% for every skip
               //     item.PerCentRate = (int)Math.Ceiling(minusSkips);

                    db.Update(item);
                }                
                var trzs = db.Table<Track>().ToList();
            }
        } 

        public ObservableCollection<Track> GetShuffleTracks()         //  todo: sort into layers by rate%, binned, q.p.s
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == false);
                foreach (var tr in query)
                {
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        ArtistName = tr.ArtistName,
                        FileName = tr.FileName
                    };
                    _tracks.Add(trk);
                }
            }

            Random rng = new Random();
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }
            return _tracks;
        }
       
        public string[] ShuffleGSListToArray(ObservableCollection<Track> gsList)
        {
            Random rng = new Random();
            int n = gsList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = gsList[k];
                gsList[k] = gsList[n];
                gsList[n] = value;
            }
            string[] trkks = new string[gsList.Count];
            for (int i = 0; i < gsList.Count; i++)
            {
                trkks[i] = gsList[i].ArtistName + "," + gsList[i].Name + "," + gsList[i].GSSessionKey + ",shuffle";
            }
            return trkks;
        }

        public string[] SortGSListToArray(ObservableCollection<Track> gsList)
        {            
            string[] trkks = new string[gsList.Count];
            for (int i = 0; i < gsList.Count; i++)
            {               
                trkks[i] = gsList[i].ArtistName + "," + gsList[i].Name + "," + gsList[i].GSSessionKey + ",shuffle";                 
            }
            return trkks;
        }

        public string[] shuffleThese(ObservableCollection<Track> shfThese)
        {
            string[] trkks = new string[shfThese.Count];
            for (int i = 0; i < shfThese.Count; i++)
            {
                //trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
                trkks[i] = shfThese[i].TrackId.ToString() + "," + shfThese[i].FileName + "," + shfThese[i].ArtistName + ",shuffle";
            }
            return trkks;
        }

        public string[] shuffleAll()
        {
            ObservableCollection<Track> shuffled = new ObservableCollection<Track>();
            shuffled = GetShuffleTracks();
            string[] trkks = new string[shuffled.Count];
            for (int i = 0; i < shuffled.Count; i++)
            {
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].FileName + "," + shuffled[i].ArtistName + ",shuffle";
            }
            return trkks;
        }

        public string[] ShuffleAlbum(string id)
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            _tracks = GetTracksByAlbum(id);

            Random rng = new Random();
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }
            return shuffleThese(_tracks);
        }

        public string[] ShuffleGenre(string id)
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            _tracks = GetTracksByGenre(id);

            Random rng = new Random();
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }
            return shuffleThese(_tracks);
        }

        public string[] ShuffleTopPlays()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            _tracks = GetTopTracks();

            Random rng = new Random();
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }
            return shuffleThese(_tracks);
        }

        public string[] ShuffleQuickPicks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            _tracks = GetQuickPicks();

            Random rng = new Random();
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }
            return shuffleThese(_tracks);
        }

        public string[] ShufflePlaylist(List<Track> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            string[] trkks = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                trkks[i] = list[i].TrackId.ToString() + "," + list[i].FileName + "," + list[i].ArtistName + ",shuffle";
            }
            return trkks;
        }

        public string[] TracksToArray(List<Track> trks) // convert track list to string[]
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {                
                string[] trkArray = new string[trks.Count];
                int counter = 0;
                foreach (var item in trks)
                {
                    trkArray[counter] = item.TrackId.ToString() + "," + item.FileName + "," + item.ArtistName + ",notShuffle";
                    counter++;
                }
                return trkArray;
            }
        }

        public ObservableCollection<Artist> GetArtists()
        {
            ObservableCollection<Artist> _artists = new ObservableCollection<Artist>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Artist>().OrderBy(c => c.Name);
                foreach (var ar in query)
                {
                    //var pic = db.Table<Track>().Where(a => a.ArtistId == ar.ArtistId).FirstOrDefault().ImageUri;
                    var art = new Artist()
                    {
                        Name = ar.Name,
                        ArtistId = ar.ArtistId
                        //Image = pic
                    };
                    _artists.Add(art);
                }
            }
            return _artists;
        }

        public ObservableCollection<Album> GetAlbums()
        {
            ObservableCollection<Album> _albums = new ObservableCollection<Album>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var albs = db.Table<Album>().ToList();
                foreach (var item in albs)
                {
                    Album al = new Album
                    {
                        Name = item.Name,
                        AlbumId = item.AlbumId,
                        ArtistId = item.ArtistId
                    };
                    _albums.Add(al);
                }
            }
            return _albums;
        }

        public ObservableCollection<Album> GetAlbumsByArtist(string id)
        {
            int _id = Convert.ToInt32(id);
            ObservableCollection<Album> _albums = new ObservableCollection<Album>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var albs = db.Table<Album>().Where(a => a.ArtistId == _id);
                foreach (var item in albs)
                {
                    ObservableCollection<Track> ts = new ObservableCollection<Track>();
                    var tracks = db.Table<Track>().Where(a => a.AlbumId == item.AlbumId).ToList();
                    Album al = new Album
                    {
                        Name = item.Name,
                        ArtistId = item.ArtistId,
                        AlbumId = item.AlbumId,                       
                       // ArtistName = tracks.Select(a => a.Artist).FirstOrDefault() //artNme.Name
                    };
                    _albums.Add(al);
                }
            }
            return _albums;
        }

        public ObservableCollection<Genre> GetGenres()
        {
            ObservableCollection<Genre> _genres = new ObservableCollection<Genre>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var geners = db.Table<Genre>();
                foreach (var tr in geners)
                {
                    var gCount = db.Table<Track>().Where(a => a.GenreId == tr.GenreId).Count();
                    string tc = "No of Tracks: " + gCount.ToString();
                    var trk = new Genre()
                    {
                        GenreId = tr.GenreId,
                        Name = tr.Name,
                        TrackCount = tc
                    };
                    _genres.Add(trk);
                }
            }
            return _genres;
        }

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

        #region local playlists

        public int CreatePlaylist(Playlist pl)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                db.Insert(pl);
                var id = db.Table<Playlist>().Where(a => a.Name == pl.Name && a.Description == pl.Description).FirstOrDefault();
                return id.PlaylistId;
            }
        }

        public ObservableCollection<Playlist> GetPlaylists()    
        {
            ObservableCollection<Playlist> plyts = new ObservableCollection<Playlist>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var pls = db.Table<Playlist>().ToList();
                foreach (var tr in pls)
                {                    
                    plyts.Add(tr);
                }
            }
            return plyts;
        }

        public Playlist GetThisPlaylist(int id)
        {
            Playlist plys = new Playlist();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                plys = db.Table<Playlist>().Where(a => a.PlaylistId == id).FirstOrDefault();               
            }
            return plys;
        }

        public ObservableCollection<Track> GetPlaylistTracks(Playlist pl)
        {
            ObservableCollection<Track> plTracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                try
                {
                    var list = db.Table<PlaylistTracks>().ToList();
                    if (list.Count > 0)
                    {
                        var ft = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == pl.PlaylistId).ToList();
                        //List<int> pls = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == pl.PlaylistId).Select(n => n.TrackId).ToList();
                        foreach (var item in ft)
                        {
                            var track = db.Table<Track>().Where(a => a.TrackId == item.TrackId).FirstOrDefault();
                            if (track != null)
                            {
                                plTracks.Add(track);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    string bh = ex.Message;
                }
                           
            }
            return plTracks;
        }

        public ObservableCollection<Track> GetTracksLessThisPlaylist(Playlist pl)
        {
            ObservableCollection<Track> Tracks = GetAllTracks();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                List<int> songsInThisPlaylist = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == pl.PlaylistId).Select(n => n.TrackId).ToList();
                foreach (var item in songsInThisPlaylist)
                {
                    var track = db.Table<Track>().Where(a => a.ArtistId == item).FirstOrDefault();
                    if(Tracks.Contains(track))
                    { Tracks.Remove(track); }
                }
            }
            return Tracks;
        }

        public string[] GetPlayListToPlay(int id) // orders all songs that come after selected song (+ selected) into a string[]
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var plLst = db.Table<Playlist>().Where(a => a.PlaylistId == id).FirstOrDefault();
                var trks = GetPlaylistTracks(plLst).ToList();
                string[] trkArray = new string[trks.Count];
                int counter = 0;
                foreach (var item in trks)
                {
                    trkArray[counter] = item.TrackId.ToString() + "," + item.FileName + "," + item.ArtistName + ",notShuffle";
                    counter++;
                }
                return trkArray;
            }
        }

        public void AddToPlaylist(int playlistId, int trackId)
        {
            
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var plTrack = new PlaylistTracks { PlaylistId = playlistId, TrackId = trackId };
                var ft = db.Table<PlaylistTracks>().ToList();
                db.Insert(plTrack);
            }
            
        }

        public void RemoveFromPlaylist(int playlistId, int trackId)
        {

            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                PlaylistTracks ft = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == playlistId && a.TrackId == trackId).FirstOrDefault();
                if (ft != null)
                { db.Delete(ft); }
            }

        }

        public void RemovePlaylist(Playlist pl)    
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                db.Delete(pl);
            }           
        }

        #endregion

        #region streaming

        public async Task<string> GetGSSessionId(string nme, string pword)
        {
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sampleTracks = GetTopTracks().Take(5);
            string resp = await client.GetStringAsync("api/grooveshark?userName=" + nme + "&password=" + pword);//

            return resp;
        }

        public async Task<ObservableCollection<Artist>> GetSimilarArtists(string sessionId, string artist, int top)
        {
            List<Artist> artists = new List<Artist>();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Artist> arts = new List<Artist>();
            try
            {
                string url = string.Format("api/grooveshark?arts={0}&sessionId={1}&limit={2}", artist, sessionId, top);
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<Artist>>(resp);

                artists.AddRange(result);
            }
            catch (HttpRequestException ex)
            {
                return null;
            }             
            return new ObservableCollection<Artist>(artists);
        }

        public async Task<Track> GetGrooveSharkTrackUrl(string artist, string track, string sessionId)
        {
            Track tr = new Track();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string url = string.Format("api/grooveshark?artist={0}&track={1}&sessionId={2}", artist, track, sessionId);
                string resp = await client.GetStringAsync(url);
                var r = JsonConvert.DeserializeObject<TrackDTO>(resp);  
                tr.Name = r.Name;
                tr.ArtistName = r.ArtistName;
                tr.ImageUrl = r.Image;
                tr.GSSongKeyUrl = r.GSSongKeyUrl;
                tr.GSServerId = r.GSServerId;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            return tr;
        }

        // gets a list of tracks by given artists. then gets gs url for each and returns collection as Tracks
        public async Task<ObservableCollection<Track>> GetDeezerArtistTracks(string artist, int top, string sessionId)
        {
            List<Track> tracks = new List<Track>();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string url = string.Format("api/deezer?artist={0}&topNo={1}", artist, top);
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<KeyValuePair<string, List<string>>>(resp);
                foreach (var item in result.Value)
                {
                    var tr = await GetGrooveSharkTrackUrl(result.Key, item, sessionId);
                    if (tr != null)
                    {                                              
                        tracks.Add(tr);
                    }
                }                
            }
            catch (HttpRequestException)
            {
                return null;
            }
            return new ObservableCollection<Track>(tracks);
        }

        //public async Task<ObservableCollection<Track>> GetLastFmArtistTracks(string artist, int num)
        //{
        //    List<Track> tracks = new List<Track>();
        //    client = new HttpClient();
        //    client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    try
        //    {

        //        string url = string.Format("api/lastfm?artist={0}&num={1}", artist, num);
        //        string resp = await client.GetStringAsync(url);
        //        var result = JsonConvert.DeserializeObject<List<LastFmTrackDTO>>(resp);
        //        foreach (LastFmTrackDTO t in result)
        //        {
        //            Track tr = new Track { Artist = t.artist.Name, Name = t.name, listeners = t.listeners, mbid = t.mbid };
        //            tracks.Add(tr);
        //        }

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return null;
        //    }
        //    return new ObservableCollection<Track>(tracks);
        //}

        public async Task<ObservableCollection<Artist>> GetSimilarLastFmArtists(string artist, int top)
        {
            List<Artist> arts = new List<Artist>();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string url = string.Format("api/lastfm?artist={0}&top={1}", artist, top);
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<LfmArtistDTO>>(resp);
                foreach (LfmArtistDTO t in result)
                {
                    Artist ar = new Artist
                    {
                        Name = t.name
                    };
                    arts.Add(ar);
                }

            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            return new ObservableCollection<Artist>(arts);
        }

        public async Task<ObservableCollection<Track>> GetSimilarLastFmTracks(string artist, int num, string sessionId)
        {
            List<Track> tracks = new List<Track>();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                
                string url = string.Format("api/lastfm?artist={0}&num={1}", artist, num);
                string resp = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<LastFmTrackDTO>>(resp);
                foreach (LastFmTrackDTO t in result)
                {
                    var info = await GetAudioSummaryAsync(t.artist.Name, t.name);                    
                    var tempo = info != null ? info.tempo : 0.0;
                    Track tr = new Track 
                    { 
                        ArtistName = t.artist.Name, 
                        ImageUrl = t.OneImage,
                        Name = t.name, 
                        listeners = t.listeners,                         
                        mbid = t.mbid ,
                        tempo = tempo                        
                    };
                        tracks.Add(tr);
                }
                
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            return new ObservableCollection<Track>(tracks);
        }


        #endregion

        #region data base stuff

        public async Task GetEchoNestInfo(string artist, string track)
        {
            Track tr = new Track();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {               
                try
                {
                    Track t = db.Table<Track>().Where(a => a.ArtistName == artist && a.Name == track).FirstOrDefault();
                    var r = await GetAudioSummaryAsync(t.ArtistName, t.Name);
                    if (r != null)
                    {
                        t.acousticness = r.acousticness;
                        t.analysis_url = r.analysis_url;
                        t.audio_md5 = r.audio_md5;
                        t.danceability = r.danceability;
                        t.duration = r.duration;
                        t.energy = r.energy;
                        t.instrumentalness = r.instrumentalness;
                        t.liveness = r.liveness;
                        t.loudness = r.loudness;
                        t.mode = r.mode;
                        t.speechiness = r.speechiness;
                        t.tempo = r.tempo;
                        t.time_signature = r.time_signature;
                        t.valence = r.valence;                           
                    }
                    else
                    { 
                        t.Summary = true; 
                    }
                    db.Update(t);
                }
                catch (HttpRequestException ex)
                {
                    return;
                }                
            }        
        }

        private async Task<Track> GetAudioSummaryAsync(string artist, string track)
        {
            var info = new Track();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string url = string.Format("api/echeonestinfo?artist={0}&track={1}", artist, track);             
                string resp = await client.GetStringAsync(url);

                if (resp == "null")
                { 
                    return null; 
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<EcheoNestRoot>(resp);
                    info = DtoConverter.EcheoNestRoot2Track(result);
                    return info;
                }
            }
            catch (HttpRequestException ex)
            {
                return null;
            }            
        }

        public async Task SortPics()
        {
            Stopwatch sw = Stopwatch.StartNew();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            { 
                var all = db.Table<Track>().Where(a => a.PicGot != true ).ToList();
                foreach (var tr in all)
                {
                    var f = await getPic(tr.ArtistName, tr.Name);

                    if (string.IsNullOrEmpty(f))
                    {
                        tr.PicGot = true;
                    }
                    else
                    {
                        tr.ImageUrl = f.Split(',')[0];
                        tr.Genre = f.Split(',')[1];
                        tr.PicGot = true;                        
                    }
                    Genre gr = new Genre { Name = tr.Genre };
                    var checkGenre = (from a in db.Table<Genre>()
                                      where a.Name == gr.Name
                                      select a).ToList();
                    if (checkGenre.Count < 1) 
                    { 
                        db.Insert(gr);
                        tr.GenreId = gr.GenreId;
                    }
                    else
                    {
                        tr.GenreId = checkGenre.FirstOrDefault().GenreId;
                    }
                    
                    db.Update(tr);

                }                   
             }
            var time = (double)sw.ElapsedMilliseconds / 1000;
            var countfnnf = sngs.Count;  
        }

        public async Task AddArtistName()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var all = db.Table<Track>().Where(a => a.ArtistName == null).ToList();
                List<StorageFile> sf = new List<StorageFile>();
                StorageFolder folder = KnownFolders.MusicLibrary;
                IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);

                foreach (var tr in all)
                {
                    var g = lf.Where(a => a.Name == tr.FileName).FirstOrDefault();
                    if(g != null)
                    {
                        var song = await g.Properties.GetMusicPropertiesAsync();
                        tr.ArtistName = song.Artist;
                        db.Update(tr);
                    }                    
                }
            }
        }

        private async Task<string> getPic(string artist, string title)
        {
            string pic = "ms-appx:///Assets/misc.png", genre = "Unknown";
            try
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                string url = string.Format("?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
                string xmlString = await client.GetStringAsync(url);
                XDocument doc = XDocument.Parse(xmlString);

                if (doc.Descendants("image").Any())
                {
                    pic = (from el in doc.Descendants("image")
                                    where (string)el.Attribute("size") == "large"
                                    select el).First().Value;
                }
                if (doc.Descendants("tag").Any())
                {
                    XNode genr = (((from el in doc.Descendants("tag")
                                    select el).First()).DescendantNodes().First());
                    genre = (genr as XElement).Value;
                }                              
            }
            catch (HttpRequestException)
            {
                return pic + "," + genre;
            }
            return pic + "," + genre;
        }

        public async Task FillRadioDB()
        {
            Stopwatch sw = Stopwatch.StartNew();
            client = new HttpClient();
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
            var time = (double)sw.ElapsedMilliseconds / 1000;
        }   // radio from api

        //public async void BackUpDb()
        //{            
        //    using (var db = new SQLite.SQLiteConnection(App.DBPath))
        //    {
        //        List<Track> tts = new List<Track>();
        //        var trks = db.Table<Track>().Take(40);
        //        tts.AddRange(trks);
        //        var arts = db.Table<Artist>();
        //        var albs = db.Table<Album>();
        //        var gens = db.Table<Genre>();
        //        BackUpDB bk = new BackUpDB() 
        //        { 
        //            Name = "Back Up:" + (DateTime.Now).ToString() ,
        //            Tracks = new List<Track>(trks),
        //            //Artists = new List<Artist>(arts),
        //            //Albums = new List<Album>(albs),
        //            //Genres = new List<Genre>(gens) 
        //        };
        //        StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Data\BackUpDb.xml");
        //        //using (Stream stream = (await file.OpenReadAsync()).AsStreamForRead())
        //        //using (BinaryReader reader = new BinaryReader(stream))
        //        //{
        //        //    Windows.Storage.FileProperties.BasicProperties x = await file.GetBasicPropertiesAsync();
        //        //}

        //        var serializer = new DataContractSerializer(typeof(BackUpDB));

        //        //var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/BackUpDb.xml"));
        //        using (var fileStream = await file.OpenStreamForWriteAsync())
        //        {
        //            //serializer.Serialize(fileStream, bk);
        //            serializer.WriteObject(fileStream, bk);
                    
        //        }

        //        var myStream = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/BackUpDb.xml"));
        //        var gh = await myStream.OpenStreamForReadAsync();
        //        using (StreamReader reader = new StreamReader(gh))
        //        {
        //            string content = await reader.ReadToEndAsync();
        //        }
                
        //        BackUpDB bb = new BackUpDB();
        //        var fiile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/test/BackUpDb.xml"));
        //        using (var fileStream = await fiile.OpenStreamForReadAsync())
        //        {
        //            var ft = new List<Track>();
        //            serializer.WriteObject(fileStream,ft);
        //        }
        //    }            
        //}

        public static async Task SaveObjectToXml<T>(T objectToSave, string filename)
        {            
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            Stream stream = await file.OpenStreamForWriteAsync();
            using (stream)
            {
                serializer.Serialize(stream, objectToSave);
            }
        }
        
        public List<Track> sngs = new List<Track>();
        public async Task fillDB4()
        {
            DropDB();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Track> songs = new List<Track>();
                var cnt = db.Table<Track>().ToList();
                try
                {
                    List<StorageFile> sf = new List<StorageFile>();
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                    int quarter = lf.Count / 8;
                    Task m1Task = q1(0, quarter, lf);
                    Task m2Task = q2(0, quarter, lf);
                    Task m3Task = q3(0, quarter, lf);
                    Task m4Task = q4(0, quarter, lf);
                    Task m5Task = q5(0, quarter, lf);
                    Task m6Task = q6(0, quarter, lf);
                    Task m7Task = q7(0, quarter, lf);
                    Task m8Task = q8(0, quarter, lf);
                    Task all = Task.WhenAll(m1Task, m2Task, m3Task, m4Task, m5Task, m6Task, m7Task, m8Task);
                    await all;

                    var time = (double)sw.ElapsedMilliseconds / 1000;
                    var countfnnf = sngs.Count;                    
                }    
                catch (Exception)
                {                    
                    throw;
                }
            }
        }
   
        #region eights
        private async Task q1(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            List<Track> songs = new List<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = start; i < quarter; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q2(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter; i < quarter * 2; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }

                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q3(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter * 2; i < quarter * 3; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }

                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q4(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter * 3; i < quarter * 4; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q5(int start, int eight, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = eight * 4; i < eight * 5; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q6(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter * 5; i < quarter * 6; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q7(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter * 6; i < quarter * 7; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        private async Task q8(int start, int quarter, IReadOnlyList<StorageFile> lf)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                for (int i = quarter * 7; i < lf.Count; i++)
                {
                    try
                    {

                        var song = await lf[i].Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty)
                        {
                            if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                            else
                            {
                                string[] splitter = song.Title.Split('-');
                                tr.Name = splitter[splitter.Count() - 1];
                                tr.ArtistName = splitter[0];
                            }

                        }
                        else
                        {
                            tr = new Track
                            {
                                Name = song.Title,
                                ArtistName = song.Artist,
                                Plays = 0,
                                Skips = 0,
                                FileName = lf[i].Name
                            };
                        }
                        tr.DateAdded = DateTime.Now;
                        db.Insert(tr);
                        sngs.Add(tr);
                    }
                    catch (Exception ex) { throw ex; }
                }
            }
        }
        #endregion

        private async Task<List<Track>> GetSampleTracks()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string user = "user1";
                string url = string.Format("api/user?id={0}", "user1");
                var resp = await client.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                IEnumerable<TrackDTO> result = await resp.Content.ReadAsAsync<IEnumerable<TrackDTO>>();
                var trs = DtoConverter.DtoTotracks(result.ToList());
                return trs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task fillDbFromXml()
        {            
            DropDB();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var cnt = db.Table<Track>().ToList();
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    List<Track> trs = await GetSampleTracks();
                    List<object> updates = new List<object>();
                    foreach (var tr in trs)
                    {
                        db.Insert(tr);
                        
                        Artist ar = new Artist { Name = tr.ArtistName };
                        Album al = new Album { Name = tr.Album };

                        var checkArtist = db.Table<Artist>().Where(a => a.Name == tr.Name).ToList();
                        if (checkArtist.Count < 1)
                        {
                            db.Insert(ar);
                            tr.ArtistId = ar.ArtistId;
                            updates.Add(ar);
                        }
                        else
                        { tr.ArtistId = checkArtist.FirstOrDefault().ArtistId; }                        

                        var checkAlb = db.Table<Album>().Where(a => a.Name == al.Name).ToList();                          
                        if (checkAlb.Count < 1)
                        {
                            al.ArtistId = tr.ArtistId;
                            db.Insert(al);
                            tr.AlbumId = al.AlbumId;
                        }
                        else
                        { tr.AlbumId = checkAlb.FirstOrDefault().AlbumId; }

                        updates.Add(al);
                        updates.Add(tr);
                        db.UpdateAll(updates);
                        updates.Clear();
                    }
                    //Parallel.ForEach(trs, item =>
                    //{
                    //    db.Insert(item);
                    //    updates.Add(item);
                    //});
                    //db.UpdateAll(updates);
                    var time = (double)sw.ElapsedMilliseconds / 1000;
                    List<object> updtes = new List<object>();
                    var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                    for (int i = 0; i < trks.Count(); i++)
                    {
                        trks[i].OrderNo = i;
                        //db.Update(trks[i]);
                        updtes.Add(trks[i]);
                    }
                    db.UpdateAll(updtes);
                    cnt = db.Table<Track>().ToList();
                    time = (double)sw.ElapsedMilliseconds / 1000;

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task fillDB3()
        {
            DropDB();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var cnt = db.Table<Track>().ToList();
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    List<StorageFile> sf = new List<StorageFile>();
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);                    
                    //var tasks = lf.Select(async item =>
                    //{
                    foreach (var item in lf)
                    {


                        try
                        {
                            List<object> updates = new List<object>();
                            List<int> artists = new List<int>();
                            List<int> albums = new List<int>();
                            var song = await item.Properties.GetMusicPropertiesAsync();
                            Track tr = new Track();
                            if (song.Artist.Contains("{") || song.Artist == string.Empty)
                            {
                                if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                                else
                                {
                                    string[] splitter = song.Title.Split('-');
                                    tr.Name = splitter[splitter.Count() - 1];
                                    tr.ArtistName = splitter[0];
                                }
                            }
                            else
                            {
                                tr = new Track
                                {
                                    Name = song.Title,
                                    ArtistName = song.Artist,
                                    Plays = 0,
                                    Skips = 0,
                                    FileName = item.Name
                                };
                            }
                            tr.DateAdded = DateTime.Now;
                            Artist ar = new Artist { Name = song.Artist };
                            Album al = new Album { Name = song.Album };

                            tr.FileName = item.Name;
                            db.Insert(tr);

                            var checkArtist = (from a in db.Table<Artist>()
                                               where a.Name == ar.Name
                                               select a).ToList();

                            if (checkArtist.Count < 1)
                            {
                                db.Insert(ar);
                                tr.ArtistId = ar.ArtistId;
                            }
                            else
                            { tr.ArtistId = checkArtist.FirstOrDefault().ArtistId; }
                            updates.Add(ar);

                            var checkAlb = (from a in db.Table<Album>()
                                            where a.Name == al.Name
                                            select a).ToList();
                            if (checkAlb.Count < 1)
                            {
                                al.ArtistId = tr.ArtistId;
                                db.Insert(al);
                                tr.AlbumId = al.AlbumId;
                            }
                            else
                            { tr.AlbumId = checkAlb.FirstOrDefault().AlbumId; }

                            updates.Add(al);
                            updates.Add(tr);
                            db.UpdateAll(updates);
                            updates.Clear();
                        }

                        catch (Exception ex) { throw ex; }
                    }
                    //});
                    //await Task.WhenAll(tasks);       
                    //Parallel.ForEach(lf, async currentElement =>                    
                    //{                                          
                    //});
                    var time = (double)sw.ElapsedMilliseconds / 1000;
                    List<object> updtes = new List<object>();
                    var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                    for (int i = 0; i < trks.Count(); i++)
                    {
                        trks[i].OrderNo = i;
                        //db.Update(trks[i]);
                        updtes.Add(trks[i]);
                    }                    
                    db.UpdateAll(updtes); 
                    cnt = db.Table<Track>().ToList();
                    time = (double)sw.ElapsedMilliseconds / 1000;                   
                }
	            catch (Exception)
	            {		
		            throw;
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

        public async Task SyncDB()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var dbTracks = db.Table<Track>().ToList();
                int yre = dbTracks.Count();
                try
                {
                    List<StorageFile> sf = new List<StorageFile>();
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    IReadOnlyList<StorageFile> musicLib = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                    if (dbTracks.Count() != musicLib.Count)
                    {
                        foreach (var item in musicLib)
                        {
                            var checkForTrack = dbTracks.Where(a => a.FileName == item.Name).Count();
                            if (checkForTrack == 0)
                            {
                                await addNewTrack(item);
                            }
                        }
                        foreach (Track item in dbTracks)
                        {
                            var checkForTrack = musicLib.Where(a => a.Name == item.FileName).Count();
                            if (checkForTrack == 0)
                            {
                                db.Delete(item);
                            }
                        }
                    }
                    var updates = (dbTracks).Where(a => a.Summary != true).ToList();
                    if (updates.Count > 0)
                        DoUpdates(updates);
                }
                catch (Exception ex) 
                { 
                    string error = ex.Message; 
                }
                int cnt = db.Table<Track>().Count();              
            }
        }

        private async void DoUpdates(List<Track> updates)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                foreach (var tr in updates)
                {
                    Track t = await GetAudioSummaryAsync(tr.ArtistName, tr.Name);
                    if (t != null)
                    {
                        tr.acousticness = t.acousticness;
                        tr.analysis_url = t.analysis_url;
                        tr.audio_md5 = t.audio_md5;
                        tr.danceability = tr.danceability;
                        tr.duration = t.duration;
                        tr.energy = t.energy;
                        tr.ImageUrl = t.ImageUrl;
                        tr.instrumentalness = t.instrumentalness;
                        tr.key = t.key;
                        tr.liveness = t.liveness;
                        tr.loudness = t.loudness;
                        tr.mbid = t.mbid;
                        tr.tempo = t.tempo;
                        //tr.ImageUrl = t.ImageUrl;
                        //tr.Genre = t.Genre;
                    }
                    else
                    { tr.Summary = true; }
                    db.Update(tr);
                }
            }
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
        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public async Task<List<Track>> GetPrologList()         // sends up top 40 played to get update api db 
        {
            await AddArtistName();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<TrackDTO> tracks = new List<TrackDTO>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {                
                var user = ((App)Application.Current).UserId;
                try
                {
                    string url = string.Format("api/user?id={0}", user);
                    var resp = await client.GetAsync(url);
                    resp.EnsureSuccessStatusCode();
                    IEnumerable<TrackDTO> sp = await resp.Content.ReadAsAsync<IEnumerable<TrackDTO>>();
                    return DtoConverter.DtoTotracks(sp.ToList());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task SyncWithApi()         // sends up top 40 played to get update api db 
        {
          //  await AddArtistName(); 
            client = new HttpClient();           
            client.BaseAddress = new Uri("http://proj400.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<TrackDTO> tracks = new List<TrackDTO>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                //var top = GetTopTracks().Take(20).ToList();
                var top = GetTracks().Take(100).ToList(); 
                
                tracks = DtoConverter.TrackToDTO(top);
                var user = ((App)Application.Current).UserId;           
                UserTrackList ut = new UserTrackList { Songs = tracks, UserId = user };
                try
                {
                    string url = string.Format("api/user");                   
                    var resp = await client.PostAsJsonAsync(url, ut);
                    resp.EnsureSuccessStatusCode();                  
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
       
        private async Task addNewTrack(StorageFile item)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                try
                {
                    var song = await item.Properties.GetMusicPropertiesAsync();
                    Track tr = new Track();
                    if (song.Artist.Contains("{") || song.Artist == string.Empty)
                    {
                        if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
                        else
                        {
                            string[] splitter = song.Title.Split('-');
                            tr.Name = splitter[splitter.Count() - 1];
                            tr.ArtistName = splitter[0];
                        }
                    }
                    else
                    { tr = new Track { Name = song.Title, ArtistName = song.Artist, Plays = 0, Skips = 0, 
                        FileName = item.Name }; }
                    //           MyMusic.HelperClasses.Track getPicAndGenre = await getPic(song.Artist, song.Title);
                    //            tr.ImageUri = getPicAndGenre.ImageUri;
                    tr.DateAdded = DateTime.Now;
                    Artist ar = new Artist { Name = song.Artist };
                    Album al = new Album { Name = song.Album };
                    Genre gr = new Genre { Name = song.Genre.FirstOrDefault() };

                    db.Insert(tr);

                    var checkArtist = (from a in db.Table<Artist>()
                                       where a.Name == ar.Name
                                       select a).ToList();
                    if (checkArtist.Count < 1) { db.Insert(ar); }

                    var checkAlb = (from a in db.Table<Album>()
                                    where a.Name == al.Name
                                    select a).ToList();
                    if (checkAlb.Count < 1) { db.Insert(al); }

                    var checkGenre = (from a in db.Table<Genre>()
                                      where a.Name == gr.Name
                                      select a).ToList();
                    if (checkGenre.Count < 1) { db.Insert(gr); }

                    var arty = db.Table<Artist>().Where(a => a.Name == song.Artist).FirstOrDefault();
                    tr.ArtistId = arty.ArtistId;
                    db.Update(tr);

                    Album album = db.Table<Album>().Where(a => a.Name == song.Album).FirstOrDefault();
                    tr.AlbumId = album.AlbumId;
                    if (album.ArtistId == 0)
                    { al.ArtistId = arty.ArtistId; }
                    var png = await getPic(tr.ArtistName, tr.Name);
                    tr.ImageUrl = png.Split(',')[0];
                    tr.Genre = png.Split(',')[1];

                    db.Update(tr);
                    db.Update(album);                    
                }
                catch (Exception ex) { throw ex; }
            }
        }

        #endregion
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

    public class EcheoNestRoot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Genre { get; set; }
        public object ArtistId { get; set; }
        public object GSSongKey { get; set; }
        public string ArtistName { get; set; }
        public object GSSongKeyUrl { get; set; }
        public object GSServerId { get; set; }
        public object GSSongId { get; set; }
        public object key { get; set; }
        public string analysis_url { get; set; }
        public double energy { get; set; }
        public double liveness { get; set; }
        public double tempo { get; set; }
        public double speechiness { get; set; }
        public double acousticness { get; set; }
        public double instrumentalness { get; set; }
        public int mode { get; set; }
        public int time_signature { get; set; }
        public double duration { get; set; }
        public double loudness { get; set; }
        public string audio_md5 { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
        public string DateAdded { get; set; }
    }
}




//public async Task fillDB2()
//{
//    DropDB();

//    using (var db = new SQLite.SQLiteConnection(App.DBPath))
//    {

//        var cnt = db.Table<Track>().ToList();
//        try
//        {
//            List<StorageFile> sf = new List<StorageFile>();
//            StorageFolder folder = KnownFolders.MusicLibrary;
//            IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);

//            int quarter = lf.Count / 8;
//            Task m1Task = q1(0,quarter,lf);
//            Task m2Task = q2(0, quarter, lf);
//            Task m3Task = q3(0, quarter, lf);
//            Task m4Task = q4(0, quarter, lf);
//            Task m5Task = q5(0, quarter, lf);
//            Task m6Task = q6(0, quarter, lf);
//            Task m7Task = q7(0, quarter, lf);
//            Task m8Task = q8(0, quarter, lf);
//            Task all = Task.WhenAll(m1Task, m2Task, m3Task, m4Task, m5Task, m6Task, m7Task, m8Task);
//            await all;

//        }
//        catch (Exception ex)
//        {
//            string g = ex.InnerException.Message;
//        }
//        var tcnt = db.Table<Track>().ToList();
//        var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
//    }
//}

//public async Task fillDB()
//{
//    DropDB();

//    using (var db = new SQLite.SQLiteConnection(App.DBPath))
//    {

//        var cnt = db.Table<Track>().ToList();
//        try
//        {
//            List<StorageFile> sf = new List<StorageFile>();
//            StorageFolder folder = KnownFolders.MusicLibrary;
//            IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);

//            foreach (var item in lf)
//            {
//                var song = await item.Properties.GetMusicPropertiesAsync();
//                Track tr = new Track();
//                if (song.Artist.Contains("{") || song.Artist == string.Empty)
//                {
//                    if (song.AlbumArtist != string.Empty) { tr.ArtistName = song.AlbumArtist; tr.Name = song.Title; }
//                    else
//                    {
//                        string[] splitter = song.Title.Split('-');
//                        tr.Name = splitter[splitter.Count() - 1];
//                        tr.ArtistName = splitter[0];
//                    }
//                }
//                else
//                { tr = new Track { Name = song.Title, ArtistName = song.Artist, Plays = 0, Skips = 0 }; }
//                tr.DateAdded = DateTime.Now;
//                tr.FileName = item.Name;
//                tr.InTheBin = false;

//                Artist ar = new Artist { Name = song.Artist };
//                Album al = new Album { Name = song.Album };

//                db.Insert(tr);

//                var checkArtist = (from a in db.Table<Artist>()
//                                   where a.Name == ar.Name
//                                   select a).ToList();
//                if (checkArtist.Count < 1) { db.Insert(ar); }

//                var checkAlb = (from a in db.Table<Album>()
//                                where a.Name == al.Name
//                                select a).ToList();
//                if (checkAlb.Count < 1) { db.Insert(al); }

//                var arty = db.Table<Artist>().Where(a => a.Name == song.Artist).FirstOrDefault();
//                tr.ArtistId = arty.ArtistId;
//                db.Update(tr);

//                var album = db.Table<Album>().Where(a => a.Name == song.Album).FirstOrDefault();
//                tr.AlbumId = album.AlbumId;
//                al.ArtistId = arty.ArtistId;
//                db.Update(tr);
//                db.Update(al);
//            }
//        }
//        catch (Exception ex)
//        {
//            string g = ex.InnerException.Message;
//        }
//        var tcnt = db.Table<Track>().ToList();
//        var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
//        for (int i = 0; i < trks.Count(); i++)
//        {
//            trks[i].OrderNo = i;
//            db.Update(trks[i]);
//        }
// //       await GetApiFillDB();
//    }
//}
