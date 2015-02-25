using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.DAL
{
    public interface IRepository
    {
        ObservableCollection<Track> GetAllTracks();
        ObservableCollection<Track> GetTracks();
        Track GetThisTrack(int id);
        ObservableCollection<Track> GetTracksLessQpicks();
        ObservableCollection<Track> GetQuickPicks();
        ObservableCollection<Track> GetTopTracks();
        ObservableCollection<Track> GetBinnedTracks();
        ObservableCollection<Track> GetTracksByArtist(int id);
        ObservableCollection<Track> GetTracksByAlbum(string albId);
        ObservableCollection<Track> GetTracksByGenre(string genId);
        string[] GetListToPlay(int startPos);
        
        void AddRandomPlay(int id);
        void AddPlay(int id);
        void AddSkip(int trackId);
        void AddThisToQuickPick(int trackId);
        void OutFromQuickPick(int trackId);
        void BinThis(int trackId);
        void BackIn(int trackId);
        void DoPercent();

        string[] shuffleThese(ObservableCollection<Track> shfThese);
        string[] shuffleAll();
        string[] ShuffleAlbum(string id);
        string[] ShuffleGenre(string id);
        string[] ShuffleTopPlays();
        string[] ShuffleQuickPicks();

        ObservableCollection<Artist> GetArtists();
        ObservableCollection<Album> GetAlbums();
        ObservableCollection<Album> GetAlbumsByArtist(string id);
        ObservableCollection<Genre> GetGenres();

        ObservableCollection<RadioGenre> GetRadioGenres();
        ObservableCollection<RadioStream> GetRadioStations();

        int CreatePlaylist(Playlist pl);
        ObservableCollection<Playlist> GetPlaylists();
        Playlist GetThisPlaylist(int id);
        ObservableCollection<Track> GetPlaylistTracks(Playlist pl);
        ObservableCollection<Track> GetTracksLessThisPlaylist(Playlist pl);
        string[] GetPlayListToPlay(int id);

        void AddToPlaylist(int playlistId, int trackId);
        void RemoveFromPlaylist(int playlistId, int trackId);
        void RemovePlaylist(Playlist pl);

        void BackUpDb();
        void GetApiFillDB();
        void fillDB();
        
    }
}
