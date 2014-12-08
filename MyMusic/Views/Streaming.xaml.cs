using MyMusic.Common;
using MyMusicApp;
using MyToolkit.Multimedia;
//using MyToolkit.Multimedia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Streaming : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        
        public Streaming()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)      //  /2.0/?method=radio.search&name=radiohead&api_key=6101eb7c600c8a81166ec8c5c3249dd4
        {
            string artist = txtArtist.Text;     //  fields=entry[link/@rel='http://gdata.youtube.com/schemas/2007%23mobile']
            GetFlux(string.Format("https://www.googleapis.com/youtube/v3/search?part=snippet,id&q={0}&type=video&key=AIzaSyAqE2BkqebAZx90sP0Sa8iFGURKe18lm7Q",artist));

            GetFlux(string.Format("https://www.googleapis.com/youtube/v3/search?format=5&part=snippet,id&q={0}&type=video&key=AIzaSyAqE2BkqebAZx90sP0Sa8iFGURKe18lm7Q", artist));           
        }

        private async void GetFlux(string url)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            HttpResponseMessage response = await http.GetAsync(new System.Uri(url));
            string webresponse = await response.Content.ReadAsStringAsync();
            GetYoutubeChannel(webresponse);            
        }

        private void GetYoutubeChannel(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<RootObject>(json);

                List<YoutubeVideo> videosList = new List<YoutubeVideo>();
                YoutubeVideo video;
                foreach (var item in result.items)
                {
                    video = new YoutubeVideo();

                    video.YoutubeLink = new Uri("http://www.youtube.com/watch?v=" + item.id.videoId +"&feature=youtube_gdata");// + "&feature=youtube_gdata");  
                    string a = video.YoutubeLink.ToString().Remove(0, 31);
                    video.Id = a.Substring(0, 11);  
                    video.Title = item.snippet.title;
                    video.PubDate = Convert.ToDateTime(item.snippet.publishedAt);

                    video.Thumbnail = MyToolkit.Multimedia.YouTube.GetThumbnailUri(video.Id, YouTubeThumbnailSize.Large);

                    videosList.Add(video);
                }

                lstVideos.ItemsSource = videosList;
            }
            catch { }
        }

        private void lstVideos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                // Get the SyndicationItem that was tapped.
                YoutubeVideo video = (YoutubeVideo)listBox.SelectedItem;
                string id = video.Id;
                this.Frame.Navigate(typeof(YouTube), id);
            }
        }

        //private async void getStream()
        //{
        //    string song = txtSong.Text;
        //    string artist = txtArtist.Text;
            
        //    Uri resourceUri;
        //    string secHalf = string.Format("http://ws.audioscrobbler.com/2.0/?method=radio.search&name={0}&api_key=6101eb7c600c8a81166ec8c5c3249dd4", artist);
        //    if (!Helpers.TryGetUri(secHalf, out resourceUri))
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
        //        var xmlString = response.Content.ReadAsStringAsync().GetResults();
        //        XDocument doc = XDocument.Parse(xmlString);

        //        string picc = (from el in doc.Descendants("image")
        //                       where (string)el.Attribute("size") == "large"
        //                       select el).First().Value;

        //    }
        //    catch (Exception exx) { string error = exx.Message; }
        //}

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
