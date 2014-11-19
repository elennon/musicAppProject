using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YouTube : Page
    {
        public YouTube()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //player.MediaFailed += mediaPlayer_MediaFailed;
            player.MediaFailed += player_MediaFailed;
            string VideoId = e.Parameter.ToString();
            if (VideoId != null)
            {
                string hj = "BEzncMLLOxE";  //works
                string hu = "W0UAJMNTUro";  //works
                string hy = "9i9737ySDTw";  // no work
                var url = await MyToolkit.Multimedia.YouTube.GetVideoUriAsync(VideoId, YouTubeQuality.Quality480P);
       
                player.Source = url.Uri;
                //player.Source = new Uri("rtsp://r3---sn-4g57kue6.c.youtube.com/CiILENy73wIaGQk8DZK833sv9hMYDSANFEgGUgZ2aWRlb3MM/0/0/0/video.3gp");
                //player.Source = new Uri("http://www.youtube.com/v/9i9737ySDTw?version=3&f=videos&app=youtube_gdata", UriKind.RelativeOrAbsolute);
                
            }         
        }

        void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            string error = e.ErrorMessage;
        }

        private void mediaPlayer_MediaFailed(MediaElement sender, MediaFailedRoutedEventArgs args)
        {
            string error = args.ErrorMessage;
        }
    }
}
