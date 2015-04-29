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
        Track GetThisTrack(string artist, string name);
        ObservableCollection<Track> GetTracksLessQpicks();
        ObservableCollection<Track> GetQuickPicks();
        ObservableCollection<Track> GetQuickPicksWithTempo();
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
        void AddLike(int trackId, bool ifLiked);
        int DoPercent(Track tr);
        void DoAllPercent();

        string[] SortGSListToArray(ObservableCollection<Track> gsList);
        string[] ShuffleGSListToArray(ObservableCollection<Track> gsList);
        string[] shuffleThese(ObservableCollection<Track> shfThese);
        string[] shuffleAll();
        string[] ShuffleAlbum(string id);
        string[] ShuffleGenre(string id);
        string[] ShuffleTopPlays();
        string[] ShuffleQuickPicks();
        string[] ShufflePlaylist(List<Track> list);
        string[] TracksToArray(List<Track> trks);

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
        Task<List<Track>> GetPrologList();

        ///     api calls
        Task<string> GetGSSessionId(string nme, string pword);
        Task<ObservableCollection<Artist>> GetSimilarArtists(string sessionId, string artist, int top);
        Task<Track> GetGrooveSharkTrackUrl(string artist, string track, string sessionId);
        Task<ObservableCollection<Track>> GetDeezerArtistTracks(string artist, int topNo, string sessionId);

        Task<ObservableCollection<Artist>> GetSimilarLastFmArtists(string artist, int top);
        Task<ObservableCollection<Track>> GetSimilarLastFmTracks(string artist, int num, string sessionId);

        Task GetEchoNestInfo(string artist, string track);
        void AddToPlaylist(int playlistId, int trackId);
        void RemoveFromPlaylist(int playlistId, int trackId);
        void RemovePlaylist(Playlist pl);

        //void BackUpDb();
        Task FillRadioDB();
        
        Task fillDB3();       
        Task fillDbFromXml();
        
        Task SortPics();
        Task AddArtistName();
        Task SyncWithApi();     
        Task SyncDB();
    }
}
