using MyMusic.Common;
using MyMusic.HelperClasses;
using MyToolkit.Multimedia;
//using ShoutCast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class YouTube : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public YouTube()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            //player.MediaFailed += mediaPlayer_MediaFailed;
            player.MediaFailed += player_MediaFailed;

            ////http://download.wavetlan.com/SVV/Media/HTTP/MP4/ConvertedFiles/MediaCoder/MediaCoder_test9_1m10s_MPEG4SP_VBR_91kbps_480x320_15fps_MPEG1Layer3_CBR_320kbps_Stereo_44100Hz.mp4
            //var uri = new Uri("http://83.169.61.65:8080");
            //var headers = await StreamingRandomAccessStream.GetHeaders(uri);
            //ulong length = (ulong)headers.ContentLength;
            //string mimeType = headers.ContentType.MediaType;
            //var stream = new StreamingRandomAccessStream(null, uri, length);
            //player.SetSource(stream, mimeType);

            string VideoId = e.Parameter.ToString();
            if (VideoId != null)
            {
                string hj = "BEzncMLLOxE";  //works
                string hu = "W0UAJMNTUro";  //works
                string hy = "9i9737ySDTw";  // no work
                var url = await MyToolkit.Multimedia.YouTube.GetVideoUriAsync(VideoId, YouTubeQuality.Quality480P);
       
                //player.Source = url.Uri;
                //player.Source = new Uri("rtsp://r3---sn-4g57kue6.c.youtube.com/CiILENy73wIaGQk8DZK833sv9hMYDSANFEgGUgZ2aWRlb3MM/0/0/0/video.3gp");
                //player.Source = new Uri("http://www.youtube.com/v/9i9737ySDTw?version=3&f=videos&app=youtube_gdata", UriKind.RelativeOrAbsolute);
                //player.Source = new Uri("http://87.118.78.71:13000");

                

                //player.Source = new Uri("http://38.96.148.18:8127");
            }         
        }

        void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            string error = e.ErrorMessage;
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            //Play();
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
