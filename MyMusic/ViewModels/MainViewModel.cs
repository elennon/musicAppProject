using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.Common;
using MyMusic.DAL;
using MyMusic.HelperClasses;
//using MyMusic.HelperClasses;
using MyMusic.Models;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyMusic.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private Repository repo =new Repository();
        private INavigationService _navigationService;

        private ObservableCollection<RadioGenre> _genres;
        public ObservableCollection<RadioGenre> Genres
        {
            get
            {
                return _genres;
            }
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    NotifyPropertyChanged("Genres");
                }
            }
        } 
        
        public ObservableCollection<DataGroup> Collections { get; set; }
        public ObservableCollection<DataGroup> Streams { get; set; }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
        }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RadioGenre> RadioItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> CollectionItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> PlaylistItemSelectedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand NowPlayingCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand BackUpCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FillDbCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand TestCommand { get; set; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Genres = repo.GetRadioGenres();
            Collections = LoadCollectionList();
            Streams = LoadStreamingList();
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.RadioItemSelectedCommand = new RelayCommand<RadioGenre>(OnRadioItemSelectedCommand);
            this.CollectionItemSelectedCommand = new RelayCommand<DataGroup>(OnCollectionItemSelectedCommand);
            this.PlaylistItemSelectedCommand = new RelayCommand<DataGroup>(OnPlaylistItemSelectedCommand);
            this.NowPlayingCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnNowPlayingCommand);
            this.BackUpCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnBackUpCommand);
            this.FillDbCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFillDbCommand);
            this.TestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnTestCommand);            
        }

        #region Commands

        private async void OnTestCommand()
        {
            //await repo.AddDuration();
            //string rUrl = "radio,http://stream209a-he.grooveshark.com/stream.php?streamKey=d2f3c858ebfa71e7cc0d472d6307ef6ab39bfe07_55060902_de997c_17073f1_1811fc720_8_0," + "138";
            //_navigationService.NavigateTo("NowPlaying", rUrl);
            //await repo.SortPics();
           // await repo.GetSimilarLastFmTracks("nofx", 5);
        }

        private async void OnFillDbCommand()
        {
          //  repo.fillDB();
            await Task.Run(async delegate()
            {
                await repo.SortPics();
            });
        }

        private void OnBackUpCommand()
        {
            //repo.BackUpDb();
        }

        private void OnNowPlayingCommand()
        {
            _navigationService.NavigateTo("NowPlaying");
        }

        private void OnPlaylistItemSelectedCommand(DataGroup obj)
        {
            string itemId = obj.UniqueId;
            switch (itemId)
            {
                case "Stream":
                    object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.GSSessionId);
                    if (value == null)
                    {
                        _navigationService.NavigateTo("GSSignIn");
                    }
                    else
                    {
                        _navigationService.NavigateTo("GSMainPage", (string)value);
                    }
                    break;
                case "ReadyMade":
                    _navigationService.NavigateTo("GenerateFromCollection");
                    break;
                case "SavedPlayList":
                    _navigationService.NavigateTo("SavedPlaylists", itemId);
                    break;
            }          
        }

        private void OnCollectionItemSelectedCommand(DataGroup gp)
        {
            var itemId = gp.UniqueId;
            _navigationService.NavigateTo("Collection", itemId);
        }

        private void OnRadioItemSelectedCommand(RadioGenre rg)
        {
            var rdoName = rg.RadioGenreName;
            //_navigationService.Navigate(typeof(RadioStreams), rdoName);
            _navigationService.NavigateTo("Radio", rdoName);
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {           
            //repo.BackUpDb();
            //await repo.GetPrologList();
            //await repo.SyncWithApi();          
            Stopwatch sw = Stopwatch.StartNew();
            var firstBitDone = false;
            object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.DbFirstHalf);
            if (value != null)
            {
                firstBitDone = (bool)value;
            }          
            var isFirst = ((App)Application.Current).IsAppFirstStart;
            if (isFirst == true)
            {
                IsVisible = true;
                if (!firstBitDone)
                {
                    //await repo.fillDbFromXml();
                    await repo.fillDB3();
                    await repo.FillRadioDB();
                    await repo.SortPics();
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.DbFirstHalf, true);
                }                
                Genres = repo.GetRadioGenres();
                Collections = LoadCollectionList();
                Streams = LoadStreamingList();
                var time = (double)sw.ElapsedMilliseconds / 1000;
                IsVisible = false;
                await Task.Run(async delegate()
                {                    
                    await repo.SyncWithApi();
       //             await repo.SyncDB();
                    time = (double)sw.ElapsedMilliseconds / 1000;
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.IsFirstTime, false);
                });            
            }
            else
            {               
                await Task.Run(async delegate()
                {
                    await repo.SyncDB();                
                });
            }
            
            Logger.GetLogger().logChannel.LogMessage("Main page loading" + DateTime.Now.ToString());
            var ts = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
            IReadOnlyList<StorageFile> lf = await ts.GetFilesAsync();
            foreach (var item in lf)
            {
                var ty = item.OpenReadAsync();
                string text = await Windows.Storage.FileIO.ReadTextAsync(item);
            }
        }

        #endregion

        public void Activate(object parameter)
        {
            
        }

        public void Deactivate(object parameter)
        {
            
        }

        public ObservableCollection<DataGroup> LoadCollectionList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup { Title = "All Tracks", UniqueId = "All Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Top Tracks", UniqueId = "Top Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Artists", UniqueId = "Artist", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Album", UniqueId = "Album", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Genre", UniqueId = "Genre", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Quick Pick", UniqueId = "QuickPick", ImagePath = "ms-appx:///Assets/music3.jpg" });
            return groups;
        }

        public ObservableCollection<DataGroup> LoadStreamingList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup("Stream", "Streaming", "music streaming and playlist generation", "ms-appx:///Assets/music.jpg"));           
            groups.Add(new DataGroup("ReadyMade", "Ready Made Playlists", "music collection", "ms-appx:///Assets/music3.jpg"));
            groups.Add(new DataGroup("SavedPlayList", "DIY PlayLists", "Playlists collection", "ms-appx:///Assets/radio.jpg"));
            return groups;
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









//readLog();
//trkView.lookIn();
//var getPicAndGenre = await trkView.getPic2("nofx", "bob");
//var fr = getPicAndGenre.album.image.FirstOrDefault(); //Where(a => a.size == "large").FirstOrDefault();
//var tyu = getPicAndGenre.toptags.tag.FirstOrDefault().name;
//trkView.loadUpImagesAndGenre();
//trkView.sortOrderNum();
//var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
//var filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
//var logSave = Logger.GetLogger().logSession.SaveToFileAsync(folder, filename).AsTask();
//logSave.Wait();
//string rUrl = "radio,http://37.58.75.163:9272/stream";
//if (!Frame.Navigate(typeof(NowPlaying), rUrl))
//{
//    Debug.WriteLine("navigation failed from main to radio lists ");
//}                
//repo.GetApiFillDB();