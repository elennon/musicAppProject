using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.Common;
using MyMusic.DAL;
using MyMusic.Models;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyMusic.ViewModels
{
    public class CollectionViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private enum Section { AllTracks, TopPlays, Artist, Album, Genre, QuickPick }
        private Section inSection;
        private Collection col;// = Collection.CollectionView;
        private string NavigateParam = "";
        private bool InBinTrackMode = true, addMode = true, InShowBinned = false;

        private List<Track> tempList = new List<Track>();
       
        private IRepository repo = new Repository();
        private INavigationService _navigationService;

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

        private ObservableCollection<Track> _topTracks;
        public ObservableCollection<Track> TopTracks
        {
            get
            {
                return _topTracks;
            }
            set
            {
                if (_topTracks != value)
                {
                    _topTracks = value;
                    NotifyPropertyChanged("TopTracks");
                }
            }
        }

        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get
            {
                return _genres;
            }
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    NotifyPropertyChanged("Genres");
                }
            }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
        }

        private bool _qpIsVisible = false;
        public bool qpIsVisible
        {
            get { return _qpIsVisible; }
            set { _qpIsVisible = value; NotifyPropertyChanged("qpIsVisible"); }
        }

        private string _editPic;
        public string EditPic
        {
            get { return _editPic; }
            set { _editPic = value; NotifyPropertyChanged("EditPic"); }
        }
        
        private ObservableCollection<ContactGroup> _cGroups;
        public ObservableCollection<ContactGroup> CGroups
        {
            get { return _cGroups; }
            set { _cGroups = value; NotifyPropertyChanged("CGroups"); }
        }
        private ObservableCollection<ContactGroup> _qpGroups;
        public ObservableCollection<ContactGroup> QpGroups
        {
            get { return _qpGroups; }
            set { _qpGroups = value; NotifyPropertyChanged("QpGroups"); }
        }

        public CollectionViewSource AllListViewSource { get; set; }    
        public CollectionViewSource QpListViewSource { get; set; }        
        public CollectionViewSource ArtistListViewSource { get; set; }
        public CollectionViewSource AlbumListViewSource { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<SectionsInViewChangedEventArgs> SectionsInViewChangedCommand { get; set; }
        public RelayCommand<Track> ItemSelectedCommand { get; set; }
        public RelayCommand ShuffleCommand { get; set; }
        public RelayCommand AddQpCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand<Track> DeleteSelectedCommand { get; set; }
        public RelayCommand<Track> QuickpDeleteSelectedCommand { get; set; }
        public RelayCommand ShowBinCommand { get; set; }

        public RelayCommand<Object> PlayItemTapCommand { get; set; }
        public RelayCommand<Object> ItemTapCommand { get; set; }
        
        public CollectionViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.ItemSelectedCommand = new RelayCommand<Track>(OnItemSelectedCommand);
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.SectionsInViewChangedCommand = new RelayCommand<SectionsInViewChangedEventArgs>(OnSectionsInViewChangedCommand);
            this.ShuffleCommand = new RelayCommand(OnShuffleCommand);
            this.AddQpCommand = new RelayCommand(OnAddQpCommand);
            this.EditCommand = new RelayCommand(OnEditCommand);
            this.DeleteSelectedCommand = new RelayCommand<Track>(OnDeleteSelectedCommand);
            this.QuickpDeleteSelectedCommand = new RelayCommand<Track>(OnQuickpDeleteSelectedCommand);
            this.ShowBinCommand = new RelayCommand(OnShowBinCommand);

            this.PlayItemTapCommand = new RelayCommand<Object>(OnPlayItemTapCommand);
            this.ItemTapCommand = new RelayCommand<Object>(OnItemTapCommand);
           
            this.AllTracks = repo.GetTracks();
            this.QuickTracks = repo.GetQuickPicks();
            this.TopTracks = repo.GetTopTracks();
            this.Genres = repo.GetGenres();
            LoadListViewSource();
            EditPic = "/Assets/bin.png";
        }

        #region Commands

        private void OnItemTapCommand(Object obj)       // tap artist/album to view their albums/tracks
        {
            if(obj is Artist )
            {
                string id = ((Artist)obj).ArtistId.ToString();
                _navigationService.NavigateTo("Albums", id);     
            }
            else if (obj is Album)
            {
                string id = ((Album)obj).AlbumId + ",album";
                _navigationService.NavigateTo("ShowAllTracks", id); 
            }
            else if (obj is Genre)
            {
                string id = ((Genre)obj).GenreId + ",genre";
                _navigationService.NavigateTo("ShowAllTracks", id);
            }
        }

        private void OnPlayItemTapCommand(Object obj)       // tap the headphones image to play all tracks by artist/album
        {
            if (obj is Artist)
            {
                string playThese = "artistTracks," + ((Artist)obj).ArtistId;      
                _navigationService.NavigateTo("NowPlaying", playThese);
            }
            else if (obj is Album)
            {
                string playThese = "albumTracks," + ((Album)obj).AlbumId;
                _navigationService.NavigateTo("NowPlaying", playThese);
            }
            
        }

        private void OnQuickpDeleteSelectedCommand(Track obj)
        {
            ObservableCollection<Track> t = QpGroups.Select(b => b.Tracks).Where(n => n.Contains(obj)).FirstOrDefault();
            t.Remove(obj);
            repo.OutFromQuickPick(obj.TrackId);
        }

        private void OnShowBinCommand()
        {
            if (InShowBinned == false)
            {
                AllTracks = repo.GetBinnedTracks();
                InShowBinned = true;
            }
            else
            {
                AllTracks = repo.GetTracks();
                InShowBinned = false;
            }
            CGroups = GetContactGroups(AllTracks);
            AllListViewSource.Source = CGroups; 
            AllListViewSource.ItemsPath = new PropertyPath("Tracks");
        }
        
        private async void OnDeleteSelectedCommand(Track obj)
        {
            if (InBinTrackMode)
            {
                MessageDialog msgbox = new MessageDialog("Are you sure you want " + obj.Name + " out??");
                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                msgbox.Commands.Add(new UICommand { Label = "No", Id = 1 });
                var res = await msgbox.ShowAsync();

                if ((int)res.Id == 0)
                {
                  
                    ObservableCollection<Track> t = CGroups.Select(b => b.Tracks).Where(n => n.Contains(obj)).FirstOrDefault();
                    t.Remove(obj);
                    repo.BinThis(obj.TrackId);
                }
                if ((int)res.Id == 1)
                {
                    return;
                }
            }
            else
            {                   // this to find the right collection of tracks( alphabetical group) for the track to add 
                repo.AddThisToQuickPick(obj.TrackId);
                var trs = repo.GetQuickPicks();
                var selectedTrack = trs.Where(a => a.TrackId == obj.TrackId).FirstOrDefault();
                QpGroups = GetContactGroups(trs);

                ObservableCollection<Track> trks = QpGroups.Select(b => b.Tracks).Where(n => n.Contains(selectedTrack)).FirstOrDefault();
                trks.Add(selectedTrack);
                
                ObservableCollection<Track> t = CGroups.Select(b => b.Tracks).Where(n => n.Contains(obj)).FirstOrDefault(); // get the collection for this track
                t.Remove(obj);      // temperarally remove from all tracks list after add to quick picks
               
               
            }
        }

        private void OnEditCommand()
        {
            InBinTrackMode = true;
            EditPic = "/Assets/bin.png";
            if (inSection == Section.AllTracks)
            {
                IsVisible = !IsVisible;
            }
            else if (inSection == Section.QuickPick)
            {
                IsVisible = !IsVisible;
            }
        }

        private void OnAddQpCommand()       // in all track section and adding tracks to quick picks
        {
            InBinTrackMode = false;
            EditPic = "/Assets/tickk.png";
            if (IsVisible) IsVisible = false;
            else IsVisible = true;

            if (addMode)                // when add clicked, show all less qp (reset all tracks)
            { 
                AllTracks = repo.GetTracksLessQpicks(); 
                addMode = false; 
            }        
            else                                                                         
            {
                AllTracks = repo.GetTracks();        // when unclicked show all 
                addMode = true;
                QuickTracks = repo.GetQuickPicks();     // reset quick picks
            }

            CGroups = GetContactGroups(AllTracks);              
            AllListViewSource.Source = CGroups; // GetContactGroups(AllTracks);
            AllListViewSource.ItemsPath = new PropertyPath("Tracks");

            QpGroups = GetContactGroups(QuickTracks);
            QpListViewSource.Source = QpGroups;
            QpListViewSource.ItemsPath = new PropertyPath("Tracks");             
        }

        private void OnShuffleCommand()
        {
            switch (inSection)
            {
                case Section.AllTracks:
                    _navigationService.NavigateTo("NowPlaying", "shuffleAll");
                    break;
                case Section.TopPlays:
                    string tP = "shuffleThese," + "0,topplay";
                    _navigationService.NavigateTo("NowPlaying", tP);
                    break;               
                case Section.QuickPick:
                    string qP = "shuffleThese," + "0,qPick";
                    _navigationService.NavigateTo("NowPlaying", qP);
                    break;
            }
        }

        private void OnSectionsInViewChangedCommand(SectionsInViewChangedEventArgs obj)
        {
            col = Collection.CollectionView;
            if (col.collHub.SectionsInView[0] == col.allTracksSec)
            {
                inSection = Section.AllTracks;
                setAppBarAllTracks();               
            }
            else if (col.collHub.SectionsInView[0] == col.topPlaysSec)
            {
                inSection = Section.TopPlays;
                setAppBarTopTracks();               
            }
            else if (col.collHub.SectionsInView[0] == col.artistSec)
            {
                inSection = Section.Artist;
                col.appBarr.Visibility = Visibility.Collapsed;
            }
            else if (col.collHub.SectionsInView[0] == col.albumSec)
            {
                inSection = Section.Album;
                col.appBarr.Visibility = Visibility.Collapsed;
            }
            else if (col.collHub.SectionsInView[0] == col.genreSec)
            {
                inSection = Section.Genre;
                setAppBarGenre(); 
            }
            else if (col.collHub.SectionsInView[0] == col.qpSec)
            {
                inSection = Section.QuickPick;
                setAppBarQuickPicks();                
            }
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
            col = Collection.CollectionView;
            switch (NavigateParam)
            {
                case "All Tracks":
                    inSection = Section.AllTracks;
                    setAppBarAllTracks();
                    col.collHub.ScrollToSection(col.allTracksSec);
                    break;
                case "Top Tracks":
                    inSection = Section.TopPlays;
                    setAppBarTopTracks();
                    col.collHub.ScrollToSection(col.topPlaysSec);
                    break;
                case "Artist":
                    inSection = Section.Artist;
                    col.appBarr.Visibility = Visibility.Collapsed;
                    col.collHub.ScrollToSection(col.artistSec);
                    break;
                case "Album":
                    inSection = Section.Album;
                    col.appBarr.Visibility = Visibility.Collapsed;
                    col.collHub.ScrollToSection(col.albumSec);
                    break;
                case "Genre":
                    inSection = Section.Genre;
                    setAppBarGenre();
                    col.collHub.ScrollToSection(col.genreSec);
                    break;
                case "QuickPick":
                    inSection = Section.QuickPick;                    
                    setAppBarQuickPicks();
                    col.collHub.ScrollToSection(col.qpSec);
                    break;
            }
        }

        private void OnItemSelectedCommand(Track tr)
        {
            if (!IsVisible && !qpIsVisible)     // if in edit mode let it go to delete command, not play
            {

                var itemId = tr.TrackId;
                string playThese = "allTracks," + itemId.ToString();
                _navigationService.NavigateTo("NowPlaying", playThese);
            }
        }

        #endregion 

        #region set app bar buttons

        private void setAppBarGenre()
        {
            col.appBarr.Visibility = Visibility.Collapsed;           
        }

        private void setAppBarQuickPicks()
        {
            col.appBarr.Visibility = Visibility.Visible;
            col.edit.Visibility = Visibility.Visible;
            col.showBinned.Visibility = Visibility.Visible;
            col.shuffle.Visibility = Visibility.Visible;
            col.addToQp.Visibility = Visibility.Collapsed;
        }

        private void setAppBarTopTracks()
        {
            col.appBarr.Visibility = Visibility.Visible;
            col.edit.Visibility = Visibility.Collapsed;
            col.showBinned.Visibility = Visibility.Collapsed;
            col.shuffle.Visibility = Visibility.Visible;
            col.addToQp.Visibility = Visibility.Collapsed;
        }

        private void setAppBarAllTracks()
        {
            col.appBarr.Visibility = Visibility.Visible;
            col.edit.Visibility = Visibility.Visible;
            col.showBinned.Visibility = Visibility.Visible;
            col.shuffle.Visibility = Visibility.Visible;
            col.addToQp.Visibility = Visibility.Visible;
        }

        #endregion
        
        private void LoadListViewSource()
        {
            CGroups = GetContactGroups(AllTracks);  // adding CGroups as observable collection as the data source to be able to bind adding or deleting from list i.e. bin tracks
            AllListViewSource = new CollectionViewSource();
            AllListViewSource.IsSourceGrouped = true;
            AllListViewSource.Source = CGroups; 
            AllListViewSource.ItemsPath = new PropertyPath("Tracks");

            ArtistListViewSource = new CollectionViewSource();
            ArtistListViewSource.IsSourceGrouped = true;
            ArtistListViewSource.Source = GetArtistContactGroups(repo.GetArtists());
            ArtistListViewSource.ItemsPath = new PropertyPath("Artists");

            AlbumListViewSource = new CollectionViewSource();
            AlbumListViewSource.IsSourceGrouped = true;
            AlbumListViewSource.Source = GetAlbumGroups(repo.GetAlbums());
            AlbumListViewSource.ItemsPath = new PropertyPath("Albums");

            QpGroups = GetContactGroups(QuickTracks);
            QpListViewSource = new CollectionViewSource();
            QpListViewSource.IsSourceGrouped = true;
            QpListViewSource.Source = QpGroups;
            QpListViewSource.ItemsPath = new PropertyPath("Tracks");           
        }

       
      
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged ;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region methods for adding to groups for semantic zoom

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

        private List<ArtistContactGroup> GetArtistContactGroups(ObservableCollection<Artist> collection)    // method to group all tracks alphabetically
        {
            List<ArtistContactGroup> trackGroups = new List<ArtistContactGroup>();
            List<ArtistContactGroup> tempGroups = new List<ArtistContactGroup>();
            ObservableCollection<Artist> allSongs = collection;     // trkView.GetTracks();
            ObservableCollection<Artist> songsNotNumbers = new ObservableCollection<Artist>();  // to hold songs with numbers at the start
            List<char> firstLetters = new List<char>();
            var t = allSongs.GroupBy(a => a.Name.Substring(0, 1)).Select(g => g.FirstOrDefault().Name);     // get a list of alphabetical letters that songs in collection begin with
            foreach (string item in t)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Char.IsLetter(item[i]))
                    {
                        firstLetters.Add(Char.ToUpper(item[i]));
                        break;
                    }
                }
            }
            ArtistContactGroup tGroup = new ArtistContactGroup();
            foreach (Char item in firstLetters.OrderBy(a => a.ToString()).Distinct())
            {
                var tracksWithLetter = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList();
                tGroup = new ArtistContactGroup() { Title = item.ToString(), BackgroundColour = "SlateGray", Artists = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
                tempGroups.Add(tGroup);
                foreach (var tr in tracksWithLetter)    // collect all tracks that start with a letter
                {
                    songsNotNumbers.Add(tr);
                }
            }
            ObservableCollection<Artist> numberSongs = new ObservableCollection<Artist>();  // for all artists that start with numbers
            foreach (var item in allSongs)
            {
                if (songsNotNumbers.Contains(item) == false)
                { numberSongs.Add(item); }
            }
            ArtistContactGroup numbersGroup = new ArtistContactGroup() { Title = "#", Artists = numberSongs.ToList(), BackgroundColour = "SlateGray" };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                trackGroups.Add(item);
            }

            foreach (Char item in alpha)        // alpha is a list of all letter
            {
                if (!firstLetters.Contains(item))     // all letters that don't have songs listed under, set opacity down
                {
                    ArtistContactGroup lettersToBeDimmed = new ArtistContactGroup() { Title = item.ToString(), BackgroundColour = "Gray" };
                    trackGroups.Add(lettersToBeDimmed);
                }
            }
            ArtistContactGroup dots = new ArtistContactGroup() { Title = "...", BackgroundColour = "Gray" };    // last group title to add
            trackGroups.Add(dots);

            return trackGroups;
        }

        private List<AlbumContactGroup> GetAlbumGroups(ObservableCollection<Album> collection)    // method to group all albums alphabetically
        {
            List<AlbumContactGroup> albumGroups = new List<AlbumContactGroup>();
            List<AlbumContactGroup> tempGroups = new List<AlbumContactGroup>();
            ObservableCollection<Album> allAlbums = collection;     // trkView.GetTracks();
            ObservableCollection<Album> songsNotNumbers = new ObservableCollection<Album>();  // to hold songs with numbers at the start
            List<char> firstLetters = new List<char>();
            var t = allAlbums.GroupBy(a => a.Name.Substring(0, 1)).Select(g => g.FirstOrDefault().Name);     // get a list of alphabetical letters that songs in collection begin with
            foreach (string item in t)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Char.IsLetter(item[i]))
                    {
                        firstLetters.Add(Char.ToUpper(item[i]));
                        break;
                    }
                }
            }
            AlbumContactGroup tGroup = new AlbumContactGroup();
            foreach (Char item in firstLetters.OrderBy(a => a.ToString()).Distinct())
            {
                var tracksWithLetter = allAlbums.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList();
                tGroup = new AlbumContactGroup() { Title = item.ToString(), BackgroundColour = "SlateGray", Albums = allAlbums.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
                tempGroups.Add(tGroup);
                foreach (var tr in tracksWithLetter)    // collect all tracks that start with a letter
                {
                    songsNotNumbers.Add(tr);
                }
            }
            ObservableCollection<Album> numberSongs = new ObservableCollection<Album>();  // for all artists that start with numbers
            foreach (var item in allAlbums)
            {
                if (songsNotNumbers.Contains(item) == false)
                { numberSongs.Add(item); }
            }
            AlbumContactGroup numbersGroup = new AlbumContactGroup() { Title = "#", Albums = numberSongs.ToList(), BackgroundColour = "SlateGray" };
            albumGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                albumGroups.Add(item);
            }

            foreach (Char item in alpha)        // alpha is a list of all letter
            {
                if (!firstLetters.Contains(item))     // all letters that don't have songs listed under, set opacity down
                {
                    AlbumContactGroup lettersToBeDimmed = new AlbumContactGroup() { Title = item.ToString(), BackgroundColour = "Gray" };
                    albumGroups.Add(lettersToBeDimmed);
                }
            }
            AlbumContactGroup dots = new AlbumContactGroup() { Title = "...", BackgroundColour = "Gray" };    // last group title to add
            albumGroups.Add(dots);

            return albumGroups;
        }

        #endregion

        public void Activate(object parameter)
        {            
            NavigateParam = parameter.ToString();  
        }

        public void Deactivate(object parameter)
        {
            
        }

    }
}
