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
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/radio672.png"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        ImageUri = tr.ImageUri,
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
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/radio672.png"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        ImageUri = tr.ImageUri,
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

        public ObservableCollection<Track> GetTracksLessQpicks()
        {
            ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == false && a.InQuickPick == false).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/radio672.png"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        ImageUri = tr.ImageUri,
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
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/music3.jpg"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        RandomPlays = tr.RandomPlays,
                        Plays = tr.Plays,
                        Skips = tr.Skips,
                        ImageUri = tr.ImageUri,
                        OrderNo = tr.OrderNo,
                        FileName = tr.FileName
                    };
                    _tracks.Add(trk);
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
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/music3.jpg"; }
                    var trk = new Track()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        RandomPlays = tr.RandomPlays,
                        Plays = tr.Plays,
                        Skips = tr.Skips,
                        ImageUri = tr.ImageUri,
                        OrderNo = tr.OrderNo,
                        FileName = tr.FileName,
                        PerCentRate = tr.PerCentRate
                    };
                    _tracks.Add(trk);
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
                        Artist = tr.Artist,
                        ImageUri = tr.ImageUri,
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
                        Artist = tr.Artist,
                        FileName = tr.FileName,
                        ImageUri = tr.ImageUri
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
                        Artist = tr.Artist,
                        FileName = tr.FileName,
                        ImageUri = tr.ImageUri,
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
                        Artist = tr.Artist,
                        FileName = tr.FileName,
                        GenreId = tr.GenreId,
                        ImageUri = tr.ImageUri,
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
                    trkArray[counter] = item.TrackId.ToString() + "," + item.FileName + "," + item.Artist + ",notShuffle";
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

        public void DoPercent()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var trs = db.Table<Track>();
                int highestPlay = (trs.OrderByDescending(a => a.Plays).Take(1)).FirstOrDefault().Plays;
                int highestShuf = (trs.OrderByDescending(a => a.RandomPlays).Take(1)).FirstOrDefault().RandomPlays;
                var highestSkip = (trs.OrderByDescending(a => a.Skips).Take(1)).FirstOrDefault();

                var averagePlays = (trs.Where(n => n.Plays > 0).ToList()).Select(a => a.Plays).Average();
                var averageShuffles = (trs.Where(n => n.RandomPlays > 0).ToList()).Select(a => a.RandomPlays).Average();

                var bestAverageRate = (highestPlay * 3) + averageShuffles;      // based on plays * 3, +  shuffle * 1

                foreach (var item in trs)
                {
                    var thisAverRate = (item.Plays * 3) + item.RandomPlays;
                    var playPerC = (thisAverRate / bestAverageRate) * 100;        // this track % of highest
                    double minusSkips = playPerC - (playPerC * (item.Skips / 10));     // minus 10% for every skip
                    item.PerCentRate = (int)Math.Ceiling(minusSkips);

                    db.Update(item);
                }
            }
        }   // todo: add higher level shuffle(from )

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
                        Artist = tr.Artist,
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

        public string[] shuffleThese(ObservableCollection<Track> shfThese)
        {
            string[] trkks = new string[shfThese.Count];
            for (int i = 0; i < shfThese.Count; i++)
            {
                //trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
                trkks[i] = shfThese[i].TrackId.ToString() + "," + shfThese[i].FileName + "," + shfThese[i].Artist + ",shuffle";
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
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].FileName + "," + shuffled[i].Artist + ",shuffle";
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
                var ft = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == pl.PlaylistId).ToList();
                //List<int> pls = db.Table<PlaylistTracks>().Where(a => a.PlaylistId == pl.PlaylistId).Select(n => n.TrackId).ToList();
                foreach (var item in ft)
                {
                    var track = db.Table<Track>().Where(a => a.TrackId == item.TrackId).FirstOrDefault();
                    plTracks.Add(track);
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
                    trkArray[counter] = item.TrackId.ToString() + "," + item.FileName + "," + item.Artist + ",notShuffle";
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

        public async Task SyncDB()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var dbTracks = db.Table<Track>();
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
                }
                catch (Exception ex) { string error = ex.Message; }
                int cnt = db.Table<Track>().Count();
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
                        if (song.AlbumArtist != string.Empty) { tr.Artist = song.AlbumArtist; tr.Name = song.Title; }
                        else
                        {
                            string[] splitter = song.Title.Split('-');
                            tr.Name = splitter[splitter.Count() - 1];
                            tr.Artist = splitter[0];
                        }

                    }
                    else
                    { tr = new Track { Name = song.Title, Artist = song.Artist, Plays = 0, Skips = 0, FileName = item.Name }; }

                    //           MyMusic.HelperClasses.Track getPicAndGenre = await getPic(song.Artist, song.Title);
                    //            tr.ImageUri = getPicAndGenre.ImageUri;

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

                    db.Update(tr);
                    db.Update(album);
                }
                catch (Exception ex) { throw ex; }
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
