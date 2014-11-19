using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
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


namespace MyMusic.Views
{
   
    public sealed partial class ShowAllTracks : Page
    {
        private TracksViewModel trkView = new TracksViewModel();

        public ShowAllTracks()
        {
            this.InitializeComponent();
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            lstAllTracks.SelectedIndex = -1;
            lstAllTracks.DataContext = trkView.GetTracks();
        }

        private void lstAllTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id = ((ListBox)sender).SelectedValue.ToString();
            var tk = trkView.GetThisTrack(id);
            var track = new string[1];
            track[0] = tk.TrackId + "," + tk.Artist + "," + tk.Name + ",play";
            this.Frame.Navigate(typeof(NowPlaying), track);
        }
    }
}
