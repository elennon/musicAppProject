using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
//using MyMusic.HelperClasses;
using MyMusic.Models;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyMusic.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private Repository repo =new Repository();
        private INavigationService _navigationService;
        public ObservableCollection<RadioGenre> Genres { get; set; }
        public ObservableCollection<DataGroup> Collections { get; set; }
        public ObservableCollection<DataGroup> Streams { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RadioGenre> RadioItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> CollectionItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> PlaylistItemSelectedCommand { get; set; }
        public RelayCommand NowPlayingCommand { get; set; }
        public RelayCommand BackUpCommand { get; set; }
        public RelayCommand FillDbCommand { get; set; }
        public RelayCommand TestCommand { get; set; }

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
            this.NowPlayingCommand = new RelayCommand(OnNowPlayingCommand);
            this.BackUpCommand = new RelayCommand(OnBackUpCommand);
            this.FillDbCommand = new RelayCommand(OnFillDbCommand);
            this.TestCommand = new RelayCommand(OnTestCommand);
            
        }

        #region Commands

        private async void OnTestCommand()
        {
            //await repo.GetEchoNestInfo();
            //string rUrl = "radio,http://stream209a-he.grooveshark.com/stream.php?streamKey=d2f3c858ebfa71e7cc0d472d6307ef6ab39bfe07_55060902_de997c_17073f1_1811fc720_8_0," + "138";
            //_navigationService.NavigateTo("NowPlaying", rUrl);
            //await repo.SortPics();
           // await repo.GetSimilarLastFmTracks("nofx", 5);
        }

        private void OnFillDbCommand()
        {
            repo.fillDB();
        }

        private void OnBackUpCommand()
        {
            repo.BackUpDb();
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
           // await trkView.SyncDB();
            await Task.Run(async delegate()
            {
                await repo.SyncDB();
            });

            //Log.GetLog().Write("Main page nav tooooollooooooo");
            //Logger.GetLogger().logChannel.LogMessage("Main page nav to");
            //var ts = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
            //IReadOnlyList<StorageFile> lf = await ts.GetFilesAsync();
            // foreach (var item in lf)
            // {
            //     var ty = item.OpenReadAsync();
            //     string text = await Windows.Storage.FileIO.ReadTextAsync(item);               
            // }
        }

        #endregion

        public void Activate(object parameter)
        {
            string hu = "22";
        }

        public void Deactivate(object parameter)
        {
            string hu = "22";
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