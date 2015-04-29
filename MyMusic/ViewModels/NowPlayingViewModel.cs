using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MyMusic.Common;
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
using Windows.Media.Playback;
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
       
        private double _duration = 200;
        public double duration
        {
            get { return _duration; }
            set { _duration = value; NotifyPropertyChanged("duration"); }
        }

        private double _trackProgress = 0;
        public double TrackProgress
        {
            get { return _trackProgress; }
            set { _trackProgress = value; NotifyPropertyChanged("TrackProgress"); }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
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
        public RelayCommand LikeCommand { get; set; }
        public RelayCommand DislikeCommand { get; set; }

        public NowPlayingViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.PlayPauseCommand = new RelayCommand(OnPlayPauseCommand);
            this.NextCommand = new RelayCommand(OnNextCommand);
            this.BackCommand = new RelayCommand(OnBackCommand);
            this.LikeCommand = new RelayCommand(OnLikeCommand);
            this.DislikeCommand = new RelayCommand(OnDislikeCommand);
            ((App)Application.Current).BkPlayer.npi = this;
            //NowPlayingInst = this;
        }

        private void OnLikeCommand()
        {
            var trac = ((App)Application.Current).BkPlayer.CurrentTrack.TrackId;
            repo.AddLike(trac, true);
        }

        private void OnDislikeCommand()
        {
            var trac = ((App)Application.Current).BkPlayer.CurrentTrack.TrackId;
            repo.AddLike(trac, false);
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {
            if (((App)Application.Current).isResumingFromTermination == false)  // this if in normal mode (not resuming after termination/suspention)
            {
                //   condition 1: nothing currently playing + there is tracks to play
                if (BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Playing && string.IsNullOrEmpty(NavigateParam) == false)
                {
                    SortPlayList(NavigateParam);
                }
                //   condition 2: nothing currently playing + there wasn't any tracks selected to play
                else if (BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Playing && string.IsNullOrEmpty(NavigateParam) == true)
                {
                    TrackName = "nothing playing";
                }
                //   condition 3: BK currently playing + there is tracks to play
                else if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing && string.IsNullOrEmpty(NavigateParam) == false)
                {
                    SortPlayList(NavigateParam);
                }
                //   condition 4: BK currently playing + there wasn't any tracks selected to play
                else if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing && string.IsNullOrEmpty(NavigateParam) == true)
                {
                    string pic = "";
                    object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                    if (value1 == null)
                    {
                        TrackName = "";
                    }
                    if (value1 != null)
                    {
                        TrackName = (string)value1;
                    }

                    object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
                    if (value2 == null) { pic = "ms-appx:///Assets/misc.png"; }
                    else
                    {
                        int trackId = (int)value2;
                        pic = (repo.GetThisTrack(trackId)).ImageUrl;
                        if (pic == "") { pic = "ms-appx:///Assets/misc.png"; }
                        TrImage = pic;
                    }
                }
                NavigateParam = "";
            }
            else           // this if resuming after termination/suspention. just need to show track if playing or show nothing playing)
            {
                if (BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Playing )
                {
                    PlayPause = new SymbolIcon(Symbol.Play);                    
                    TrackName = "nothing playing";
                }                
                else if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                {
                    PlayPause = new SymbolIcon(Symbol.Pause);
                    string pic = "";
                    object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                    if (value1 == null)
                    {
                        TrackName = "";
                    }
                    if (value1 != null)
                    {
                        TrackName = (string)value1;
                    }

                    object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
                    if (value2 == null) { pic = "ms-appx:///Assets/misc.png"; }
                    else
                    {
                        int trackId = (int)value2;
                        pic = (repo.GetThisTrack(trackId)).ImageUrl;
                        if (pic == "") { pic = "ms-appx:///Assets/misc.png"; }
                        TrImage = pic;
                    }
                }
            }
        }

        private void SortPlayList(string NavigateParameter)
        {
            IsVisible = true;
            ((App)Application.Current).BkPlayer.PlayThese(NavigateParameter);
        }

        #region button clicks

        private void OnBackCommand()
        {
            ((App)Application.Current).BkPlayer.okStarted = false;
            var value = new ValueSet();
            value.Add(Constants.SkipPrevious, "");
            ((App)Application.Current).BkPlayer.SendMessageToBK(value);
        }

        private void OnNextCommand()
        {
            ((App)Application.Current).BkPlayer.okStarted = false;
            var value = new ValueSet();
            value.Add(Constants.SkipNext, "");
            ((App)Application.Current).BkPlayer.SendMessageToBK(value);
        }

        private void OnPlayPauseCommand()
        {
            if (playSwitch) { PlayPause = new SymbolIcon(Symbol.Play); }
            else { PlayPause = new SymbolIcon(Symbol.Pause); }

            ((App)Application.Current).BkPlayer.playClick(NavigateParam);
            playSwitch = !playSwitch;
        }

        #endregion

        #region playlist managing

        private string[] GetSongsAllInAlbum(int albumId) // orders all songs in album into a string[]
        {
            var trks = (repo.GetTracksByAlbum(albumId.ToString())).ToList();         // get all tracks in given album
            string[] trkArray = new string[trks.Count];

            for (int i = 0; i < trks.Count; i++)
            {
                trkArray[i] = trks[i].TrackId.ToString() + "," + trks[i].FileName + "," + trks[i].ArtistName + ",notShuffle";
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
                    tracks.Add(item.TrackId.ToString() + "," + item.FileName + "," + item.ArtistName + ",notShuffle");
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
                trks[i] = tracks[i].TrackId.ToString() + "," + tracks[i].FileName + "," + tracks[i].ArtistName + ",notshuffle";
            }
            return trks;
        }

        #endregion
       
        public void Activate(object parameter)
        {
            if (parameter != null)
            NavigateParam = parameter.ToString();
        }

        public void Deactivate(object parameter)
        {
           
            NavigateParam = "";
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



  //// condition 1: normal state + tracks sent here to play // && ((App)Application.Current).isResumingFromTermination == false
  //          if (string.IsNullOrEmpty(NavigateParam) == false && BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Closed)   
  //          {
  //              SortPlayList(NavigateParam);
  //          }
  //          else if (((App)Application.Current).IsMyBackgroundTaskRunning && BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing) //// condition 2: normal state + no tracks to play(either user navigated here or resuming from termination)
  //          {
  //              string pic = "";
  //              object value1 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
  //              if (value1 == null)
  //              {
  //                  TrackName = "current Track null ( in NP nav to(condition 2) )";
  //              }
  //              if (value1 != null)
  //              {
  //                  TrackName = (string)value1 + "( in Np (condition 2) )";
  //              }

  //              object value2 = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.TrackIdNo);
  //              if (value2 == null) { pic = "ms-appx:///Assets/radio672.png"; }
  //              else
  //              {
  //                  int trackId = (int)value2;
  //                  pic = (repo.GetThisTrack(trackId)).ImageUrl;
  //                  if (pic == "") { pic = "ms-appx:///Assets/radio672.png"; }
  //                  TrImage = pic;
  //              }
  //          }
  //          else
  //              TrackName = "nothing playing";      /// condition 3: nothing playing