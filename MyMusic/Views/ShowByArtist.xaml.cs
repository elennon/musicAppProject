using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
    public class ArtistContactGroup
    {
        public string Title { get; set; }
        public List<ArtistViewModel> Artists { get; set; }
    }
    public sealed partial class ShowByArtist : Page
    {
        private NavigationHelper navigationHelper;
        private ArtistsViewModel artView = new ArtistsViewModel();
        private TracksViewModel trkView = new TracksViewModel();

        public ShowByArtist()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            //lstArtistTracks.SelectedIndex = -1;
            //lstArtistTracks.DataContext = artView.GetArtists();

            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            listViewSource.Source = GetContactGroups(artView.GetArtists());
            listViewSource.ItemsPath = new PropertyPath("Artists");
            lstViewDetail.ItemsSource = listViewSource.View;
            lstViewSummary.ItemsSource = listViewSource.View.CollectionGroups;
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
            Image playPic = (Image)templateRoot.FindName("imgPlay");

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
            TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtName");

            nameTextBlock.Text = artist.Name;    // adds song name
            nameTextBlock.Tag = artist.ArtistId;
            nameTextBlock.Opacity = 1;

            args.RegisterUpdateCallback(ShowPics);  // show artist next
        }

        private void ShowPics(ListViewBase sender, ContainerContentChangingEventArgs args)     // phase 3 shows image                            
        {
            if (args.Phase != 2)
            {
                throw new Exception("Not in phase 2.");
            }

            ArtistViewModel artist = (ArtistViewModel)args.Item;
            SelectorItem itemContainer = (SelectorItem)args.ItemContainer;
            StackPanel templateRoot = (StackPanel)itemContainer.ContentTemplateRoot;
            Image _imgSongPic = (Image)templateRoot.FindName("imgPlay");

            _imgSongPic.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/_play3.png"));
            _imgSongPic.Tag = artist.ArtistId;
            _imgSongPic.Opacity = 1;
        }

        #endregion


        private List<ArtistContactGroup> GetContactGroups(ObservableCollection<ArtistViewModel> collection)    // method to group all tracks alphabetically
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
                tGroup = new ArtistContactGroup() { Title = item.ToString(), Artists = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
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
            ArtistContactGroup numbersGroup = new ArtistContactGroup() { Title = "#", Artists = numberSongs.ToList() };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                trackGroups.Add(item);
            }

            return trackGroups;
        }

        private void Border_Tapped2(object sender, TappedRoutedEventArgs e)
        {
            semanticZoom.ToggleActiveView();
            //semanticZoom.IsZoomedInViewActive = false;
        }

        private void lstViewDetail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView lstView = (ListView)sender;
            string hh = lstView.SelectedValue.ToString();
            // this.Frame.Navigate(typeof(NowPlaying), GetListToPlay(Convert.ToInt32(hh)));
        }

        private void playerListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //PlayerDetails.DataContext = e.ClickedItem as Player;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image id = (Image)sender;
            string oo = id.Tag.ToString();
            string[] playThese = sortTracks(id.Tag.ToString());
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border br = (Border)sender;
            string id = ((TextBlock)br.Child).Tag.ToString();           
            this.Frame.Navigate(typeof(Albums), id);
        }

        private string[] sortTracks(string id)
        {
            ObservableCollection<TrackViewModel> tracks = trkView.GetTracksByArtist(id);
            string[] trks = new string[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                trks[i] = tracks[i].TrackId.ToString() + "," + tracks[i].Artist + "," + tracks[i].Name + ",notshuffle";
            }
            return trks;
        }


        #region NavigationHelper


        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

    }
}
