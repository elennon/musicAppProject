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
    public class ArtistViewModel : INotifyPropertyChanged
    {
        private int _artistId;
        public int ArtistId
        {
            get
            {
                return _artistId;
            }
            set
            {
                if (_artistId != value)
                {
                    _artistId = value;
                    NotifyPropertyChanged("ArtistId");
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

        //private List<Track> _tracks;
        //public List<Track> Tracks
        //{
        //    get
        //    {
        //        return _tracks;
        //    }
        //    set
        //    {
        //        if (_tracks != value)
        //        {                   
        //            _tracks = value;
        //            NotifyPropertyChanged("Tracks");
        //        }
        //    }
        //}


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

    public class ArtistsViewModel : ViewModelBase
    {
        private ObservableCollection<ArtistViewModel> _artists;
        public ObservableCollection<ArtistViewModel> Artists
        {
            get
            {
                return _artists;
            }

            set
            {
                _artists = value;
                RaisePropertyChanged("Artists");
            }
        }

        public ObservableCollection<ArtistViewModel> GetArtists()
        {
            _artists = new ObservableCollection<ArtistViewModel>();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var query = db.Table<Artist>().OrderBy(c => c.Name);
                foreach (var ar in query)
                {
                    var art = new ArtistViewModel()
                    {                        
                        Name = ar.Name,
                        ArtistId = ar.ArtistId
                    };
                    _artists.Add(art);
                }
            }
            return _artists;
        }

        
    }
}
