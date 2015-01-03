using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{

    public sealed partial class RadioStreams : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
  
        public RadioStreamsViewModel rsvm = new RadioStreamsViewModel();

        public RadioStreams()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var ep = rsvm.GetXmlGenres().ToList();
            int index = ep[0].RadioGenreId;
            int arg = (int)e.Parameter;
            switch (arg)
            {
                case 6:
                    RadioHub.ScrollToSection(RockSection);
                    break;
                case 7:
                    RadioHub.ScrollToSection(JazzSection);
                    break;
                case 8:
                    RadioHub.ScrollToSection(AlternativeSection);
                    break;
                case 9:
                    RadioHub.ScrollToSection(CountrySection);
                    break;
                case 10:
                    RadioHub.ScrollToSection(DanceSection);
                    break;                
            }
            
            this.navigationHelper.OnNavigatedTo(e);
            
            AlternativeSection.DataContext = rsvm.GetRadioStationsXml(index);
            JazzSection.DataContext = rsvm.GetRadioStationsXml(index + 1);
            CountrySection.DataContext = rsvm.GetRadioStationsXml(index + 2);           
            RockSection.DataContext = rsvm.GetRadioStationsXml(index + 3);
            DanceSection.DataContext = rsvm.GetRadioStationsXml(index + 4);

            
        }
        
        private void OnSectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            //var section = Hub.SectionsInView[0];
            //ViewModel.DefaultIndex = Hub.Sections.IndexOf(section);
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

        private  void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var url = ((RadioStreamViewModel)e.ClickedItem).RadioUrl;
            //string UrlResult = await ReadBytes(url);
            string rUrl = "radio," + url; 
            if (!Frame.Navigate(typeof(NowPlaying), rUrl))
            {
                Debug.WriteLine("navigation failed from main to radio lists ");
            }
        }

        public async Task<string> ReadBytes(string File)        // parse out the uri from the m3u's
        {
            string result = "";
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(File);
                HttpWebResponse response = (HttpWebResponse)await myHttpWebRequest.GetResponseAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream streamResponse = response.GetResponseStream();
                    StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);

                    string source = streamRead.ReadToEnd();

                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] r = source.Split(stringSeparators, StringSplitOptions.None);
                    result = r[r.Length - 2];
                    streamRead.Dispose();
                }
            }
            catch (Exception Ex)
            {
                //throw new Exception(Ex.Message);
                this.Frame.Navigate(typeof(RadioStreams));
            }
            return result;
        }
    }
}
