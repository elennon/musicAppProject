using MyMusic.Common;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace MyMusic.Views
{
    public class ContactGroup
    {
        public string Title { get; set; }
        public List<TrackViewModel> Tracks { get; set; }
    }

    public sealed partial class ShowAllTracks : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private TracksViewModel trkView = new TracksViewModel();
        private string albumId = "";

        public ShowAllTracks()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }
       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            var para = e.Parameter;
            if (para != null)                       // if user clicked on an album on album page, just show tracks from that album
            {
                albumId = para.ToString();
                lstTracksInAlbum.ItemsSource = trkView.GetTracksByAlbum(para.ToString());
                semanticZoom.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                lstTracksInAlbum.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                CollectionViewSource listViewSource = new CollectionViewSource();
                listViewSource.IsSourceGrouped = true;
                listViewSource.Source = GetContactGroups(trkView.GetTracks());
                listViewSource.ItemsPath = new PropertyPath("Tracks");
                lstViewDetail.ItemsSource = listViewSource.View;
                lstViewSummary.ItemsSource = listViewSource.View.CollectionGroups;
            }  
         
        }

        #region fill listview incrementally

        private void ItemListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)                                
        {
            args.Handled = true;

            if (args.Phase != 0)
            {
                throw new Exception("Not in phase 0.");
            }

            StackPanel templateRoot = (StackPanel)args.ItemContainer.ContentTemplateRoot;              
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
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;               
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
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;                
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
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;                              
            Image _imgSongPic = (Image)templateRoot.FindName("imgSongPic");

            if (string.IsNullOrEmpty(track.ImageUri) == false)      //  if there is no pic, show default
            {
                _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(track.ImageUri));
            }
            else
            { 
                _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/radio672.png")); 
            }
            _imgSongPic.Opacity = 1;
        }

        #endregion

        private void lstAlbumTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)    // pass album id from list of track in given album
        {
            string albId = "albTracksFromThisOn," + ((ListBox)sender).SelectedIndex.ToString() + "," + albumId;
            this.Frame.Navigate(typeof(NowPlaying), albId);   
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)  // shuffle all
        {
            this.Frame.Navigate(typeof(NowPlaying), "shuffle");  
        }

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
                for (int i = 0; i < 5; i++)
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
                tGroup = new ContactGroup() { Title = item.ToString(), Tracks = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
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
            ContactGroup numbersGroup = new ContactGroup() { Title = "#", Tracks = numberSongs.ToList() };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                trackGroups.Add(item);
            }

            return trackGroups;
        }

        private void Song_Tapped(object sender, TappedRoutedEventArgs e)    // fires when all tracks diplayed, sends tracks order # to play it and all after it
        {
            StackPanel grd = (StackPanel)sender;
            TextBlock nameTextBlock = (TextBlock)grd.FindName("txtName");
            string tNumber = "allTracks," + nameTextBlock.Tag.ToString() ;
            this.Frame.Navigate(typeof(NowPlaying), tNumber ); //GetListToPlay(Convert.ToInt32(hh)
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            semanticZoom.ToggleActiveView();
            //semanticZoom.IsZoomedInViewActive = false;
        }

        #region NavigationHelper registration

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #endregion
    }
}


        //private string[] GetListToPlay(int orderNo) // orders all songs that come after selected song (+ selected) into a string[]
        //{
        //    ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
        //    var trks = (trkView.GetTracks()).Where(a => a.OrderNo >= orderNo).ToList(); // get all tracks listed after selected one
        //    string[] trkArray = new string[trks.Count];

        //    for (int i = 0; i < trks.Count; i++)
        //    {
        //        trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].Artist + "," + trks[i].Name + ",shuffle";
        //    }
        //    return trkArray;
        //}
