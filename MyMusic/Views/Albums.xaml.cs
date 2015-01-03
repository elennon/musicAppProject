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
            var itemId = ((TrackViewModel)e.ClickedItem).TrackId;
            string playThese = "albumTracks," + itemId.ToString();
            this.Frame.Navigate(typeof(NowPlaying), playThese);
        }

        #region bottom app buttons

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)  // shuffle all
        {
            this.Frame.Navigate(typeof(NowPlaying), "shuffle");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //editMode = true;
            //InBinList = false;
            //LoadList();
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
                    //LoadList();
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
                //LoadBinList();
            }

        }

        private void ShowBinnedButton_Click(object sender, RoutedEventArgs e)
        {
            //editMode = false;
            //InBinList = true;
            //LoadBinList();
        }

        private void AllTracksButton_Click(object sender, RoutedEventArgs e)
        {
            //editMode = false;
            //InBinList = false;
            //LoadList();
        }

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
