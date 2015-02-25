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
    public class ViewPlaylistViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        //public Playlist thisPlaylist = new Playlist();
        public Playlist thisPlaylist { get; set; }

        private ObservableCollection<Track> _playlistTracks;
        public ObservableCollection<Track> PlaylistTracks
        {
            get
            {
                return _playlistTracks;
            }
            set
            {
                if (_playlistTracks != value)
                {
                    _playlistTracks = value;
                    NotifyPropertyChanged("PlaylistTracks");
                }
            }
        }

        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Object> ItemSelectedCommand { get; set; }

        public GalaSoft.MvvmLight.Command.RelayCommand AddCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand DeleteCommand { get; set; }

        public ViewPlaylistViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<object>(OnItemSelectedCommand);
            this.AddCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddCommand);
            this.DeleteCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnDeleteCommand);
        }

        private void OnAddCommand()
        {
            string paramater = "add," + thisPlaylist.PlaylistId.ToString();
            _navigationService.NavigateTo("AddToPlaylist", paramater);
        }

        private void OnDeleteCommand()
        {
            string paramater = "delete," + thisPlaylist.PlaylistId.ToString();
            _navigationService.NavigateTo("AddToPlaylist", paramater);
        }

        private void OnItemSelectedCommand(object obj)
        {
            //throw new NotImplementedException();      
        }

        private void OnLoadCommand(RoutedEventArgs obj)
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

        public void Activate(object parameter)
        {
            if(parameter != null)
            {
                var playlistId = (int)parameter;         
                thisPlaylist = repo.GetThisPlaylist(playlistId);
                PlaylistTracks = repo.GetPlaylistTracks(thisPlaylist);
            }            
        }

        public void Deactivate(object parameter)
        {
            
        }

        
    }
}
