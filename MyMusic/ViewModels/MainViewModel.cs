﻿using GalaSoft.MvvmLight;
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
        public static ObservableCollection<RadioGenre> Genres { get; set; }
        public static ObservableCollection<DataGroup> Collections { get; set; }
        public static ObservableCollection<DataGroup> Streams { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RadioGenre> RadioItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> CollectionItemSelectedCommand { get; set; }
        public RelayCommand<DataGroup> StreamingItemSelectedCommand { get; set; }
        public RelayCommand NowPlayingCommand { get; set; }
        public RelayCommand BackUpCommand { get; set; }
        public RelayCommand FillDbCommand { get; set; }
        public RelayCommand TestCommand { get; set; }

        public MainViewModel(INavigationService navigationService)
        {
            //_navigationService = new NavigationService();
            _navigationService = navigationService;
            Genres = repo.GetRadioGenres();
            Collections = LoadCollectionList();
            Streams = LoadStreamingList();

            LoadCommand = new RelayCommand<RoutedEventArgs>((args) =>
            {
                //await trkView.SyncDB();
                //Task.Run(async delegate()
                //{
                //    await trkView.SyncDB();
                //});

                //Log.GetLog().Write("Main page nav tooooollooooooo");
                //Logger.GetLogger().logChannel.LogMessage("Main page nav to");
                //var ts = await ApplicationData.Current.LocalFolder.GetFolderAsync("MyLogFile");
                //IReadOnlyList<StorageFile> lf = await ts.GetFilesAsync();
                // foreach (var item in lf)
                // {
                //     var ty = item.OpenReadAsync();
                //     string text = await Windows.Storage.FileIO.ReadTextAsync(item);               
                // }
            
            });
 
            RadioItemSelectedCommand = new RelayCommand<RadioGenre>((rg) =>
            {
                var rdoName = rg.RadioGenreName;
                //_navigationService.Navigate(typeof(RadioStreams), rdoName);
                _navigationService.NavigateTo("Radio", rdoName);
            });
            CollectionItemSelectedCommand = new RelayCommand<DataGroup>((gp) =>
            {
                var itemId = gp.UniqueId; 
                _navigationService.NavigateTo("Collection", itemId);
            });
            StreamingItemSelectedCommand = new RelayCommand<DataGroup>((gp) =>
            {
                
            });
            NowPlayingCommand = new RelayCommand(() =>
            {
                //_navigationService.Navigate(typeof(NowPlaying));   
                _navigationService.NavigateTo("NowPlaying");
            });
            BackUpCommand = new RelayCommand(() =>
            {
                repo.BackUpDb();
            });
            FillDbCommand = new RelayCommand(() =>
            {
                repo.fillDB();
            });
            TestCommand = new RelayCommand(() =>
            {
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

                
                repo.GetApiFillDB();
            });
        }

        public void Activate(object parameter)
        {
            string hu = "22";
        }

        public void Deactivate(object parameter)
        {
            string hu = "22";
        }

        private ObservableCollection<DataGroup> LoadCollectionList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup { Title = "Top Tracks", UniqueId = "Top Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Top Tracks", UniqueId = "Top Tracks", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Artists", UniqueId = "Artist", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Album", UniqueId = "Album", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Genre", UniqueId = "Genre", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Quick Pick", UniqueId = "QuickPick", ImagePath = "ms-appx:///Assets/music3.jpg" });
            return groups;
        }

        private ObservableCollection<DataGroup> LoadStreamingList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup("Stream", "Search GrooveShark", "music streaming", "ms-appx:///Assets/music.jpg"));           
            groups.Add(new DataGroup("Play Lists", "Play Lists", "music collection", "ms-appx:///Assets/music3.jpg"));
            groups.Add(new DataGroup("Create PlayList", "Create PlayList", "online radio streaming", "ms-appx:///Assets/radio.jpg"));
            return groups;
        }
    }

    public class DataGroup
    {
        public DataGroup()
        { }

        public DataGroup(String UniqueId, String title, String description, String imagePath)
        {
            this.UniqueId = UniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
        }
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
