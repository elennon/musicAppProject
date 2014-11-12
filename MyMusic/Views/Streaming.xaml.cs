using MyMusic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Streaming : Page
    {
        private HttpBaseProtocolFilter filter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;

        public Streaming()
        {
            this.InitializeComponent();          
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            filter = new HttpBaseProtocolFilter();
            httpClient = new HttpClient(filter);
            cts = new CancellationTokenSource();
        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)      //  /2.0/?method=radio.search&name=radiohead&api_key=6101eb7c600c8a81166ec8c5c3249dd4
        {
            string song = txtSong.Text;
            string artist = txtArtist.Text;

            Uri resourceUri;
            string secHalf = string.Format("http://ws.audioscrobbler.com/2.0/?method=radio.search&name={0}&api_key=6101eb7c600c8a81166ec8c5c3249dd4", artist);
            if (!Helpers.TryGetUri(secHalf, out resourceUri))
            {
                return;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();
                XDocument doc = XDocument.Parse(xmlString);

                string picc = (from el in doc.Descendants("image")
                               where (string)el.Attribute("size") == "large"
                               select el).First().Value;
                
            }
            catch (Exception exx) { string error = exx.Message; }
            
        }
    }
}
