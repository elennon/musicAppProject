using GalaSoft.MvvmLight;
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

namespace MyMusic.ViewModels
{
    public class SavedPlaylistsViewModel : ViewModelBase, INavigable , INotifyPropertyChanged
    {
        private Repository repo = new Repository();
        private INavigationService _navigationService;
        private bool InEdit = false;

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
        }

        private ObservableCollection<Playlist> _playlists;
        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; NotifyPropertyChanged("Playlists"); }
        }
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand CreateCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand<Playlist> PlaylistDeleteSelectedCommand { get; set; }
        public RelayCommand<Playlist> ItemSelectedCommand { get; set; }
        public RelayCommand<Object> PlayItemTapCommand { get; set; }

        public SavedPlaylistsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.CreateCommand = new RelayCommand(OnCreateCommand);
            this.EditCommand = new RelayCommand(OnEditCommand);
            this.PlaylistDeleteSelectedCommand = new RelayCommand<Playlist>(OnPlaylistDeleteSelectedCommand);
            this.ItemSelectedCommand = new RelayCommand<Playlist>(OnItemSelectedCommand);
            this.PlayItemTapCommand = new RelayCommand<Object>(OnPlayItemTapCommand);
        }

        private void OnPlayItemTapCommand(Object obj)       // tap the headphones image to play all tracks by artist/album
        {
            if (obj is Playlist)
            {
                string playThese = "playlist," + ((Playlist)obj).PlaylistId;
                _navigationService.NavigateTo("NowPlaying", playThese);
            }            
        }


        private void OnItemSelectedCommand(Playlist obj)
        {
            if (!InEdit)
            {
                _navigationService.NavigateTo("ViewPlaylist", obj.PlaylistId);
            }
        }

        private void OnPlaylistDeleteSelectedCommand(Playlist obj)
        {
            Playlists.Remove(obj);
            repo.RemovePlaylist(obj);
        }

        private void OnEditCommand()
        {
            InEdit = !InEdit;
            IsVisible = !IsVisible;      
        }

        private void OnCreateCommand()
        {           
            _navigationService.NavigateTo("CreatePlaylist");
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
            Playlists = repo.GetPlaylists();     
        }

        public void Activate(object parameter)
        {
            
        }

        public void Deactivate(object parameter)
        {
            
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
