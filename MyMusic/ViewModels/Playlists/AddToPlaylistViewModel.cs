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
using Windows.UI.Xaml.Data;

namespace MyMusic.ViewModels
{
     
     public class AddToPlaylistViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private Playlist thisPlaylist = new Playlist();
        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private ObservableCollection<Track> _allTracks;
        public ObservableCollection<Track> AllTracks
        {
            get
            {
                return _allTracks;
            }
            set
            {
                if (_allTracks != value)
                {
                    _allTracks = value;
                    NotifyPropertyChanged("AllTracks");
                }
            }
        }
        public CollectionViewSource AllListViewSource { get; set; }
      
        private string _editPic;
        public string EditImage
        {
            get { return _editPic; }
            set { _editPic = value; NotifyPropertyChanged("EditImage"); }
        }

        private ObservableCollection<ContactGroup> _cGroups;
        public ObservableCollection<ContactGroup> CGroups
        {
            get { return _cGroups; }
            set { _cGroups = value; NotifyPropertyChanged("CGroups"); }
        }
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        
        public RelayCommand<Track> AddCommand { get; set; }
        public RelayCommand DoneAddingCommand { get; set; }


        public AddToPlaylistViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);           
            this.AddCommand = new RelayCommand<Track>(OnAddCommand);
            this.DoneAddingCommand = new RelayCommand(OnDoneAddingCommand);
        }

        private void OnDoneAddingCommand()
        {
            _navigationService.NavigateTo("SavedPlaylists");
        }

        private void LoadList()
        {
            CGroups = GetContactGroups(AllTracks);  // adding CGroups as observable collection as the data source to be able to bind adding or deleting from list i.e. bin tracks
            AllListViewSource = new CollectionViewSource();
            AllListViewSource.IsSourceGrouped = true;
            AllListViewSource.Source = CGroups;
            AllListViewSource.ItemsPath = new PropertyPath("Tracks");
        }

        private void OnAddCommand(Track obj)
        {
            if(EditImage.Contains("tickk"))
            {
                repo.AddToPlaylist(thisPlaylist.PlaylistId, obj.TrackId);
                ObservableCollection<Track> t = CGroups.Select(b => b.Tracks).Where(n => n.Contains(obj)).FirstOrDefault(); // get the collection for this track
                t.Remove(obj);      
            }
            else
            {
                repo.RemoveFromPlaylist(thisPlaylist.PlaylistId, obj.TrackId);
                ObservableCollection<Track> t = CGroups.Select(b => b.Tracks).Where(n => n.Contains(obj)).FirstOrDefault(); // get the collection for this track
                t.Remove(obj); 
            }
               
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
            
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
            if(parameter != null)
            {
                string para = (string)parameter;
                var playlistId = Convert.ToInt32(para.Split(',')[1]);
                thisPlaylist = repo.GetThisPlaylist(playlistId);
                if (para.Split(',')[0] == "add") 
                { 
                    EditImage = "/Assets/tickk.png";
                    AllTracks = repo.GetTracksLessThisPlaylist(thisPlaylist);
                }
                else
                { 
                    EditImage = "/Assets/bin.png";
                    AllTracks = repo.GetPlaylistTracks(thisPlaylist);
                }
                              
                LoadList();
            }            
        }

        public void Deactivate(object parameter)
        {
            
        }

        
    }
}
