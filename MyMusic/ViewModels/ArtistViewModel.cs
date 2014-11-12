using System;
using System.Collections.Generic;
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
        //[Column]
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
        //            NotifyPropertyChanging("Tracks");
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
}
