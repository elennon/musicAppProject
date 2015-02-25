using GalaSoft.MvvmLight;
using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.ViewModels
{
    public class GenreViewModel : INotifyPropertyChanged
    {
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

        private string _trackCount;
        public string TrackCount
        {
            get
            {
                return _trackCount;
            }
            set
            {
                if (_trackCount != value)
                {
                    _trackCount = value;
                    NotifyPropertyChanged("TrackCount");
                }
            }
        }

        public override string ToString()
        {
            return string.Format(" Track count:  {0} ", TrackCount);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class GenreCollViewModel : ViewModelBase
    {
        private ObservableCollection<GenreViewModel> _genres;
        public ObservableCollection<GenreViewModel> Genres
        {
            get
            {
                return _genres;
            }

            set
            {
                _genres = value;
                RaisePropertyChanged("Genres");
            }
        }

        public IEnumerable<GenreViewModel> GetGenres()
        {
            _genres = new ObservableCollection<GenreViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var geners = db.Table<Genre>();
                foreach (var tr in geners)
                {
                    var gCount = db.Table<Track>().Where(a => a.GenreId == tr.GenreId).Count();
                    string tc = "No of Tracks: " + gCount.ToString();
                    var trk = new GenreViewModel()
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

    }
}
