﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyMusic.ViewModels.StreamingPlaylists
{ 
    public class GSMainPageViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;

        public ObservableCollection<DataGroup> Methods { get; set; }
        //public ObservableCollection<Artist> artists { get; set; }

        private ObservableCollection<Artist> _artists;
        public ObservableCollection<Artist> Artists
        {
            get
            {
                return _artists;
            }
            set
            {
                if (_artists != value)
                {
                    _artists = value;
                    NotifyPropertyChanged("Artists");
                }
            }
        }

        private string _session;
        public string Session
        {
            get { return _session; }
            set { _session = value; NotifyPropertyChanged("Session"); }
        }
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RoutedEventArgs> MethodSelectedCommand { get; set; }
        
        public GSMainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Methods = LoadCollectionList();
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.MethodSelectedCommand = new RelayCommand<RoutedEventArgs>(OnMethodSelectedCommand);
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {
            Artists = await repo.GetListArtists(Session);            
        }

        private void OnMethodSelectedCommand(RoutedEventArgs obj)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<DataGroup> LoadCollectionList()
        {
            ObservableCollection<DataGroup> groups = new ObservableCollection<DataGroup>();
            groups.Add(new DataGroup { Title = "Method One", UniqueId = "Method1", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Method Two", UniqueId = "Method2", ImagePath = "ms-appx:///Assets/music3.jpg" });
            groups.Add(new DataGroup { Title = "Method Three", UniqueId = "Method3", ImagePath = "ms-appx:///Assets/music3.jpg" });
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

        public void Activate(object parameter)
        {
            Session = parameter.ToString();
        }

        public void Deactivate(object parameter)
        {
            
        }

        
    }
}
