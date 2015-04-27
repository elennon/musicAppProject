using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.Common;
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

namespace MyMusic.ViewModels.StreamingPlaylists
{ 
    public class GSMainPageViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;

        public ObservableCollection<DataGroup> Methods { get; set; }
        //public ObservableCollection<Artist> artists { get; set; }

        private ObservableCollection<Artist> _artists;
        public ObservableCollection<Artist> Artists
        {
            get
            {
                return _artists;
            }
            set
            {
                if (_artists != value)
                {
                    _artists = value;
                    NotifyPropertyChanged("Artists");
                }
            }
        }

        private string _session;
        public string Session
        {
            get { return _session; }
            set { _session = value; NotifyPropertyChanged("Session"); }
        }
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<DataGroup> MethodSelectedCommand { get; set; }
        
        public GSMainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Methods = LoadCollectionList();
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.MethodSelectedCommand = new RelayCommand<DataGroup>(OnMethodSelectedCommand);
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
                        
        }

        private async void OnMethodSelectedCommand(DataGroup obj)
        {
            switch (obj.UniqueId)
            {
                case "Method1":
                    _navigationService.NavigateTo("CreateListFromQP", Session);
                  //  Artists = await repo.GetListArtists(Session);
                    break;
                case "Method2":
                    var mix = await GetThe6040();
                    var t = repo.ShuffleGSListToArray(mix);
                    ((App)Application.Current).BkPlayer.PlayThese("gsTrackList", t);
                    _navigationService.NavigateTo("NowPlaying");
                    break;
                case "Method3":
                    var sw = await GetSineWave();
                    var tt = repo.SortGSListToArray(new ObservableCollection<Track>(sw));
                    ((App)Application.Current).BkPlayer.PlayThese("gsTrackList", tt);
                    _navigationService.NavigateTo("NowPlaying");
                    break;
                case "Method4":
                    _navigationService.NavigateTo("ViewPlaylist", "ProList");                   
                    break;
            }
            
        }

        private async Task<ObservableCollection<Track>> GetSineWave()
        {
            var take50 = repo.GetQuickPicksWithTempo().Take(50).ToList();     // first get top 50 played
            List<Track> take30 = (Mix(take50)).Take(30).ToList();   // take random 30
            List<Track> getSimilar = new List<Track>();
            foreach (var item in take50.Take(5))                // for the 5 top, get 2 similar artists
            {
                var arts = await repo.GetSimilarLastFmArtists(item.ArtistName, 2);  // then 2 tracks for each
                if (arts != null)
                {
                    foreach (var art in arts)
                    {
                        var trs = await repo.GetSimilarLastFmTracks(art.Name, 2, Session);
                        if (trs != null)
                        { getSimilar.AddRange(trs); }
                    }
                }
            }
            take30.AddRange(getSimilar);
            var half1 = new List<Track>();
            var half2 = new List<Track>();
            for (int i = 0; i < take30.Count; i++)
            {
                if (i < take30.Count / 2)
                { half1.Add(take30[i]); }
                else
                { half2.Add(take30[i]); }
                
            }
            half1 = half1.OrderByDescending(a => a.tempo).ToList();
            half2 = half2.OrderByDescending(a => a.tempo).ToList();
            half1.AddRange(half2);
            return new ObservableCollection<Track>(half1);
        }

        private async Task<ObservableCollection<Track>> GetThe6040()
        {
            var take50 = repo.GetQuickPicks().Take(50).ToList();     // first get top 50 played
            List<Track> take30 = (Mix(take50)).Take(30).ToList();   // take random 30
            List<Track> getSimilar = new List<Track>();
            foreach (var item in take50.Take(5))                // for the 5 top, get 2 similar artists
            {
                var arts = await repo.GetSimilarLastFmArtists(item.ArtistName, 2);  // then 2 tracks for each
                if (arts != null)
                {
                    foreach (var art in arts)
                    {
                        var trs = await repo.GetSimilarLastFmTracks(art.Name, 2, Session);
                        if (trs != null)
                        { getSimilar.AddRange(trs); }
                    }
                }
            }
            take30.AddRange(getSimilar);
            
            return new ObservableCollection<Track>(take30);
        }

        private List<Track> Mix(List<Track> trks)
        {
            Random rng = new Random();
            int n = trks.Count();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Track value = trks[k];
                trks[k] = trks[n];
                trks[n] = value;
            }
            return trks;
        }

        public ObservableCollection<DataGroup> LoadCollectionList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup { Title = "Relatives ", UniqueId = "Method1", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "40 / 60 Split", UniqueId = "Method2", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "the Sine Wave", UniqueId = "Method3", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "I'll have what they have", UniqueId = "Method4", ImagePath = "ms-appx:///Assets/music3.jpg" });
            return groups;
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

        public void Activate(object parameter)
        {
            Session = parameter.ToString();
        }

        public void Deactivate(object parameter)
        {
            
        }

        
    }
}
