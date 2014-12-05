using MyMusic.Common;
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
    public sealed partial class Collection : Page
    {
        private TracksViewModel trkView = new TracksViewModel();
        private RadioStreamsViewModel rdoView = new RadioStreamsViewModel();

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
            lstOptions.SelectedIndex = -1;            
        }


        private void lstOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedIndex != -1)
            {
                string title = ((ListBoxItem)lb.SelectedItem).Tag.ToString();
                switch (title)
                {
                    case "All Tracks":
                        this.Frame.Navigate(typeof(ShowAllTracks));
                        break;
                    case "Top Tracks":
                        this.Frame.Navigate(typeof(TopPlayed));
                        break;
                    case "Artist":
                        this.Frame.Navigate(typeof(ShowByArtist));
                        break;
                    case "Album":
                        this.Frame.Navigate(typeof(Albums));
                        break;
                    //case "Genre":
                    //    NavigationService.Navigate(new Uri("/Pages/Genres.xaml?title=" + title, UriKind.Relative));
                    //    break;
                }
            }
        }

        
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            string[] shuffled = shuffleAll();
            this.Frame.Navigate(typeof(NowPlaying), shuffled );
        }

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
    }
}
