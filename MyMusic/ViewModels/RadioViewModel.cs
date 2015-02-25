using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
using MyMusic.Models;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MyMusic.ViewModels
{
    public class RadioViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private List<SectionList> StationsList { get; set; }

        private ObservableCollection<RadioStream> _hubSection1;
        public ObservableCollection<RadioStream> hubSection1
        {
            get
            {
                return _hubSection1;
            }
            set
            {
                if (_hubSection1 != value)
                {
                    _hubSection1 = value;
                    NotifyPropertyChanged("hubSection1");
                }
            }
        }

        private ObservableCollection<RadioStream> _hubSection2;
        public ObservableCollection<RadioStream> hubSection2
        {
            get
            {
                return _hubSection2;
            }
            set
            {
                if (_hubSection2 != value)
                {
                    _hubSection2 = value;
                    NotifyPropertyChanged("hubSection2");
                }
            }
        }

        private ObservableCollection<RadioStream> _hubSection3;
        public ObservableCollection<RadioStream> hubSection3
        {
            get
            {
                return _hubSection3;
            }
            set
            {
                if (_hubSection3 != value)
                {
                    _hubSection3 = value;
                    NotifyPropertyChanged("hubSection3");
                }
            }
        }

        private ObservableCollection<RadioStream> _hubSection4;
        public ObservableCollection<RadioStream> hubSection4
        {
            get
            {
                return _hubSection4;
            }
            set
            {
                if (_hubSection4 != value)
                {
                    _hubSection4 = value;
                    NotifyPropertyChanged("hubSection4");
                }
            }
        }

        private ObservableCollection<RadioStream> _hubSection5;
        public ObservableCollection<RadioStream> hubSection5
        {
            get
            {
                return _hubSection5;
            }
            set
            {
                if (_hubSection5 != value)
                {
                    _hubSection5 = value;
                    NotifyPropertyChanged("hubSection5");
                }
            }
        }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RadioStream> RadioItemSelectedCommand { get; set; }

        public RadioStreams rs = RadioStreams.thisRadioStreams; // adds a reference to radioStreams view to populate the hubsection 
        private string para = "";

        public RadioViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;            
            StationsList = AddSectionLists();
            //hubSection1 = repo.GetRadioStations();
            hubSection1 = StationsList[0].RdoList;

            LoadCommand = new RelayCommand<RoutedEventArgs>((args) =>
            {                
                loadHub(para);                               
                AddToRdoLists();
            });
            RadioItemSelectedCommand = new RelayCommand<RadioStream>((rs) =>
            {
                string rUrl = "radio," + rs.RadioUrl;
                _navigationService.NavigateTo("NowPlaying", rUrl);
            });
        }

        private void AddToRdoLists()
        {
            hubSection1 = new ObservableCollection<RadioStream>(StationsList[0].RdoList);
            hubSection2 = new ObservableCollection<RadioStream>(StationsList[1].RdoList);
            hubSection3 = new ObservableCollection<RadioStream>(StationsList[2].RdoList);
            hubSection4 = new ObservableCollection<RadioStream>(StationsList[3].RdoList);
            hubSection5 = new ObservableCollection<RadioStream>(StationsList[4].RdoList);
        }

        private List<SectionList> AddSectionLists()
        {
            List<SectionList> sts = new List<SectionList>();
            sts.Add(new SectionList { Name = "Section1" });
            sts.Add(new SectionList { Name = "Section2" });
            sts.Add(new SectionList { Name = "Section3" });
            sts.Add(new SectionList { Name = "Section4" });
            sts.Add(new SectionList { Name = "Section5" });

            return sts;
        }

        public void Activate(object parameter)
        {
            para = parameter.ToString();            
        }

        public void Deactivate(object parameter)
        {
            string hu = "22";
        }

        public void loadHub(string gName)  // param passed from loaded event to set up hub sections depending on selected stations
        {
            rs = RadioStreams.thisRadioStreams;
            var genres = repo.GetRadioGenres().ToList();        // get all genres and stations 
            var stations = repo.GetRadioStations().ToList();
            var selectedGenre = genres.Where(a => a.RadioGenreName == gName).FirstOrDefault();      // selected genre
            string secNum = "Section" + selectedGenre.SectionNo;
            rs.thisHub.ScrollToSection(rs.thisHub.Sections.Where(a => a.Name == secNum).FirstOrDefault());      // scroll to the section containing the selected genre
            //rs.navigationHelper.OnNavigatedTo(parameter);
            if (selectedGenre.Group == 2)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/try5.jpg"));
                rs.thisHubImage.ImageSource = bitmapImage;
            }
            else if (selectedGenre.Group == 3)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/try7.jpg"));
                rs.thisHubImage.ImageSource = bitmapImage;
            }
            try
            {
                int sectionNumber = selectedGenre.SectionNo;
                var gg = genres.Where(a => a.Group == selectedGenre.Group).ToList();
                int counter = 1;
                foreach (var genre in gg)
                {
                    var stationList = stations.Where(a => a.RadioGenreId.ToString() == genre.RadioGenreKey).ToList();
                    var ids = stations.Select(a => a.RadioGenreId).Distinct().ToList();
                    string secNo = "Section" + counter.ToString();
                    HubSection hb = rs.thisHub.Sections.Where(a => a.Name == secNo).FirstOrDefault();
                    hb.Header = genre.RadioGenreName;
                    //hb.DataContext = stationList;
                    (StationsList.Where(a => a.Name == secNo).FirstOrDefault()).RdoList = new ObservableCollection<RadioStream>(stationList);
                    counter++;
                }

                int diff = 15 - genres.Count, sectNumber = 5;                   // 3rd group might not fill the 5 sections
                if (selectedGenre.Group == 3)                                    // for each empty section- collapse it
                    for (int i = genres.Count; i > genres.Count - diff; i--)
                    {
                        string secNo = "Section" + sectNumber.ToString();
                        HubSection hb = rs.thisHub.Sections.Where(a => a.Name == secNo).FirstOrDefault();
                        hb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        sectNumber--;
                    }

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        
    }

    public interface INavigable
    {
        void Activate(object parameter);
        void Deactivate(object parameter);
    }

    public class SectionList : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public ObservableCollection<RadioStream> _rdoList;
        public ObservableCollection<RadioStream> RdoList
        {
            get
            {
                return _rdoList;
            }
            set
            {
                if (_rdoList != value)
                {
                    _rdoList = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}



//public async Task<string> ReadBytes(string File)        // parse out the uri from the m3u's
//        {
//            string result = "";
            //try
            //{
            //    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(File);
            //    HttpWebResponse response = (HttpWebResponse)await myHttpWebRequest.GetResponseAsync();
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        Stream streamResponse = response.GetResponseStream();
            //        StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);

            //        string source = streamRead.ReadToEnd();

            //        string[] stringSeparators = new string[] { "\r\n" };
            //        string[] r = source.Split(stringSeparators, StringSplitOptions.None);
            //        result = r[r.Length - 2];
            //        streamRead.Dispose();
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    throw new Exception(Ex.Message);
            //}
        //    return result;
        //}