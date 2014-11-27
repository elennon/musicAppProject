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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Albums : Page
    {
        private NavigationHelper navigationHelper;
        private AlbumsViewModel albView = new AlbumsViewModel();
        private TracksViewModel trkView = new TracksViewModel();
        
        public Albums()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            var para = e.Parameter;
            if(para != null)
            {
                lstAlbums.DataContext = albView.GetAlbumsByArtist(para.ToString());
            }
            else { lstAlbums.DataContext = albView.GetAlbums(); }
           
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image id = (Image)sender;
            string[] playThese = sortTracks(id.Tag.ToString());
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border br = (Border)sender;
            var id = br.Tag;
            this.Frame.Navigate(typeof(ShowAllTracks), id);
        }

        private string[] sortTracks(string id)
        {
            ObservableCollection<TrackViewModel> tracks = trkView.GetTracksByAlbum(id);
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

        private void Border_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Border br = (Border)sender;
            var id = br.Tag;
            this.Frame.Navigate(typeof(ShowAllTracks), id);
        }
    }
}
