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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyMusic.ViewModels.StreamingPlaylists
{ 
    public class CreateListFromQPViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private int NumberOfTracksPerArtist = 3, counter = 0;

        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private IRepository repo = new Repository();
        private INavigationService _navigationService;

        private string _session;
        public string Session
        {
            get { return _session; }
            set { _session = value; NotifyPropertyChanged("Session"); }
        }

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

        public CollectionViewSource QpListViewSource { get; set; }

        private ObservableCollection<ContactGroup> _qpGroups;
        public ObservableCollection<ContactGroup> QpGroups
        {
            get { return _qpGroups; }
            set { _qpGroups = value; NotifyPropertyChanged("QpGroups"); }
        }

        private ObservableCollection<Track> _quickTracks;
        public ObservableCollection<Track> QuickTracks
        {
            get
            {
                return _quickTracks;
            }
            set
            {
                if (_quickTracks != value)
                {
                    _quickTracks = value;
                    NotifyPropertyChanged("QuickTracks");
                }
            }
        }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Track> ItemSelectedCommand { get; set; }
        public RelayCommand ShuffleCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        
        public CreateListFromQPViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<Track>(OnItemSelectedCommand);
            this.ShuffleCommand = new RelayCommand(OnShuffleCommand);   
            this.PlayCommand = new RelayCommand(OnPlayCommand);
            this.RefreshCommand = new RelayCommand(OnRefreshCommand);
            this.QuickTracks = repo.GetQuickPicks();
            Artists = new ObservableCollection<Artist>();
            Tracks = new ObservableCollection<Track>();
            LoadList();
        }

        private void LoadList()
        {
            QpGroups = GetContactGroups(QuickTracks);
            QpListViewSource = new CollectionViewSource();
            QpListViewSource.IsSourceGrouped = true;
            QpListViewSource.Source = QpGroups;
            QpListViewSource.ItemsPath = new PropertyPath("Tracks");      
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {

        }

        private void OnRefreshCommand()
        {
            Artists.Clear();
            Tracks.Clear();
        }

        private void OnPlayCommand()
        {
            var t = repo.SortGSListToArray(Tracks);
            ((App)Application.Current).BkPlayer.PlayThese("gsTrackList", t);
            _navigationService.NavigateTo("NowPlaying");
        }

        private void OnShuffleCommand()
        {
            var t = repo.ShuffleGSListToArray(Tracks);
            ((App)Application.Current).BkPlayer.PlayThese("gsTrackList", t);
            _navigationService.NavigateTo("NowPlaying");
        }

        private async void OnItemSelectedCommand(Track obj)
        {
            counter++;
            if(counter > 5)
            {
                MessageDialog msgbox = new MessageDialog("Thats 5 seleted!");
                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                
                var res = await msgbox.ShowAsync();
                return;
            }
            //var batch = await repo.GetSimilarArtists(Session, obj.Artist, NumberOfTracksPerArtist);    
            var batch = await repo.GetSimilarLastFmArtists(obj.Artist, 5);
            if (batch != null)
            {
                foreach (var item in batch)
                {
                    if (!Artists.Contains(item))
                        Artists.Add(item);
                }
                await GetTracksForTheseArtists(batch);
            }
        }

        private async Task GetTracksForTheseArtists(ObservableCollection<Artist> batch)
        {           
            foreach (var item in batch)
            {
                //var trkBatch = await repo.GetDeezerArtistTracks(item.Name, NumberOfTracksPerArtist, Session);
                var trkBatch = await repo.GetSimilarLastFmTracks(item.Name, 3, Session);
                if (trkBatch != null)
                {
                    foreach (var tr in trkBatch)
                    {
                        tr.GSSessionKey = Session;
                        Tracks.Add(tr);
                    }
                }
            }
        }

        private ObservableCollection<ContactGroup> GetContactGroups(ObservableCollection<Track> collection)    // method to group all tracks alphabetically
        {
            List<ContactGroup> trackGroups = new List<ContactGroup>();
            List<ContactGroup> tempGroups = new List<ContactGroup>();
            ObservableCollection<Track> allSongs = collection;     // trkView.GetTracks();
            ObservableCollection<Track> songsNotNumbers = new ObservableCollection<Track>();  // to hold songs with numbers at the start
            List<char> firstLetters = new List<char>();
            var t = allSongs.GroupBy(a => a.Name.Substring(0, 1)).Select(g => g.FirstOrDefault().Name);     // get a list of alphabetical letters that songs in collection begin with
            foreach (string item in t)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Char.IsLetter(item[i]))
                    {
                        firstLetters.Add(Char.ToUpper(item[i]));
                        break;
                    }
                }
            }
            ContactGroup tGroup = new ContactGroup();
            //foreach (Char item in firstLetters.OrderBy(a => a.ToString()).Distinct())
            foreach (Char item in alpha)
            {
                var tracksWithLetter = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList();

                var tts = new ObservableCollection<Track>(allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)));

                tGroup = new ContactGroup() { Title = item.ToString(), BackgroundColour = "SlateGray", Tracks = tts };
                tempGroups.Add(tGroup);
                foreach (var tr in tracksWithLetter)    // collect all tracks that start with a letter
                {
                    songsNotNumbers.Add(tr);
                }
            }
            ObservableCollection<Track> numberSongs = new ObservableCollection<Track>();  // for all songs that start with numbers
            foreach (var item in allSongs)
            {
                if (songsNotNumbers.Contains(item) == false)
                { numberSongs.Add(item); }
            }
            ContactGroup numbersGroup = new ContactGroup() { Title = "#", Tracks = numberSongs, BackgroundColour = "SlateGray" };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                if (item.Tracks.Count() == 0) { item.BackgroundColour = "Gray"; }
                trackGroups.Add(item);
            }
            ContactGroup dots = new ContactGroup() { Title = "...", BackgroundColour = "Gray" };    // last group title to add
            trackGroups.Add(dots);

            return new ObservableCollection<ContactGroup>(trackGroups);
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
