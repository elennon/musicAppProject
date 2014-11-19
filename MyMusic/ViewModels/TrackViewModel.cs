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
            return string.Format("({0}) {1}", Plays, Name);
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
                var tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                tr.RandomPlays ++;
                db.Update(tr);
            }
        }

        public void AddPlay(string artist, string track)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                tr.Plays++;
                db.Update(tr);               
            }
        }

        public void AddSkip(string artist, string track)
        {
            TrackViewModel trk = new TrackViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var tr = db.Table<Track>().Where(a => a.Artist == artist && a.Name == track).FirstOrDefault();
                //tr.Plays--;
                tr.Skips++;
                db.Update(tr); 
            }
        }

        public ObservableCollection<TrackViewModel> GetTopTracks()
        {
            _tracks = new ObservableCollection<TrackViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Track>().OrderByDescending(c => c.Plays).Take(10);
                foreach (var tr in query)
                {
                    var trk = new TrackViewModel()
                    {
                        TrackId = tr.TrackId,
                        Name = tr.Name,
                        Artist = tr.Artist,
                        Plays = tr.Plays,
                        Skips = tr.Skips
                    };
                    _tracks.Add(trk);
                }
            }
            return _tracks;
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
                    Artist = tr.Artist
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
                        Name = tr.Name                        
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
                    }
                }
                catch (Exception ex)
                {
                    string g = ex.InnerException.Message;
                }
                cnt = db.Table<Track>().Count();
            }
        }

        public void addaColumn()
        {
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                int cnt = db.Table<Track>().Count();

                var trks = db.Table<Track>();
                foreach (Track tr in trks)
                {
                    
                }

                
                cnt = db.Table<Track>().Count();
            }
        }
    }
}
