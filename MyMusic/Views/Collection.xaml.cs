using MyMusic.Common;
using MyMusic.HelperClasses;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    public class ContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public List<TrackViewModel> Tracks { get; set; }
    }
    public class ArtistContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public List<ArtistViewModel> Artists { get; set; }
    }
    public class AlbumContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
    }

    public sealed partial class Collection : Page
    {
        private TracksViewModel trkView = new TracksViewModel();
        private RadioStreamsViewModel rdoView = new RadioStreamsViewModel();
        private ArtistsViewModel artView = new ArtistsViewModel();
        private AlbumsViewModel albView = new AlbumsViewModel();
        private GenreCollViewModel genView = new GenreCollViewModel();

        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private bool editMode = false, InBinList = false;
        private ListView lstDetails = new ListView();
        private GridView grdSummary = new GridView();
        private ListView ArtLstDetails = new ListView();
        private GridView ArtGrdSummary = new GridView();
        private ListView AlbLstDetails = new ListView();
        private GridView AlbGrdSummary = new GridView();

        private AutoResetEvent SererInitialized;
        private readonly NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public Collection()
        {
            this.InitializeComponent();
            SererInitialized = new AutoResetEvent(false);

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
 //           Logger.GetLogger().logChannel.LogMessage("Collection page, in nav to");
            TopPlaySection.DataContext = trkView.GetTopTracks();
            GenreSection.DataContext = genView.GetGenres();

            string arg = (string)e.Parameter;
            switch (arg)
            {
                case "All Tracks":                   
                    appBarEdit.Visibility = Visibility.Visible;
                    appBarShowAll.Visibility = Visibility.Visible;
                    appBarShowBinned.Visibility = Visibility.Visible;
                    appBarShuffle.Visibility = Visibility.Visible;
                    CollectionHub.ScrollToSection(AllTracksSection);
                    break;
                case "TopPlay":
                    appBarEdit.Visibility = Visibility.Collapsed;
                    appBarShowAll.Visibility = Visibility.Collapsed;
                    appBarShowBinned.Visibility = Visibility.Collapsed;
                    appBarShuffle.Visibility = Visibility.Visible;
                    CollectionHub.ScrollToSection(TopPlaySection);
                    break;
                case "Artist":
                    appBar.Visibility = Visibility.Collapsed;
                    CollectionHub.ScrollToSection(ArtistSection);
                    break;
                case "Album":
                    appBar.Visibility = Visibility.Collapsed;
                    CollectionHub.ScrollToSection(AlbumSection);
                    break;
                case "Genre":
                    appBarEdit.Visibility = Visibility.Collapsed;
                    appBarShowAll.Visibility = Visibility.Collapsed;
                    appBarShowBinned.Visibility = Visibility.Collapsed;
                    appBarShuffle.Visibility = Visibility.Visible;
                    CollectionHub.ScrollToSection(GenreSection);
                    break; 
            }   
        }

        #region All Tracks List

        private void lstViewDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((TrackViewModel)e.ClickedItem).OrderNo;
            string playThese = "allTracks," + itemId.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        private void Track_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border brd = (Border)sender;
            TextBlock nameTextBlock = (TextBlock)brd.FindName("txtName");
            string playThese = "allTracks," + nameTextBlock.Tag.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        private async void deleteIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image del = (Image)sender;
            string inOrOut = del.Tag.ToString().Split(',')[2];
            if (inOrOut == "out")
            {
                string trkName = (del.Tag.ToString()).Split(',')[0];    // track name is 1st part of the tag
                MessageDialog msgbox = new MessageDialog("Are you sure you want " + trkName + " out??");

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                msgbox.Commands.Add(new UICommand { Label = "No", Id = 1 });
                var res = await msgbox.ShowAsync();

                if ((int)res.Id == 0)
                {
                    int id = Convert.ToInt32((del.Tag.ToString()).Split(',')[1]);   // track id is 2nd part of tag 
                    trkView.BinThis(id);
                    LoadList();
                }
                if ((int)res.Id == 1)
                {
                    return;
                }
            }
            else if (inOrOut == "in")
            {
                int id = Convert.ToInt32((del.Tag.ToString()).Split(',')[1]);   // track id is 2nd part of tag 
                trkView.BackIn(id);
                LoadBinList();
            }

        }
     
        public void AllTracksSemanticZoom_Loaded(object sender, RoutedEventArgs e)
        {
            SemanticZoom sm = (SemanticZoom)sender;
            lstDetails = (ListView)sm.FindName("lstViewDetail");
            grdSummary = (GridView)sm.FindName("lstViewSummary");
            LoadList();
        }

        private void LoadList()
        {
            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            listViewSource.Source = GetContactGroups(trkView.GetTracks());
            listViewSource.ItemsPath = new PropertyPath("Tracks");

            lstDetails.ItemsSource = listViewSource.View;
            grdSummary.ItemsSource = listViewSource.View.CollectionGroups;
        }

        private void LoadBinList()
        {
            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            listViewSource.Source = GetContactGroups(trkView.GetBinnedTracks());
            listViewSource.ItemsPath = new PropertyPath("Tracks");

            lstDetails.ItemsSource = listViewSource.View;
            grdSummary.ItemsSource = listViewSource.View.CollectionGroups;
        }

        #region fill listview incrementally

        private void ItemListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;

            if (args.Phase != 0)
            {
                throw new Exception("Not in phase 0.");
            }

            Grid templateRoot = (Grid)args.ItemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtName");
            TextBlock artistTextBlock = (TextBlock)templateRoot.FindName("txtArtist");
            Image songPic = (Image)templateRoot.FindName("imgSongPic");

            nameTextBlock.Opacity = 0;
            artistTextBlock.Opacity = 0;
            songPic.Opacity = 0;

            args.RegisterUpdateCallback(ShowSongName);  //  show song titles first
        }

        private void ShowSongName(ListViewBase sender, ContainerContentChangingEventArgs args)    // phase 1 shows title                            
        {
            if (args.Phase != 1)
            {
                throw new Exception("Not in phase 1.");
            }

            TrackViewModel track = (TrackViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            Grid templateRoot = (Grid)itemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtName");

            nameTextBlock.Text = track.Name;    // adds song name 
            nameTextBlock.Tag = track.OrderNo;
            nameTextBlock.Opacity = 1;

            args.RegisterUpdateCallback(ShowArtist);  // show artist next
        }

        private void ShowArtist(ListViewBase sender, ContainerContentChangingEventArgs args)    // phase 2 shows artist                           
        {
            if (args.Phase != 2)
            {
                throw new Exception("Not in phase 2.");
            }
            TrackViewModel track = (TrackViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            Grid templateRoot = (Grid)itemContainer.ContentTemplateRoot;
            TextBlock artistTextBlock = (TextBlock)templateRoot.FindName("txtArtist");

            artistTextBlock.Text = track.Artist;
            artistTextBlock.Opacity = 1;

            args.RegisterUpdateCallback(ShowPics);   // show pics next
        }

        private void ShowPics(ListViewBase sender, ContainerContentChangingEventArgs args)     // phase 3 shows image                            
        {
            if (args.Phase != 3)
            {
                throw new Exception("Not in phase 3.");
            }

            TrackViewModel track = (TrackViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            Grid templateRoot = (Grid)itemContainer.ContentTemplateRoot;
            Image _imgSongPic = (Image)templateRoot.FindName("imgSongPic");
            Image _deleteIcon = (Image)templateRoot.FindName("deleteIcon");



            if (string.IsNullOrEmpty(track.ImageUri) == false)      //  if there is no pic, show default
            {
                _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(track.ImageUri));
            }
            else
            {
                _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/radio672.png"));
            }
            _imgSongPic.Opacity = 1;
            if (editMode)
            {
                string tag = track.Name + "," + track.TrackId.ToString() + ",out";
                _deleteIcon.Tag = tag;
                _deleteIcon.Visibility = Visibility.Visible;
            }
            else if (InBinList)
            {
                string tag = track.Name + "," + track.TrackId.ToString() + ",in";
                _deleteIcon.Tag = tag;
                _deleteIcon.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/undo3.png"));
                _deleteIcon.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private List<ContactGroup> GetContactGroups(ObservableCollection<TrackViewModel> collection)    // method to group all tracks alphabetically
        {
            List<ContactGroup> trackGroups = new List<ContactGroup>();
            List<ContactGroup> tempGroups = new List<ContactGroup>();
            ObservableCollection<TrackViewModel> allSongs = collection;     // trkView.GetTracks();
            ObservableCollection<TrackViewModel> songsNotNumbers = new ObservableCollection<TrackViewModel>();  // to hold songs with numbers at the start
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
            foreach (Char item in firstLetters.OrderBy(a => a.ToString()).Distinct())
            {
                var tracksWithLetter = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList();
                tGroup = new ContactGroup() { Title = item.ToString(), BackgroundColour = "SlateGray", Tracks = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
                tempGroups.Add(tGroup);
                foreach (var tr in tracksWithLetter)    // collect all tracks that start with a letter
                {
                    songsNotNumbers.Add(tr);
                }
            }
            ObservableCollection<TrackViewModel> numberSongs = new ObservableCollection<TrackViewModel>();  // for all songs that start with numbers
            foreach (var item in allSongs)
            {
                if (songsNotNumbers.Contains(item) == false)
                { numberSongs.Add(item); }
            }
            ContactGroup numbersGroup = new ContactGroup() { Title = "#", Tracks = numberSongs.ToList(), BackgroundColour = "SlateGray" };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                trackGroups.Add(item);
            }

            foreach (Char item in alpha)        // alpha is a list of all letter
            {
                if(!firstLetters.Contains(item))     // all letters that don't have songs listed under, set opacity down
                {
                    ContactGroup lettersToBeDimmed = new ContactGroup() { Title = item.ToString(), BackgroundColour = "Gray" };
                    trackGroups.Add(lettersToBeDimmed);
                }
            }
            ContactGroup dots = new ContactGroup() { Title = "...", BackgroundColour = "Gray" };    // last group title to add
            trackGroups.Add(dots);

            return trackGroups;
        }

        #endregion

        #region Artist List

        private void Artist_Loaded(object sender, RoutedEventArgs e)
        {
            SemanticZoom sm = (SemanticZoom)sender;
            ArtLstDetails = (ListView)sm.FindName("ArtlstViewDetail");
            ArtGrdSummary = (GridView)sm.FindName("ArtGrdViewSummary");
            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            listViewSource.Source = GetArtistContactGroups(artView.GetArtists());
            listViewSource.ItemsPath = new PropertyPath("Artists");

            ArtLstDetails.ItemsSource = listViewSource.View;
            ArtGrdSummary.ItemsSource = listViewSource.View.CollectionGroups;

        }

        #region fill listview incrementally

        private void ArtistListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;

            if (args.Phase != 0)
            {
                throw new Exception("Not in phase 0.");
            }

            StackPanel templateRoot = (StackPanel)args.ItemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtArtistName");
            Image playPic = (Image)templateRoot.FindName("imgPlayArtist");

            nameTextBlock.Opacity = 0;
            playPic.Opacity = 0;

            args.RegisterUpdateCallback(ShowArtistName);  //  show song titles first
        }

        private void ShowArtistName(ListViewBase sender, ContainerContentChangingEventArgs args)    // phase 1 shows title                            
        {
            if (args.Phase != 1)
            {
                throw new Exception("Not in phase 1.");
            }

            ArtistViewModel artist = (ArtistViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtArtistName");

            nameTextBlock.Text = artist.Name;    // adds song name
            nameTextBlock.Tag = artist.ArtistId;
            nameTextBlock.Opacity = 1;

            args.RegisterUpdateCallback(ShowArtPics);  // show artist next
        }

        private void ShowArtPics(ListViewBase sender, ContainerContentChangingEventArgs args)     // phase 3 shows image                            
        {
            if (args.Phase != 2)
            {
                throw new Exception("Not in phase 2.");
            }

            ArtistViewModel artist = (ArtistViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;
            Image _imgSongPic = (Image)templateRoot.FindName("imgPlayArtist");

            _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/plaay.png"));
            _imgSongPic.Tag = artist.ArtistId;
            _imgSongPic.Opacity = 1;
        }

        #endregion

        private List<ArtistContactGroup> GetArtistContactGroups(ObservableCollection<ArtistViewModel> collection)    // method to group all tracks alphabetically
        {
            List<ArtistContactGroup> trackGroups = new List<ArtistContactGroup>();
            List<ArtistContactGroup> tempGroups = new List<ArtistContactGroup>();
            ObservableCollection<ArtistViewModel> allSongs = collection;     // trkView.GetTracks();
            ObservableCollection<ArtistViewModel> songsNotNumbers = new ObservableCollection<ArtistViewModel>();  // to hold songs with numbers at the start
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
            ObservableCollection<ArtistViewModel> numberSongs = new ObservableCollection<ArtistViewModel>();  // for all artists that start with numbers
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

        private void ArtistBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border br = (Border)sender;
            string id = ((TextBlock)br.Child).Tag.ToString();
            this.Frame.Navigate(typeof(Albums), id);
        }

        private void Play_Tapped_Artist(object sender, TappedRoutedEventArgs e)
        {
            Image id = (Image)sender;
            string playThese = "artistTracks," + id.Tag.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        #endregion 

        #region Albums List

        private void AlbumSemanticZoom_Loaded(object sender, RoutedEventArgs e)
        {
            SemanticZoom sm = (SemanticZoom)sender;
            AlbLstDetails = (ListView)sm.FindName("albumLstViewDetail");
            AlbGrdSummary = (GridView)sm.FindName("albumGrdViewSummary");
            LoadAlbums(null);
        }

        private void LoadAlbums(string id)
        {
            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            if(string.IsNullOrEmpty(id) == true)
            {
                listViewSource.Source = GetAlbumGroups(albView.GetAlbums());
            }
            else
            {
                listViewSource.Source = GetAlbumGroups(albView.GetAlbumsByArtist(id)); 
            }
            listViewSource.ItemsPath = new PropertyPath("Albums");

            AlbLstDetails.ItemsSource = listViewSource.View;
            AlbGrdSummary.ItemsSource = listViewSource.View.CollectionGroups;
        }

        #region fill listview incrementally

        private void AlbumListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;

            if (args.Phase != 0)
            {
                throw new Exception("Not in phase 0.");
            }

            StackPanel templateRoot = (StackPanel)args.ItemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtAlbumName");
            Image playPic = (Image)templateRoot.FindName("imgPlayAlbum");

            nameTextBlock.Opacity = 0;
            playPic.Opacity = 0;

            args.RegisterUpdateCallback(ShowAlbumName);  //  show song titles first
        }

        private void ShowAlbumName(ListViewBase sender, ContainerContentChangingEventArgs args)    // phase 1 shows title                            
        {
            if (args.Phase != 1)
            {
                throw new Exception("Not in phase 1.");
            }

            AlbumViewModel album = (AlbumViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtAlbumName");

            nameTextBlock.Text = album.Name;    // adds song name
            nameTextBlock.Tag = album.AlbumId;
            nameTextBlock.Opacity = 1;

            args.RegisterUpdateCallback(ShowAlbumPics);  // show artist next
        }

        private void ShowAlbumPics(ListViewBase sender, ContainerContentChangingEventArgs args)     // phase 3 shows image                            
        {
            if (args.Phase != 2)
            {
                throw new Exception("Not in phase 2.");
            }

            AlbumViewModel album = (AlbumViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;
            Image _imgSongPic = (Image)templateRoot.FindName("imgPlayAlbum");

            _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/plaay.png"));
            _imgSongPic.Tag = album.AlbumId;
            _imgSongPic.Opacity = 1;
        }

        #endregion

        private List<AlbumContactGroup> GetAlbumGroups(ObservableCollection<AlbumViewModel> collection)    // method to group all albums alphabetically
        {
            List<AlbumContactGroup> albumGroups = new List<AlbumContactGroup>();
            List<AlbumContactGroup> tempGroups = new List<AlbumContactGroup>();
            ObservableCollection<AlbumViewModel> allAlbums = collection;     // trkView.GetTracks();
            ObservableCollection<AlbumViewModel> songsNotNumbers = new ObservableCollection<AlbumViewModel>();  // to hold songs with numbers at the start
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
            ObservableCollection<AlbumViewModel> numberSongs = new ObservableCollection<AlbumViewModel>();  // for all artists that start with numbers
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

        private void Album_Tapped(object sender, TappedRoutedEventArgs e)   // this for when all albums are in list. passes id of album selected to show tracks in that album
        {
            Border br = (Border)sender;
            string id = ((TextBlock)br.Child).Tag.ToString() + ",album";
            this.Frame.Navigate(typeof(ShowAllTracks), id);
        }

        private void AlbumImage_Tapped(object sender, TappedRoutedEventArgs e)   // to play all tracks in this album
        {
            Image id = (Image)sender;
            string playThese = "albumTracks," + id.Tag.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        private void showTracksInAlbum_Tapped(object sender, TappedRoutedEventArgs e)       // listbox to show for particular artist
        {
            Border br = (Border)sender;
            string id = br.Tag.ToString();
            this.Frame.Navigate(typeof(ShowAllTracks), id);
        }
       

        #endregion

        private string[] shuffleAll()
        {
            List<int> trks = new List<int>();
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            shuffled = trkView.GetShuffleTracks();
            string[] trkks = new string[shuffled.Count];
            for (int i = 0; i < shuffled.Count; i++)
            {
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
            }
            return trkks;
        }

        #region bottom app buttons

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)  // shuffle all
        {
            this.Frame.Navigate(typeof(NowPlaying), "shuffleAll");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            editMode = true;
            InBinList = false;

            LoadList();
        }
       
        private void ShowBinnedButton_Click(object sender, RoutedEventArgs e)
        {
            editMode = false;
            InBinList = true;
            LoadBinList();
        }

        private void AllTracksButton_Click(object sender, RoutedEventArgs e)
        {
            editMode = false;
            InBinList = false;
            LoadList();
        }

        #endregion

        #region NavigationHelper

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Genre_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((GenreViewModel)e.ClickedItem).GenreId;
            string gId = itemId.ToString() + ",genre";
            if (!Frame.Navigate(typeof(ShowAllTracks), gId))
            {
                Debug.WriteLine("navigation failed from main to collection ");
            }     
        }

        private void CollectionHub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            if (CollectionHub.SectionsInView[0] == AllTracksSection)
            {
                appBar.Visibility = Visibility.Visible;
                appBarEdit.Visibility = Visibility.Visible;
                appBarShowAll.Visibility = Visibility.Visible;
                appBarShowBinned.Visibility = Visibility.Visible;
                appBarShuffle.Visibility = Visibility.Visible;
            }
            else if (CollectionHub.SectionsInView[0] == TopPlaySection)
            {
                appBar.Visibility = Visibility.Visible;
                appBarEdit.Visibility = Visibility.Collapsed;
                appBarShowAll.Visibility = Visibility.Collapsed;
                appBarShowBinned.Visibility = Visibility.Collapsed;
                appBarShuffle.Visibility = Visibility.Visible;
            }
            else if (CollectionHub.SectionsInView[0] == ArtistSection)
            {
                appBar.Visibility = Visibility.Collapsed;
            }
            else if (CollectionHub.SectionsInView[0] == AlbumSection)
            {
                appBar.Visibility = Visibility.Collapsed;
            }
            else if (CollectionHub.SectionsInView[0] == GenreSection)
            {
                appBar.Visibility = Visibility.Collapsed;
            }
        }
  
    }
}


//private void ShuffleButton_Click(object sender, RoutedEventArgs e)
//{
//    string[] shuffled = shuffleAll();
//    this.Frame.Navigate(typeof(NowPlaying), "shuffle" );
//}






           
        //    UserControl cb = FindChildControl<UserControl>(AllTracksSection, "lstGG") as UserControl;
        //    var sec = Hub.Sections[0];
        //    var btn = sec.FindName("lstGG") as UserControl;
        //}
        

        

        //private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        //{
        //    int childNumber = VisualTreeHelper.GetChildrenCount(control);
        //    for (int i = 0; i < childNumber; i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(control, i);
        //        FrameworkElement fe = child as FrameworkElement;
        //        // Not a framework element or is null
        //        if (fe == null) return null;

        //        if (child is T && fe.Name == ctrlName)
        //        {
        //            // Found the control so return
        //            return child;
        //        }
        //        else
        //        {
        //            // Not found it - search children
        //            DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
        //            if (nextLevel != null)
        //                return nextLevel;
        //        }
        //    }
        //    return null;
        //}