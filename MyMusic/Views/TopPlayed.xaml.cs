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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.ViewModels
{
   
    public sealed partial class TopPlayed : Page
    {
        private TracksViewModel trkView = new TracksViewModel();

        public TopPlayed()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            lstTopPlays.SelectedIndex = -1;
            lstTopPlays.DataContext = trkView.GetTopTracks();
        }

        private void lstTopPlays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
