using MyMusic.Common;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    //public class ContactGroup
    //{
    //    public string Title { get; set; }
    //    public List<TrackViewModel> Tracks { get; set; }
    //}

    public sealed partial class ShowAllTracks : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private TracksViewModel trkView = new TracksViewModel();
        private string albumId = "", genreId = "";
        //private bool editMode = false, InBinList = false;

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
         
            if (para.ToString().Contains("album"))
            {
                albumId = (para.ToString()).Split(',')[0];
                lstTracksByGenre.DataContext = trkView.GetTracksByAlbum(albumId);
                
            }
            else if (para.ToString().Contains("genre"))
            {
                genreId = (para.ToString()).Split(',')[0];
                lstTracksByGenre.DataContext = trkView.GetTracksByGenre(genreId);
                
            }                 
        }

        private void Track_ItemClick(object sender, ItemClickEventArgs e)   // to play all tracks in this album
        {
            var itemId = ((TrackViewModel)e.ClickedItem).TrackId;
            string albId = "albTracksFromThisOn," + itemId.ToString() + "," + albumId;
            if (!Frame.Navigate(typeof(NowPlaying), albId))
            {
                Debug.WriteLine("navigation failed from main to collection ");
            } 
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)  // shuffle all
        {
            string id = "";
            if (!string.IsNullOrEmpty(albumId))
            {
                id = "shuffleThese," + albumId + ",album";
            }
            else if (!string.IsNullOrEmpty(genreId))
            {
                id = "shuffleThese," + genreId + ",genre";
            }
            this.Frame.Navigate(typeof(NowPlaying), id);
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
