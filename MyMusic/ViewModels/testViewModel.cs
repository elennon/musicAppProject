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
   
    public class testViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        
        private ObservableCollection<Track> _tracks;
        public ObservableCollection<Track> Tracks
        {
            get
            {
                return _tracks;
            }
            set
            {
                if (_tracks != value)
                {
                    _tracks = value;
                    NotifyPropertyChanged("Tracks");
                }
            }
        }

        public RelayCommand<Track> ItemSelectedCommand { get; set; }
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand DeleteIconCommand { get; set; }

        public testViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Tracks = repo.GetTracks();
            ItemSelectedCommand = new RelayCommand<Track>((tr) =>
            {
                Tracks.Remove(tr);
            });
            LoadCommand = new RelayCommand<RoutedEventArgs>((args) =>
            {
                
            });
            DeleteIconCommand = new RelayCommand(() =>
            {
                string bb = "";
            });
        }
      
        public void Activate(object parameter)
        {
            
        }

        public void Deactivate(object parameter)
        {
            string hu = "22";
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
