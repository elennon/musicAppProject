using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyMusic.ViewModels.StreamingPlaylists
{
    
   public class GSSignInViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;

        private string _Name = "";
        public string UserName
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("UserName"); }
        }
        private string _pWord = "";
        public string PWord
        {
            get { return _pWord; }
            set { _pWord = value; NotifyPropertyChanged("PWord"); }
        }
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RoutedEventArgs> GSLinkCommand { get; set; }
        public RelayCommand<RoutedEventArgs> SignInCommand { get; set; }

        public GSSignInViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.GSLinkCommand = new RelayCommand<RoutedEventArgs>(OnGSLinkCommand);
            this.SignInCommand = new RelayCommand<RoutedEventArgs>(OnSignInCommand);
        }

        private async void OnSignInCommand(RoutedEventArgs obj)
        {
            if (UserName == "") {  UserName = "kilmaced"; }
            if (PWord == "") { PWord = "Rhiabit1"; }
            var session = await repo.GetGSSessionId(UserName, PWord);
                    
            ApplicationSettingsHelper.SaveSettingsValue(Constants.GSSessionId, session);
           
            _navigationService.NavigateTo("GSMainPage", session);
        }

        private void OnGSLinkCommand(RoutedEventArgs obj)
        {
            throw new NotImplementedException();//link to gs
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
