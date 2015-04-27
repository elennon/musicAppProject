using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.Common;
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
    public class CreatePlaylistViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;

        public string PlaylistName { get; set; }
        public string Description { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        
        public GalaSoft.MvvmLight.Command.RelayCommand SavePlaylistCommand { get; set; }

        public CreatePlaylistViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);            
            this.SavePlaylistCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnSavePlaylistCommand);           
        }

        private void OnSavePlaylistCommand()
        {
            var pl = new Playlist { Name = PlaylistName, Description = Description };
            int plId = repo.CreatePlaylist(pl);
            string para = "add," + plId.ToString();
            _navigationService.NavigateTo("AddToPlaylist", para);
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
            
        }

        public void Deactivate(object parameter)
        {
            
        }        
    }
}