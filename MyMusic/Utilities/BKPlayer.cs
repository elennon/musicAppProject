using MyMusic.DAL;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MyMusic.Utilities
{
    public class BKPlayer : INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private AutoResetEvent SererInitialized;

        public NowPlayingViewModel npi { get; set; }
        
        public string[] orders;
        private bool isPlayRadio = false, isPlayGSTrack = false;

        public BKPlayer()
        {
            SererInitialized = new AutoResetEvent(false);
            AddMediaPlayerEventHandlers();
        }

        public void PlayThese(string request)
        {
            var requestType = request.Split(',')[0];
            switch (requestType)
            {
                case "shuffleAll":
                    orders = repo.shuffleAll();
                    break;
                case "shuffleThese":
                    string id = (request.ToString().Split(',')[1]);
                    string type = (request.ToString().Split(','))[2];
                    if (type.Contains("album"))
                    {
                        orders = repo.ShuffleAlbum(id);
                    }
                    if (type.Contains("genre"))
                    {
                        orders = repo.ShuffleGenre(id);
                    }
                    if (type.Contains("topplay"))
                    {
                        orders = repo.ShuffleTopPlays();
                    }
                    if (type.Contains("qPick"))
                    {
                        orders = repo.ShuffleQuickPicks();
                    }
                    break;
                case "allTracks":
                    int trackNumber = Convert.ToInt32((request.ToString().Split(','))[1]);
                    orders = repo.GetListToPlay(trackNumber);
                    break;
                case "albumTracks":
                    int albumId = Convert.ToInt32((request.ToString().Split(','))[1]);
                    orders = GetSongsAllInAlbum(albumId);
                    break;
                case "albTracksFromThisOn":
                    int trackId = Convert.ToInt32((request.ToString().Split(','))[1]);
                    int albmId = Convert.ToInt32((request.ToString().Split(','))[2]);
                    orders = GetSongsInAlbumFromThis(trackId, albmId);
                    break;
                case "artistTracks":
                    int artistId = Convert.ToInt32((request.ToString().Split(','))[1]);
                    orders = GetSongsByThisArtist(artistId);
                    break;
                case "radio":
                    orders = new string[1];
                    orders[0] = (request.ToString().Split(','))[1];
                    isPlayRadio = true;
                    break;
                case "gsStreamTrack":
                    orders = new string[1];
                    orders[0] = (request.ToString().Split(','))[1];
                    isPlayGSTrack = true;
                    break;
                case "playlist":
                    int playlistId = Convert.ToInt32((request.ToString().Split(','))[1]);
                    orders = repo.GetPlayListToPlay(playlistId);
                    break;
            }
            if (((App)Application.Current).IsMyBackgroundTaskRunning)
            {
                Task.Run(delegate()
                {
                    if (isPlayRadio == true)
                    {
                        var message = new ValueSet();
                        message.Add(Constants.PlayRadio, orders);
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                    else
                    {
                        var message = new ValueSet();
                        message.Add(Constants.StartPlayback, orders);
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                });
            }
            else
            {
                StartBackgroundAudioTask();     // else start it up
            }
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

        #region Background messages

        void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {           
            Debug.WriteLine("message recieved loud and clear  ");
            Track tr = new Track();
            //string artist = "", title = "";
            int trackId = 0;
            string[] currentTrack = (e.Data.Values.FirstOrDefault().ToString()).Split(',');     // current track will be a comma seperated string with name, artist...
            if (currentTrack[0] != string.Empty && currentTrack[0] != "True")
            {
                if (currentTrack.Length > 1)
                {
                    trackId = Convert.ToInt32(currentTrack[0]);
                    tr = repo.GetThisTrack(trackId);
                    //artist = currentTrack[1];
                    //title = currentTrack[2];
                    if (currentTrack[3].Contains("shuffle")) //  if its a track from random shuffle
                    {
                        repo.AddRandomPlay(tr.TrackId);
                    }
                    else if (currentTrack[3].Contains("notShuffle"))
                    {
                        repo.AddPlay(tr.TrackId); // if it was chosen specifically
                    }
                    if (currentTrack.Count() > 4) //  if its got the extra word, its a skipped track
                    {
                        repo.AddSkip(trackId);
                    }
                }
                else
                {
                    tr.Artist = "Radio :";
                    tr.Name = currentTrack[0];
                    //tbkSongName.Text = currentTrack[0];   // this is a radio station
                }
            }
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        string pic = tr.ImageUri;
                        if (string.IsNullOrEmpty(pic)) { pic = "ms-appx:///Assets/radio672.png"; }
                        npi.TrImage = pic;    // the image for this song
                        npi.TrackName = tr.Artist + "-" + tr.Name;                                                                   
                        break;
                    case Constants.BackgroundTaskStarted:
                        SererInitialized.Set();
                        break;
                    case Constants.PlayRadioFailed:
                        npi.TrackName = "radio don't play good";
                        break;
                }
            }
        }

        #endregion

        #region setup/start background task

        public void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        public void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void StartBackgroundAudioTask()             // starts background---sends mesaage (tracks to play) from here
        {
            var backgroundtaskinitializationresult = Task.Run(delegate()
            {
                //if (((App)Application.Current).IsMyBackgroundTaskRunning && BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Closed)
                //{                   
                    if (isPlayRadio == true)
                    {
                        var message = new ValueSet();
                        message.Add(Constants.PlayRadio, orders);
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                        isPlayRadio = false;
                    }
                    else if (isPlayGSTrack == true)
                    {
                        var message = new ValueSet();
                        message.Add(Constants.PlayGSTrack, orders);
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                        isPlayGSTrack = false;
                    }
                    else
                    {
                        var message = new ValueSet();
                        message.Add(Constants.StartPlayback, orders);
                        Debug.WriteLine("FG: sending Message");
                        BackgroundMediaPlayer.SendMessageToBackground(message);
                    }
                //}                                   
            });
            backgroundtaskinitializationresult.AsAsyncAction().Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }

        #endregion

        public void SendMessageTpBK(ValueSet value)
        {           
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        public void playClick()
        {
            if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
            {
                BackgroundMediaPlayer.Current.Pause();
            }
            else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
            {
                BackgroundMediaPlayer.Current.Play();
            }
            else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
            {
                StartBackgroundAudioTask();
            }

            else
            {
                StartBackgroundAudioTask();
            }
        }

        #region INotifyPropertyChanged

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
