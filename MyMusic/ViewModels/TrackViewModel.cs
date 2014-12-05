using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Threading;
using System.Xml.Linq;


namespace MyMusic.ViewModels
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        #region Properties

        private int _trackId;
        public int TrackId
        {
            get
            {
                return _trackId;
            }
            set
            {
                if (_trackId != value)
                {
                    _trackId = value;
                    NotifyPropertyChanged("TrackId");
                }
            }
        }

        private int _orderNo;
        public int OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                if (_orderNo != value)
                {
                    _orderNo = value;
                    NotifyPropertyChanged("OrderNo");
                }
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private int _ArtistId;
        public int ArtistId
        {
            get
            {
                return _ArtistId;
            }
            set
            {
                if (_ArtistId != value)
                {
                    _ArtistId = value;
                    NotifyPropertyChanged("ArtistId");
                }
            }
        }

        private int _AlbumId;
        public int AlbumId
        {
            get
            {
                return _AlbumId;
            }
            set
            {
                if (_AlbumId != value)
                {
                    _AlbumId = value;
                    NotifyPropertyChanged("AlbumId");
                }
            }
        }

        private string _Artist;
        public string Artist
        {
            get
            {
                return _Artist;
            }
            set
            {
                if (_Artist != value)
                {
                    _Artist = value;
                    NotifyPropertyChanged("Artist");
                }
            }
        }

        private int _plays;
        public int Plays
        {
            get
            {
                return _plays;
            }
            set
            {
                if (_plays != value)
                {
                    _plays = value;
                    NotifyPropertyChanged("Plays");
                }
            }
        }

        private int _rndPlays;
        public int RandomPlays
        {
            get
            {
                return _rndPlays;
            }
            set
            {
                if (_rndPlays != value)
                {
                    _rndPlays = value;
                    NotifyPropertyChanged("RandomPlays");
                }
            }
        }

        private int _skips;
        public int Skips
        {
            get
            {
                return _skips;
            }
            set
            {
                if (_skips != value)
                {
                    _skips = value;
                    NotifyPropertyChanged("Skips");
                }
            }
        }

        private string _imageUri;
        public string ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                if (_imageUri != value)
                {
                    _imageUri = value;
                    NotifyPropertyChanged("ImageUri");
                }
            }
        }

        //private BitmapImage _pic;
        //public BitmapImage Image
        //{
        //    get
        //    {
        //        return _pic;
        //    }
        //    set
        //    {
        //        if (_pic != value)
        //        {
        //            _pic = value;
        //            NotifyPropertyChanged("Image");
        //        }
        //    }
        //}

        //public Artist Composer { get; set; }
        //public Album Album { get; set; }
        //public Genre Genre { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format(" Play count:  {0} ", Plays + RandomPlays);
        }
   
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class TracksViewModel : ViewModelBase
    {
        private ObservableCollection<TrackViewModel> _tracks;
        public ObservableCollection<TrackViewModel> tracks
        {
            get
            {
                return _tracks;
            }

            set
            {
                _tracks = value;
                RaisePropertyChanged("tracks");
            }
        }

        private static HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
        private HttpClient httpClient = new HttpClient(filter);
        private CancellationTokenSource cts = new CancellationTokenSource();
        
        public TrackViewModel ShowStats(string id)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == Convert.ToInt32(id)).FirstOrDefault();
                trk = new TrackViewModel()
                {
                    TrackId = tr.TrackId,
                    Name = tr.Name,
                    Skips = tr.Skips,
                    Plays = tr.Plays
                };
            }
            return trk;
        }

        public void AddRandomPlay(string artist, string track)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                Track tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                if(tr != null)
                {
                    tr.RandomPlays = tr.RandomPlays + 1;
                    db.Update(tr);
                }
                Track trr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
            }
        }

        public void AddPlay(string artist, string track)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                if (tr != null)
                {
                    tr.Plays++;
                    db.Update(tr);
                }
            }
        }

        public void AddSkip(string artist, string track)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                //tr.Plays--;
                if (tr != null)
                {
                    tr.Skips++;
                    db.Update(tr);
                }
            }
        }

        public IEnumerable<TrackViewModel> GetTopTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var topPlays = db.Table<Track>().Where(a => a.Plays > 0).OrderByDescending(c => c.Plays).ToList();
                var topPplays = db.Table<Track>().Where(a => a.Plays > 0).ToList();
                var topShufflePlays = db.Table<Track>().Where(a => a.RandomPlays > 0).OrderByDescending(c => c.RandomPlays).ToList();              
                foreach (var tr in topPlays)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        RandomPlays = tr.RandomPlays,
                        OrderNo =tr.OrderNo,
                        Plays = tr.Plays,
                        Skips = tr.Skips,
                        ImageUri = tr.ImageUri
                    };
                    _tracks.Add(trk);
                }
                foreach (var tr in topShufflePlays)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        RandomPlays = tr.RandomPlays,
                        OrderNo = tr.OrderNo,
                        Plays = tr.Plays,
                        Skips = tr.Skips,
                        ImageUri = tr.ImageUri
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks.Take(30);
        }

        public TrackViewModel GetThisTrack(string id)
        {
            TrackViewModel trk = new TrackViewModel();
            int ID = Convert.ToInt32(id);
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == ID).FirstOrDefault();
                trk = new TrackViewModel()
                {
                    TrackId = tr.TrackId,
                    Name = tr.Name,
                    Artist = tr.Artist,
                    ImageUri = tr.ImageUri
                };
            }
            return trk;
        }

        public ObservableCollection<TrackViewModel> GetTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().OrderBy(c => c.Name);
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        OrderNo = tr.OrderNo,
                        ImageUri = tr.ImageUri
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<TrackViewModel> GetShuffleTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>();
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist
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
                TrackViewModel value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }  
            return _tracks;
        }

        public ObservableCollection<TrackViewModel> GetTracksByArtist(string artId)
        {
            int id = Convert.ToInt32(artId);

            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.ArtistId == id).ToList();
                foreach (var tr in tracks)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        OrderNo = tr.OrderNo
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<TrackViewModel> GetTracksByAlbum(string albId)
        {
            int id = Convert.ToInt32(albId);
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.AlbumId == id).ToList();                
                foreach (var tr in tracks)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        OrderNo = tr.OrderNo
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public async void fillDB()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                int cnt = db.Table<Track>().Count();
                try
                {
                    List<StorageFile> sf = new List<StorageFile>();
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    IReadOnlyList<StorageFile> lf = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                    //foreach (var item in lf)
                    //{
                    //    string h = item.DisplayName;
                    //    var hj = await item.Properties.GetMusicPropertiesAsync();
                    //    string sd = hj.Title;
                    //    string oo = hj.Artist;
                    //}
                    
                    foreach (var item in lf)
                    {
                        var song = await item.Properties.GetMusicPropertiesAsync();
                        Track tr = new Track();
                        if (song.Artist.Contains("{") || song.Artist == string.Empty )
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
                        tr.ImageUri = await getPic(song.Artist, song.Title);
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
                cnt = db.Table<Track>().Count();
                var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                for (int i = 0; i < trks.Count(); i++)
                {
                    trks[i].OrderNo = i;
                    db.Update(trks[i]);
                }
            }
        }

        private async Task<string> getPic(string artist, string title)
        {
            Uri resourceUri;

            string picc = "";
            string secHalf = string.Format("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
            if (!Helpers.TryGetUri(secHalf, out resourceUri))
            {
                return null;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();
                XDocument doc = XDocument.Parse(xmlString);

                if (doc.Root.FirstAttribute.Value == "failed")
                {
                    picc = "ms-appx:///Assets/radio672.png";
                }
                else
                {
                    picc = (from el in doc.Descendants("image")
                            where (string)el.Attribute("size") == "large"
                            select el).First().Value;                   
                }
            }
            catch (Exception exx) { string error = exx.Message; }
            return picc;
        }        


        public void addaColumn()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                int cnt = db.Table<Track>().Count();

                var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                var atrs = db.Table<Artist>();

                //foreach (var item in atrs)
                //{
                //    item.TrackId = trks.Where(a => a.Artist == item.Name).Select(n => n.TrackId).FirstOrDefault();
                //}

                //for (int i = 0; i < trks.Count(); i++)
                //{
                //    trks[i].OrderNo = i;
                //    db.Update(trks[i]);
                //}
                
                
                cnt = db.Table<Track>().Count();
            }
        }
    }
}
