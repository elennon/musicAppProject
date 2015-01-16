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
using System.Xml.Serialization;
using Newtonsoft.Json;
using MyMusic.HelperClasses;
using System.Xml;
using SQLite;


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

        private string _album;
        public string Album
        {
            get
            {
                return _album;
            }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    NotifyPropertyChanged("Album");
                }
            }
        }

        private bool _inTheBin = false;
        public bool InTheBin
        {
            get
            {
                return _inTheBin;
            }
            set
            {
                if (_inTheBin != value)
                {
                    _inTheBin = value;
                    NotifyPropertyChanged("InTheBin");
                }
            }
        }

        private bool _inQuickPick = false;
        public bool InQuickPick
        {
            get
            {
                return _inQuickPick;
            }
            set
            {
                if (_inQuickPick != value)
                {
                    _inQuickPick = value;
                    NotifyPropertyChanged("InQuickPick");
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

        private int _perCentRate;
        public int PerCentRate
        {
            get
            {
                return _perCentRate;
            }
            set
            {
                if (_perCentRate != value)
                {
                    _perCentRate = value;
                    NotifyPropertyChanged("PerCentRate"); 
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

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        private string _dateAdded;
        public string DateAdded
        {
            get
            {
                return _dateAdded;
            }
            set
            {
                if (_dateAdded != value)
                {
                    _dateAdded = value;
                    NotifyPropertyChanged("DateAdded");
                }
            }
        }

        private int _genreId;
        public int GenreId
        {
            get
            {
                return _genreId;
            }
            set
            {
                if (_genreId != value)
                {
                    _genreId = value;
                    NotifyPropertyChanged("GenreId");
                }
            }
        }

        #endregion

        public override string ToString()
        {
            //return string.Format(" Play count:  {0} ", Plays + RandomPlays);
            return string.Format(" Play count:  {0} ({1}) %", Plays + RandomPlays, PerCentRate);
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
               
        public void AddRandomPlay(int id)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                Track tr = db.Table<Track>().Where(a => a.TrackId == id).FirstOrDefault();
                if(tr != null)
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
                    item.perCentRate = (int)Math.Ceiling(minusSkips); 

                    db.Update(item);
                }
            }
        }   // todo: add higher level shuffle(from )

        public ObservableCollection<TrackViewModel> GetTopTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            List<TrackViewModel> trs = new List<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                //var topPlays = db.Table<Track>().Where(a => a.Plays > 0 && a.InTheBin == false).OrderByDescending(c => c.Plays).ToList();
                var topPlays = db.Table<Track>().Where(a => a.Plays > 0 && a.InTheBin == false).OrderByDescending(a => a.perCentRate).Take(30).ToList();                           
                foreach (var tr in topPlays)        // gets all tracks that were intentionally selected
                {
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/music3.jpg"; }
                    var trk = new TrackViewModel()
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
                        PerCentRate = tr.perCentRate
                    };
                    _tracks.Add(trk);
                }                
            }            
            return _tracks;
        }

        public ObservableCollection<TrackViewModel> GetQuickPicks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var qp = db.Table<Track>().Where(a => a.InQuickPick == true && a.InTheBin == false).ToList();
                
                foreach (var tr in qp)        
                {
                    if (string.IsNullOrEmpty(tr.ImageUri)) { tr.ImageUri = "ms-appx:///Assets/music3.jpg"; }
                    var trk = new TrackViewModel()
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

        public TrackViewModel GetThisTrack(int id)
        {
            TrackViewModel trk = new TrackViewModel();
            //int ID = Convert.ToInt32(id);
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.TrackId == id).FirstOrDefault();
                if (tr != null)
                {
                    trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        ImageUri = tr.ImageUri,
                        FileName = tr.FileName
                    };
                }
            }
            return trk;
        }

        public ObservableCollection<TrackViewModel> GetTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == false).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
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
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
        }

        public ObservableCollection<TrackViewModel> GetBinnedTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == true).OrderBy(c => c.Name);     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
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

        public ObservableCollection<TrackViewModel> GetShuffleTracks()         //  todo: sort into layers by rate%, binned, q.p.s
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().Where(a => a.InTheBin == false);
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
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
                TrackViewModel value = _tracks[k];
                _tracks[k] = _tracks[n];
                _tracks[n] = value;
            }  
            return _tracks;
        }

        public string[] shuffleAll()
        {
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
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
            _tracks = new ObservableCollection<TrackViewModel>();           
            _tracks = GetTracksByAlbum(id);

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
            return shuffleThese(_tracks);
        }

        public string[] ShuffleGenre(string id)
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            _tracks = GetTracksByGenre(id);

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
            return shuffleThese(_tracks);
        }

        public string[] ShuffleTopPlays()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            _tracks = GetTopTracks();

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
            return shuffleThese(_tracks);
        }

        public ObservableCollection<TrackViewModel> GetTracksByArtist(int id)
        {
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
                        FileName = tr.FileName,
                        ImageUri = tr.ImageUri
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
                var album = db.Table<Album>().Where(c => c.AlbumId == id).FirstOrDefault();
                foreach (var tr in tracks)
                {
                    var trk = new TrackViewModel()
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

        public ObservableCollection<TrackViewModel> GetTracksByGenre(string genId)
        {
            int id = Convert.ToInt32(genId);
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tracks = db.Table<Track>().Where(c => c.GenreId == id).ToList();
                var albums = db.Table<Album>();
                foreach (var tr in tracks)
                {
                    var trk = new TrackViewModel()
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

        public ObservableCollection<TrackViewModel> GetThisArtist(string id)
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                //var query = db.Table<Track>().Where(c => c.Artist.Contains("Van"));
                var query = db.Table<Track>().Where(c => c.AlbumId == 199);
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        ArtistId = tr.ArtistId,
                        AlbumId = tr.AlbumId,
                        Name = tr.Name,
                        Artist = tr.Artist,                      
                        ImageUri = tr.ImageUri,
                        FileName = tr.FileName
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
            //using (var db = new SQLite.SQLiteConnection(App.DBPath))
            //{
            //    var trks = db.Table<Track>().Where(a => a.InTheBin == false).OrderBy(a => a.Name).ToList();
            //    string[] trkArray = new string[trks.Count - (startPos)];
            //    int counter = 0;
            //    for (int i = startPos; i < trks.Count(); i++)
            //    {
            //        trkArray[counter] = trks[i].TrackId.ToString() + "," + trks[i].FileName + "," + trks[i].Artist + ",notShuffle";
            //        counter++;
            //    }
            //    return trkArray;
            //}           
        }

        private string[] shuffleThese(ObservableCollection<TrackViewModel> shfThese)
        {
            string[] trkks = new string[shfThese.Count];
            for (int i = 0; i < shfThese.Count; i++)
            {
                //trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
                trkks[i] = shfThese[i].TrackId.ToString() + "," + shfThese[i].FileName + "," + shfThese[i].Artist + ",shuffle";
            }
            return trkks;
        }

        public async void fillDB()
        {
            DropDB();
            _tracks = new ObservableCollection<TrackViewModel>();
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
                AddRadios();
            }
        }

        public async void AddRadios()
        {
            //await readXMLAsync();
            Stations sts = new Stations();
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
                    XmlSerializer xs = new XmlSerializer(typeof(Stations), new Type[] { typeof(radioStation) });
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/Stations.xml"));
                    using (var fileStream = await file.OpenStreamForReadAsync())
                    {
                        sts = (Stations)xs.Deserialize(fileStream);
                    }

                    foreach (radioStation item in sts.stations.ToList())
                    {
                        RadioStream rds = new RadioStream
                        {
                            RadioName = item.Name,
                            RadioUrl = item.Urls.FirstOrDefault().urlName
                        };
                        db.Insert(rds);
                    }

                    IEnumerable<string> strs = sts.stations.Select(a => a.Genre).Distinct();

                    foreach (var item in strs)
                    {
                        RadioGenre rds = new RadioGenre
                        {
                            RadioGenreName = item,
                            RadioImage = "ms-appx:///Assets/music3.jpg"
                        };
                        db.Insert(rds);
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
                                addNewTrack(item);
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

        private async void addNewTrack(StorageFile item)
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                try
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
                    if(album.ArtistId == 0 || album.ArtistId == null)
                    { al.ArtistId = arty.ArtistId; }
                    
                    db.Update(tr);
                    db.Update(album);
                }
                catch (Exception ex) { throw ex; }
            }
        }

        public async void lookIn()
        {
            List<StorageFile> sf = new List<StorageFile>();
            StorageFolder folder = KnownFolders.MusicLibrary;
            IReadOnlyList<StorageFile> musicLib = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
            foreach (var item in musicLib)
            {
                var song = await item.Properties.GetMusicPropertiesAsync();
                var gen = song.Genre.FirstOrDefault();
                IList<string> ggnr = song.Genre;
                foreach (var g in ggnr)
                {
                    string ggg = g;
                }
                var tr = item;
            }
        }

        public void sortOrderNum()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tcnt = db.Table<Track>().ToList();
                var trks = db.Table<Track>().OrderBy(a => a.Name).ToList();
                for (int i = 0; i < trks.Count(); i++)
                {
                    trks[i].OrderNo = i;
                    db.Update(trks[i]);
                }
                var tccnt = db.Table<Track>().ToList();
            }
        }

        public async Task<string> getPic(string artist, string title)
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
            catch (Exception) 
            {
                return null;
            }
            return picc;
        }

        public async Task<lfm> getGenre(string artist, string title)
        {
            Uri resourceUri;
            lfm tr = new lfm();
            string secHalf = string.Format("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=6101eb7c600c8a81166ec8c5c3249dd4&artist={0}&track={1}", artist, title);
            if (!Helpers.TryGetUri(secHalf, out resourceUri))
            {
                return null;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);

                string xml = await response.Content.ReadAsStringAsync();
                if (xml != null)
                    tr = xml.ParseXML<lfm>();
            }
            catch (Exception)
            {
                return null;
            }
            return tr;
        }        

        public async void loadUpImagesAndGenre()
        {
            string genre = "";
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var ggr = db.Table<Genre>().ToList();
                db.BeginTransaction();         
                var query = db.Table<Track>();     //  .Where(a => a.InTheBin == false)
                foreach (var tr in query)
                {
                    try
                    {
                        lfm tt = await getGenre(tr.Artist, tr.Name);
                        if (tt != null)
                        {                            
                            if (tt.track.toptags.FirstOrDefault() != null)
                            {
                                genre = tt.track.toptags.FirstOrDefault().name;
                            }
                            else { genre = "Unknown"; }

                            var gr = db.Table<Genre>().Where(a => a.Name == genre).FirstOrDefault();
                            if (gr != null)
                            {
                                tr.GenreId = gr.GenreId;
                            }
                            else
                            {
                                Genre grn = new Genre { Name = genre };
                                db.Insert(grn);
                                var grr = db.Table<Genre>().Where(a => a.Name == genre).FirstOrDefault();
                                tr.GenreId = grr.GenreId;
                            }
                        }
                        tr.ImageUri = await getPic(tr.Artist, tr.Name);
                        db.Update(tr);
                    }
                    catch (Exception exx)
                    {
                        db.Rollback();
                        string error = exx.Message;
                    }
                }
                db.Commit();
            }            
        }        
    }
}

