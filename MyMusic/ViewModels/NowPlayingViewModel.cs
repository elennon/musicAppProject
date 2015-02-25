using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.DAL;
using MyMusic.Models;
using MyMusic.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MyMusic.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private string NavigateParam = "";
        private bool playSwitch = false; 

        private string _trackName = "";
        public string TrackName
        {
            get { return _trackName; }
            set { _trackName = value; NotifyPropertyChanged("TrackName"); }
        }

        private string _image;
        public string TrImage
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("TrImage"); }
        }

        private SymbolIcon _playPause = new SymbolIcon(Symbol.Pause);
        public SymbolIcon PlayPause
        {
            get { return _playPause; }
            set { _playPause = value; NotifyPropertyChanged("PlayPause"); }
        }

        public static NowPlayingViewModel NowPlayingInst;
        
        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand PlayPauseCommand { get; set; }
        public RelayCommand NextCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

        public NowPlayingViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.PlayPauseCommand = new RelayCommand(OnPlayPauseCommand);
            this.NextCommand = new RelayCommand(OnNextCommand);
            this.BackCommand = new RelayCommand(OnBackCommand);
            ((App)Application.Current).BkPlayer.npi = this;
            //NowPlayingInst = this;
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
            //NavigateParam = obj.ToString();
            if (string.IsNullOrEmpty(NavigateParam) == false && ((App)Application.Current).isResumingFromTermination == false)   // condition 1: normal state + tracks sent here to play
            {
                SortPlayList(NavigateParam);
            }
            else if (((App)Application.Current).IsMyBackgroundTaskRunning ) //// condition 2: normal state + no tracks to play(either user navigated here or resuming from termination)
            {
                string pic = "";
                object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                if (value1 == null)
                {
                    TrackName = "current Track null ( in NP nav to (condition 2) )";
                }
                if (value1 != null)
                {
                    TrackName = (string)value1 + "( in NP nav to )";
                }

                object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
                if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
                else
                {
                    int trackId = (int)value2;
                    pic = (repo.GetThisTrack(trackId)).ImageUri;
                    if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
                    TrImage = pic;
                }
            }
            else
                TrackName = "nothing playing";      /// condition 3: nothing playing
        }

        private void SortPlayList(string NavigateParameter)
        {
            ((App)Application.Current).BkPlayer.PlayThese(NavigateParameter);
        }

        private void OnBackCommand()
        {
            var value = new ValueSet();
            value.Add(Constants.SkipPrevious, "");
            ((App)Application.Current).BkPlayer.SendMessageTpBK(value);
        }

        private void OnNextCommand()
        {
            var value = new ValueSet();
            value.Add(Constants.SkipNext, "");
            ((App)Application.Current).BkPlayer.SendMessageTpBK(value);
        }

        private void OnPlayPauseCommand()
        {
            if (playSwitch) { PlayPause = new SymbolIcon(Symbol.Play); }
            else { PlayPause = new SymbolIcon(Symbol.Pause); }
            ((App)Application.Current).BkPlayer.playClick();
            playSwitch = !playSwitch;
        }


        #region playlist managing

        private string[] GetSongsAllInAlbum(int albumId) // orders all songs in album into a string[]
        {
            var trks = (repo.GetTracksByAlbum(albumId.ToString())).ToList();         // get all tracks in given album
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].FileName + "," + trks[i].Artist + ",notShuffle";
            }
            return trkArray;
        }

        private string[] GetSongsInAlbumFromThis(int trackId, int albumId) // orders all songs in album into a string[]
        {
            List<string> tracks = new List<string>();
            var trks = repo.GetTracksByAlbum(albumId.ToString());    // get all tracks in given album
            bool yes = false;
            foreach (var item in trks)                                  // then only take the selected song and all after it in the list
            {
                if (item.TrackId == trackId) { yes = true; }
                if (yes)
                {
                    tracks.Add(item.TrackId.ToString() + "," + item.FileName + "," + item.Artist + ",notShuffle");
                }
            }

            string[] trkArray = new string[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                trkArray[i] = tracks[i];
            }
            return trkArray;
        }

        private string[] GetSongsByThisArtist(int id)
        {
            ObservableCollection<Track> tracks = repo.GetTracksByArtist(id);
            string[] trks = new string[tracks.Count];
            for (int i = 0; i < tracks.Count; i++)
            {
                trks[i] = tracks[i].TrackId.ToString() + "," + tracks[i].FileName + "," + tracks[i].Artist + ",notshuffle";
            }
            return trks;
        }

        #endregion
       
        public void Activate(object parameter)
        {
            NavigateParam = parameter.ToString();
        }

        public void Deactivate(object parameter)
        {

        }

        #region INotifyPropertyChanged 

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            OnUIThread(() =>
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            });
        }

        #endregion

        protected async void OnUIThread(DispatchedHandler onUIThreadDelegate)
        {
            if (onUIThreadDelegate != null)
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                if (dispatcher.HasThreadAccess)
                {
                    onUIThreadDelegate();
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, onUIThreadDelegate);
                }
            }
        }

       
    }
}
