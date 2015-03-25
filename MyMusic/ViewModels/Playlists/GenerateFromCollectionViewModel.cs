using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyMusic.ViewModels.Playlists
{
    public class GenerateFromCollectionViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        
        private ObservableCollection<Track> _tracks;
        public ObservableCollection<Track> Tracks
        {
            get
            {
                return _tracks;
            }
            set
            {
                if (_tracks != value)
                {
                    _tracks = value;
                    NotifyPropertyChanged("Tracks");
                }
            }
        }

        private ObservableCollection<Track> _tracks2;
        public ObservableCollection<Track> Tracks2
        {
            get
            {
                return _tracks2;
            }
            set
            {
                if (_tracks2 != value)
                {
                    _tracks2 = value;
                    NotifyPropertyChanged("Tracks2");
                }
            }
        }

        private ObservableCollection<Track> _tracks3;
        public ObservableCollection<Track> Tracks3
        {
            get
            {
                return _tracks3;
            }
            set
            {
                if (_tracks3 != value)
                {
                    _tracks3 = value;
                    NotifyPropertyChanged("Tracks3");
                }
            }
        }

        private ObservableCollection<Track> _tracks4;
        public ObservableCollection<Track> Tracks4
        {
            get
            {
                return _tracks4;
            }
            set
            {
                if (_tracks4 != value)
                {
                    _tracks4 = value;
                    NotifyPropertyChanged("Tracks4");
                }
            }
        }

        public double AddedMax { get; set; }
        private double _added;
        public double Added
        {
            get { return _added; }
            set { _added = value; NotifyPropertyChanged("Added"); }
        }

        public double EnergyMax { get; set; }
        private double _energy;
        public double Energy
        {
            get { return _energy; }
            set { _energy = value; NotifyPropertyChanged("Energy"); }
        }

        public double LivenessMax { get; set; }
        private double _liveness;
        public double Liveness
        {
            get { return _liveness; }
            set { _liveness = value; NotifyPropertyChanged("Liveness"); }
        }

        public double TempoMax { get; set; }
        private double _tempo;
        public double Tempo
        {
            get { return _tempo; }
            set { _tempo = value; NotifyPropertyChanged("Tempo"); }
        }

        public double LoudnessMax { get; set; }
        private double _loudness;
        public double Loudness
        {
            get { return _loudness; }
            set { _loudness = value; NotifyPropertyChanged("Loudness"); }
        }

        public RelayCommand<Track> ItemSelectedCommand { get; set; }
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand GenerateListCommand { get; set; }
        public RelayCommand ShuffleCommand { get; set; }

        public GenerateFromCollectionViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            var trs = repo.GetTracks().OrderByDescending(a => a.Rating).ToList();
            //Tracks = new ObservableCollection<Track>(trs);

            var trs2 = repo.GetTracks().OrderByDescending(a => a.energy).ToList();
            Tracks2 = new ObservableCollection<Track>(trs2);

            var trs3 = repo.GetTracks().OrderByDescending(a => a.liveness).ToList();
            Tracks3 = new ObservableCollection<Track>(trs3);

            var trs4 = repo.GetTracks().OrderByDescending(a => a.tempo).ToList();
            Tracks4 = new ObservableCollection<Track>(trs4);

            Added = trs.Average(a => a.Rating);
            AddedMax = trs.OrderByDescending(a => a.Rating).Select(b => b.Rating).Take(1).First();

            Energy = (trs.Average(a => a.energy)) * 10;
            EnergyMax = (trs.OrderByDescending(a => a.energy).Select(b => b.energy).Take(1).First()) * 10;

            Liveness = (trs.Average(a => a.liveness)) * 10;
            LivenessMax = (trs.OrderByDescending(a => a.liveness).Select(b => b.liveness).Take(1).First()) * 10;

            Loudness = (trs.Average(a => a.loudness)) +50;
            //LoudnessMax = (trs.OrderByDescending(a => a.loudness).Select(b => b.loudness).Take(1).First()) * -1;

            Tempo = trs.Average(a => a.tempo);
            TempoMax = trs.OrderByDescending(a => a.tempo).Select(b => b.tempo).Take(1).First();

            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<Track>(OnItemSelectedCommand);
            this.GenerateListCommand = new RelayCommand(OnGenerateListCommand);
            this.ShuffleCommand = new RelayCommand(OnShuffleCommand);
        }

        private void OnShuffleCommand()
        {
            var t = repo.ShufflePlaylist(Tracks.ToList());
            ((App)Application.Current).BkPlayer.PlayThese("genPlaylist", t);
            _navigationService.NavigateTo("NowPlaying");
        }

        private void OnGenerateListCommand()
        {
            double d = Added;
            List<Track> trks = GetBestFit(Added, Energy, Liveness, Loudness, Tempo);
            Tracks = new ObservableCollection<Track>(trks);
        }

        private List<Track> GetBestFit(double Added, double Energy, double Liveness, double Loudness, double Tempo)
        {
            List<Track> trs = new List<Track>();
            var trks = repo.GetAllTracks();
            var t = trks.Where(a => a.energy < ((Energy + Energy * 0.07)/10) && a.energy > ((Energy - Energy * 0.07)/10)).ToList();
            trs.AddRange(t);
            t = trks.Where(a => a.liveness < ((Liveness * 1.07) / 10) && a.liveness > ((Liveness - Liveness * 0.07) / 10)).ToList();
            trs.AddRange(t);
            t = trks.Where(a => a.tempo < (Tempo * 1.07) && a.tempo > (Tempo - Tempo * 0.07)).ToList();
            trs.AddRange(t);  
            var f = (Loudness - 50) + (((Loudness - 50) * -1) * 0.07);
            var c = (Loudness - 50) - (((Loudness - 50) * -1) * 0.07);
            t = trks.Where(a => a.loudness < f && a.loudness > c).ToList();
            trs.AddRange(t);

            return trs;
        }



        private void OnItemSelectedCommand(Track obj)
        {
            throw new NotImplementedException();
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {}
  
        public void Activate(object parameter)
        {}
                   
        public void Deactivate(object parameter)
        {}
      
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
}
