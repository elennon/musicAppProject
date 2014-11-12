using MyMusic.Common;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
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

        public Collection()
        {
            this.InitializeComponent();           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
                        this.Frame.Navigate(typeof(Collection));
                        break;
                    //case "Album":
                    //    NavigationService.Navigate(new Uri("/Pages/Albums.xaml?title=" + title, UriKind.Relative));
                    //    break;
                    //case "All Tracks":
                    //    NavigationService.Navigate(new Uri("/Pages/AllTracks.xaml?title=" + title, UriKind.Relative));
                    //    break;
                    //case "Genre":
                    //    NavigationService.Navigate(new Uri("/Pages/Genres.xaml?title=" + title, UriKind.Relative));
                    //    break;
                }
            }
        }

        private void btnPoops_Click(object sender, RoutedEventArgs e)
        {
            trkView.fillDB();
        }

        private void btnClearDB_Click(object sender, RoutedEventArgs e)
        {
            trkView.emptyDB();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NowPlaying), "shuffle" );
        }

    }
}
