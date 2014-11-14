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
            string VideoId = e.Parameter.ToString();
            if (VideoId != null)
            {
                string hj = "BEzncMLLOxE";
                string hu = "W0UAJMNTUro";
                var url = await MyToolkit.Multimedia.YouTube.GetVideoUriAsync(VideoId, YouTubeQuality.Quality480P);
                try
                {
                    player.Source = url.Uri;
                }
                catch (Exception ev) { }
            }         
        }
    }
}
