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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
          //  this.listView.ItemClick += ListView_ItemClick;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var genres = rsvm.GetRadioGenres().ToList();        // get all genres and stations 
            var stations = rsvm.GetRadioStations().ToList();
            string gName = (string)e.Parameter;
            var selectedGenre = genres.Where(a => a.RadioGenreName == gName).FirstOrDefault();      // selected genre
            string secNum = "Section" + selectedGenre.SectionNo;
            RadioHub.ScrollToSection(RadioHub.Sections.Where(a => a.Name == secNum).FirstOrDefault());      // scroll to the section containing the selected genre
            this.navigationHelper.OnNavigatedTo(e);

            if (selectedGenre.Group == 2) 
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/try5.jpg"));
                bkImage.ImageSource = bitmapImage;
            }
            else if (selectedGenre.Group == 3)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/try7.jpg"));
                bkImage.ImageSource = bitmapImage;
            }

            try
            {                
                int sectionNumber = selectedGenre.SectionNo;
                var gg = genres.Where(a => a.Group == selectedGenre.Group).ToList();
                int counter = 1;
                foreach (var genre in gg)
                {
                    var stationList = stations.Where(a => a.RadioGenre == genre.RadioGenreKey.ToString()).ToList();
                    var ids = stations.Select(a => a.RadioGenre).Distinct().ToList();
                    string secNo = "Section" + counter.ToString();
                    HubSection hb = RadioHub.Sections.Where(a => a.Name == secNo).FirstOrDefault();
                    hb.Header = genre.RadioGenreName;
                    hb.DataContext = stationList;
                    counter++;
                }

                int diff = 15 - genres.Count, sectNumber = 5;                   // 3rd group might not fill the 5 sections
                if(selectedGenre.Group == 3)                                    // for each empty section- collapse it
                    for (int i = genres.Count; i > genres.Count - diff; i--)
                    {
                        string secNo = "Section" + sectNumber.ToString();
                        HubSection hb = RadioHub.Sections.Where(a => a.Name == secNo).FirstOrDefault();
                        hb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        sectNumber--;
                    }

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
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
                throw new Exception(Ex.Message);
            }
            return result;
        }
    }
}


//protected override void OnNavigatedTo(NavigationEventArgs e)
//        {
//            var ep = rsvm.GetRadioGenres().ToList();
//            int index = ep[0].RadioGenreKey;//.RadioGenreId;
//            string arg = (string)e.Parameter;
           
//            switch (arg)
//            {
//                case "Altenrative":
//                    RadioHub.ScrollToSection(AltenrativeSection);
//                    break;
//                case "Blues":
//                    RadioHub.ScrollToSection(BluesSection);
//                    break;
//                case "Comedy":
//                    RadioHub.ScrollToSection(ComedySection);
//                    break;
//                case "Country":
//                    RadioHub.ScrollToSection(CountrySection);
//                    break;
//                case "Dance":
//                    RadioHub.ScrollToSection(DanceSection);
//                    break;
//                case "Folk":
//                    RadioHub.ScrollToSection(FolkSection);
//                    break;
//                case "Irish":
//                    RadioHub.ScrollToSection(IrishSection);
//                    break;
//                case "Jazz":
//                    RadioHub.ScrollToSection(JazzSection);
//                    break;
//                case "Pop":
//                    RadioHub.ScrollToSection(PopSection);
//                    break;
//                case "R and B":
//                    RadioHub.ScrollToSection(RnBSection);
//                    break;
//                case "Reggae":
//                    RadioHub.ScrollToSection(ReggaeSection);
//                    break;
//                case "Rock":
//                    RadioHub.ScrollToSection(RockSection);
//                    break;  
//            }

