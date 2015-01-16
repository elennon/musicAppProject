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
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MyMusic.Views
{
  
    public sealed partial class Albums : Page
    {
        private NavigationHelper navigationHelper;
        private AlbumsViewModel albView = new AlbumsViewModel();
        private TracksViewModel trkView = new TracksViewModel();
        //private string albumId = "";

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        
        public Albums()
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
            int artId = Convert.ToInt32(para);
            var artistTrks = trkView.GetTracksByArtist(artId);
            albumsSection.DataContext = albView.GetAlbumsByArtist(para.ToString());
            tracksSection.DataContext = artistTrks;
            Hub.DataContext = artistTrks.FirstOrDefault();

            albumsSection2.DataContext = albView.GetAlbumsByArtist(para.ToString());
            tracksSection2.DataContext = trkView.GetTracksByArtist(artId);

            albumsSection.Header = artistTrks.Select(a => a.Artist).FirstOrDefault();            
            tracksSection.Header = artistTrks.Select(a => a.Artist).FirstOrDefault();
            albumsSection2.Header = artistTrks.Select(a => a.Artist).FirstOrDefault();
            tracksSection2.Header = artistTrks.Select(a => a.Artist).FirstOrDefault();
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

        private void Track_ItemClick(object sender, ItemClickEventArgs e)   // to play all tracks in this album
        {            
            var trk = (TrackViewModel)e.ClickedItem;
            string playThese = "albTracksFromThisOn," + trk.TrackId.ToString() + "," + trk.AlbumId.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        #region bottom app buttons

        #endregion
       
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
