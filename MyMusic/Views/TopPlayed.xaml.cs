using MyMusic.Common;
using MyMusic.Views;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.ViewModels
{
   
    public sealed partial class TopPlayed : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private TracksViewModel trkView = new TracksViewModel();

        public TopPlayed()
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
            lstViewDetail.ItemsSource = trkView.GetTopTracks();
        }

        private void lstTopPlays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            //string[] shuffled = shuffleTopPlayed();
            //this.Frame.Navigate(typeof(NowPlaying), shuffled);
        }

        private string[] shuffleTopPlayed()
        {
            List<int> trks = new List<int>();
            ObservableCollection<TrackViewModel> shuffled = shuffleTop((ObservableCollection<TrackViewModel>)trkView.GetTopTracks());  // get top tracks and send them to be shuffled                
            string[] trkks = new string[shuffled.Count];
            for (int i = 0; i < shuffled.Count; i++)
            {
                trkks[i] = shuffled[i].TrackId.ToString() + "," + shuffled[i].Artist + "," + shuffled[i].Name + ",shuffle";
            }
            return trkks;
        }

        private ObservableCollection<TrackViewModel> shuffleTop(ObservableCollection<TrackViewModel> trks4shuffle)
        {
            Random rng = new Random();
            int n = trks4shuffle.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                TrackViewModel value = trks4shuffle[k];
                trks4shuffle[k] = trks4shuffle[n];
                trks4shuffle[n] = value;
            }
            return trks4shuffle;
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