//            StringBuilder sb = new StringBuilder();
//            sb.Append("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
//            sb.Append("<ListView x:Name=\"lstSectionView\" SelectionMode=\"None\" IsItemClickEnabled=\"True\" ItemTemplate=\"{StaticResource StandardDoubleLineItemTemplate}\" ItemClick=\"ListView_ItemClick\">");
//            sb.Append("</ListView >");
//            sb.Append("</DataTemplate>");
//            DataTemplate datatemplate = (DataTemplate)XamlReader.Load(sb.ToString());
//            DataTemplate headertemplate = (Windows.UI.Xaml.DataTemplate)this.Resources.Where(a => a.Key == "HubSectionHeaderTemplate").FirstOrDefault().Value;


//            HubSection hs = new HubSection { Name = "s1", Header = "h1", ContentTemplate = datatemplate, HeaderTemplate = headertemplate };
//            RadioHub.Sections.Add(hs);

//            ListView lv = new ListView();
//            lv.SelectionMode = ListViewSelectionMode.None;
//            lv.ItemClick += ListView_ItemClick;
//            lv.ItemTemplate = (Windows.UI.Xaml.DataTemplate)this.Resources.Where(a => a.Key == "StandardDoubleLineItemTemplate").FirstOrDefault().Value;
//            DataTemplate dt = new DataTemplate();
      
//            dt.LoadContent();

            
            



//            this.navigationHelper.OnNavigatedTo(e);
//            var alt = rsvm.GetRadioStationsXml(index);
//            AltenrativeSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(alt.FirstOrDefault().Image) });
//            AltenrativeSection.Background.Opacity = 0.4;
//            AltenrativeSection.DataContext = alt;

//            var blu = rsvm.GetRadioStationsXml(index + 1);
//            BluesSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(blu.FirstOrDefault().Image) });
//            BluesSection.Background.Opacity = 0.3;
//            BluesSection.DataContext = blu;

//            var com = rsvm.GetRadioStationsXml(index + 2);
//            ComedySection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(com.FirstOrDefault().Image) });
//            ComedySection.Background.Opacity = 0.3;
//            ComedySection.DataContext = com;

//            var hick = rsvm.GetRadioStationsXml(index + 3);
//            CountrySection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(hick.FirstOrDefault().Image) });
//            CountrySection.Background.Opacity = 0.3;
//            CountrySection.DataContext = hick;

//            var dan = rsvm.GetRadioStationsXml(index + 4);
//            DanceSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(dan.FirstOrDefault().Image) });
//            DanceSection.Background.Opacity = 0.3;
//            DanceSection.DataContext = dan;

//            var folk = rsvm.GetRadioStationsXml(index + 5);
//            FolkSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(folk.FirstOrDefault().Image) });
//            FolkSection.Background.Opacity = 0.3;
//            FolkSection.DataContext = folk;

//            var paddy = rsvm.GetRadioStationsXml(index + 6);
//            IrishSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(paddy.FirstOrDefault().Image) });
//            IrishSection.Background.Opacity = 0.3;
//            IrishSection.DataContext = paddy;

//            var jazz = rsvm.GetRadioStationsXml(index + 7);
//            JazzSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(jazz.FirstOrDefault().Image) });
//            JazzSection.Background.Opacity = 0.3;
//            JazzSection.DataContext = jazz;

//            var pop = rsvm.GetRadioStationsXml(index + 8);
//            PopSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(pop.FirstOrDefault().Image) });
//            PopSection.Background.Opacity = 0.3;
//            PopSection.DataContext = pop;

//            var rb = rsvm.GetRadioStationsXml(index + 9);
//            RnBSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(rb.FirstOrDefault().Image) });
//            RnBSection.Background.Opacity = 0.3;
//            RnBSection.DataContext = rb;

//            var reg = rsvm.GetRadioStationsXml(index + 10);
//            ReggaeSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(reg.FirstOrDefault().Image) });
//            ReggaeSection.Background.Opacity = 0.3;
//            ReggaeSection.DataContext = reg;

