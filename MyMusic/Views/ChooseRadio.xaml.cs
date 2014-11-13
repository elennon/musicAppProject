using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChooseRadio : Page
    {
        public RadioStreamsViewModel rsvm = new RadioStreamsViewModel();
        
        public ChooseRadio()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            lstRadio.ItemsSource = await rsvm.GetRadioStations(e.Parameter.ToString());            
        }

        private void lstRadio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string url = ((ListBox)sender).SelectedValue.ToString();



            this.Frame.Navigate(typeof(NowPlaying), url);
            //this.Frame.Navigate(typeof(NowPlaying), "shuffle");
        }
    }
}
