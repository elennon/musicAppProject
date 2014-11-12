using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace MyMusic.ViewModels
{
    public class RadioStreamViewModel : INotifyPropertyChanged
    {
        private int _radioId;
        public int RadioId
        {
            get
            {
                return _radioId;
            }
            set
            {
                if (_radioId != value)
                {
                    _radioId = value;
                    NotifyPropertyChanged("RadioId");
                }
            }
        }

        private string _Name;
        public string RadioName
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged("RadioName");
                }
            }
        }

        private string _genre;
        public string RadioGenre
        {
            get
            {
                return _genre;
            }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    NotifyPropertyChanged("RadioGenre");
                }
            }
        }

        //private string _genreId;
        //public string RadioGenreId
        //{
        //    get
        //    {
        //        return _genreId;
        //    }
        //    set
        //    {
        //        if (_genreId != value)
        //        {
        //            _genreId = value;
        //            NotifyPropertyChanged("RadioGenreId");
        //        }
        //    }
        //}

        private string _Url;
        public string RadioUrl
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    NotifyPropertyChanged("RadioUrl");
                }
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class RadioStreamsViewModel : ViewModelBase
    {
        
        private HttpBaseProtocolFilter filter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;
        private string key = "tEsnhZWScj3FfzqtypxBSCx04DM1nCKWdfYQJl5W90OzUuFyqivtylJ3wD3vn5YIX3GkPNMO1HebqbFFOIT7pMCFevEmaXBueMyZhj5mHxy1UKM3V0hiG0yI4DLmCkvl";
        

        private ObservableCollection<RadioStreamViewModel> _radioStreams;
        public ObservableCollection<RadioStreamViewModel> RadioStreams
        {
            get
            {
                return _radioStreams;
            }

            set
            {
                _radioStreams = value;
                RaisePropertyChanged("RadioStreams");
            }
        }

        //public ObservableCollection<RadioStreamViewModel> GetRadioStreams()
        //{
        //    _radioStreams = new ObservableCollection<RadioStreamViewModel>();
        //    using (var db = new SQLite.SQLiteConnection(App.DBPath))
        //    {
        //        var query = db.Table<RadioStream>().OrderBy(c => c.RadioName);
        //        foreach (var rs in query)
        //        {
        //            var rdo = new RadioStreamViewModel()
        //            {
        //                RadioId = rs.RadioId,
        //                RadioName = rs.RadioName,
        //                RadioGenre = rs.RadioGenre,
        //                RadioUrl = rs.RadioUrl
        //            };
        //            _radioStreams.Add(rdo);
        //        }
        //    }
        //    return _radioStreams;
        //}

        public async Task<List<RadioStreamGenre>> Getgenres()
        {
            filter = new HttpBaseProtocolFilter();
            httpClient = new HttpClient(filter);
            cts = new CancellationTokenSource();
            List<RadioStreamGenre> geners = new List<RadioStreamGenre>();

            Uri resourceUri;
            string queryString = string.Format("http://streamfinder.com/api/index.php?api_codekey={0}&return_data_format=xml&do=get_genre_list", key);

            if (!Helpers.TryGetUri(queryString, out resourceUri))
            {
                return null;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                //var response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();
                
                XDocument doc = XDocument.Parse(xmlString);

                List<XElement> genList = (from a in doc.Descendants("genre")
                               select a).ToList();
                if (genList != null)
                {
                    foreach (XElement item in genList)
                    {
                        var gid = item.Element("gid").Value;
                        var _name = item.Element("genre_name").Value;
                        geners.Add(new RadioStreamGenre { RadioGenreName = _name.ToString(), RadioGenreKey = gid.ToString() });
                    }
                }                
            }
            catch (Exception exx) { string error = exx.Message; }

            return geners;
        }

        public async Task<IEnumerable<RadioStream>> GetRadioStations(string gid)
        {
            filter = new HttpBaseProtocolFilter();
            httpClient = new HttpClient(filter);
            cts = new CancellationTokenSource();
            List<RadioStream> raders = new List<RadioStream>();

            Uri resourceUri;
            string queryString = string.Format("http://streamfinder.com/api/index.php?api_codekey={0}&return_data_format=xml&do=genre_search&gid={1}&format=mp3", key, gid);

            if (!Helpers.TryGetUri(queryString, out resourceUri))
            {
                return null;
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                var xmlString = response.Content.ReadAsStringAsync().GetResults();

                XDocument doc = XDocument.Parse(xmlString);
                List<XElement> rdoList = (from a in doc.Descendants("data")
                                          select a).ToList();
                if (rdoList != null)
                {
                    foreach (XElement data in rdoList)
                    {
                        var _name = data.Element("name").Value;
                        var url = (from a in data.Element("streams").Descendants("stream_url")
                                   select a).First().Value;
                        if ((from a in raders where a.RadioName == _name || a.RadioUrl == url select a).Count() < 1)
                        { 
                            raders.Add(new RadioStream { RadioName = _name.ToString(), RadioUrl = url.ToString() }); 
                        }
                    }
                }
            }
            catch (Exception exx) { string error = exx.Message; }
      
            return raders;
        }
        
        public async void refreshStreams()
        {           
            List<RadioStreamGenre> gns = await Getgenres();            
        }
    }
}