//            var rok = rsvm.GetRadioStationsXml(index + 11);
//            RockSection.Background.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage() { UriSource = new Uri(rok.FirstOrDefault().Image) });
//            RockSection.Background.Opacity = 0.3;
//            RockSection.DataContext = rok;           
//        }



       
        //private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        //{
        //    int childNumber = VisualTreeHelper.GetChildrenCount(control);
        //    for (int i = 0; i < childNumber; i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(control, i);
        //        FrameworkElement fe = child as FrameworkElement;
        //        // Not a framework element or is null
        //        if (fe == null) return null;

        //        if (child is T && fe.Name == ctrlName)
        //        {
        //            // Found the control so return
        //            return child;
        //        }
        //        else
        //        {
        //            // Not found it - search children
        //            DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
        //            if (nextLevel != null)
        //                return nextLevel;
        //        }
        //    }
        //    return null;
        //}
        ////static DependencyObject FindChildByName(DependencyObject from, string name)
        ////{
        ////    int count = VisualTreeHelper.GetChildrenCount(from);

        ////    for (int i = 0; i < count; i++)
        ////    {
        ////        var child = VisualTreeHelper.GetChild(from, i);
        ////        if (child is FrameworkElement && ((FrameworkElement)child).Name == name)
        ////            return child;

        ////        var result = FindChildByName(child, name);
        ////        if (result != null)
        ////            return result;
        ////    }

        ////    return null;
        ////}      



//protected override void OnNavigatedTo(NavigationEventArgs e)
//{
//    this.navigationHelper.OnNavigatedTo(e);
//    var genres = rsvm.GetRadioGenres().ToList();
//    var stations = rsvm.GetRadioStations().ToList();
//    int index = genres[0].RadioGenreKey;//.RadioGenreId;        x:Name=\"lstSectionView\" ItemTemplate=\"{StaticResource StandardDoubleLineItemTemplate}\"
//    string arg = (string)e.Parameter;
//    RadioHub.DataContext = stations;

//    try
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
//        sb.Append("<ListView Name=\"lstView\" SelectionMode=\"None\" IsItemClickEnabled=\"True\" ItemsSource=\"{Binding}\" ItemTemplate=\"{StaticResource StandardDoubleLineItemTemplate}\" >");
//        sb.Append("</ListView >");
//        sb.Append("</DataTemplate>");

//        DataTemplate dt = (DataTemplate)XamlReader.Load(sb.ToString());
//        DataTemplate headertemplate = (Windows.UI.Xaml.DataTemplate)this.Resources.Where(a => a.Key.ToString() == "HubSectionHeaderTemplate").FirstOrDefault().Value;

//        foreach (var item in genres)
//        {
//            string sectionName = "Section" + item.SectionNo.ToString();
//            HubSection hs = new HubSection { Name = sectionName, ContentTemplate = dt, Header = item.RadioGenreName, HeaderTemplate = headertemplate };  // HeaderTemplate = headertemplate
//            hs.DataContext = stations.Where(a => a.RadioGenre == item.RadioGenreKey.ToString()).ToList();
//            RadioHub.Sections.Add(hs);

//            var result = (from m in RadioHub.Sections
//                          where (m is FrameworkElement) && ((FrameworkElement)m).Name == "Section1"
//                            select m as FrameworkElement).FirstOrDefault();

//            var wes = (ListView)result.FindName("lstView");
//        }

//        //var tf = (ListView)FindChildByName(this, "lstView");
//        //var gh = FindChildControl<ListView>(RadioHub, "lstView") as ListView;
//        //foreach (var item in RadioHub.Sections)
//        //{


//        //    SelectorItem itemContainer = (SelectorItem)item.ItemContainer;
//        //    HubSection templateRoot = (HubSection)itemContainer.ContentTemplateRoot;
//        //    TextBlock nameTextBlock = (TextBlock)templateRoot.FindName("txtArtistName");
//        //}

//    }
//    catch (Exception Ex)
//    {
//        throw new Exception(Ex.Message);
//    }
//}