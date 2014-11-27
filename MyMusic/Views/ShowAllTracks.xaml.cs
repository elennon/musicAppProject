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
            var para = e.Parameter;
            if (para != null)
            {
                lstAllTracks.DataContext = trkView.GetTracksByAlbum(para.ToString());
            }
            else { lstAllTracks.DataContext = trkView.GetTracks(); }
            
        }

        private void lstAllTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string orderNo = ((ListBox)sender).SelectedValue.ToString();

            this.Frame.Navigate(typeof(NowPlaying), GetListToPlay(Convert.ToInt32(orderNo)));
        }

        private string[] GetListToPlay(int orderNo)
        {
            ObservableCollection<TrackViewModel> shuffled = new ObservableCollection<TrackViewModel>();
            var trks = (trkView.GetTracks()).Where(a => a.OrderNo >= orderNo).ToList(); // get all tracks listed after selected one
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].Artist + "," + trks[i].Name + ",shuffle";
            }
            return trkArray;
        }
        
    }
}
