using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //lstRadio.ItemsSource = await rsvm.GetRadioStations(e.Parameter.ToString());            

            lstRadio.ItemsSource = rsvm.GetRadioStationsXml(e.Parameter.ToString()).Where(a => a.RadioName != ""); 
        }

        private void lstRadio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string url = ((ListBox)sender).SelectedValue.ToString();
            //string UriResult = await ReadBytes(url);
            //RadioStream rs = new RadioStream { RadioUrl = UriResult };           
            string url = ((ListBox)sender).SelectedValue.ToString();
           
           
            RadioStream rs = new RadioStream { RadioUrl = url };     
            this.Frame.Navigate(typeof(NowPlaying), rs);
        }

        public async Task<string> ReadBytes(string File)
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
