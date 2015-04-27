using MyMusicAPI.Controllers;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusicAPI.DAL
{
    public interface IMusicCentralRepo
    {
        void AddUser(string userId);
        bool CheckIfNewUser(string userId);
        void CheckUserTracks(string userId, List<Track> trks);

        //void FillPrologDb();
        List<Track> GetSameTaste(string id);
        List<Track> getTracks();
        List<Track> getTracks(string id);
        int getTrackCount(string userId);

        //string test();
        IEnumerable<RadioStream> GetAllStations();
        IEnumerable<RadioGenre> GetAllStationGenres();
        RadioGenre GetGenre(string gName);
        void DeleteGenreAndSts(string gNme);
        
        bool CheckGenre(string gn);
        void AddGenreKey(string name, string key);
        void insertGenre(RadioGenre gn);
        void insertRadioStation(RadioStream rs);
        void UpdateStation(RadioStream st, bool isOk);
        void SeedDb();
        //void SeedDb();
        void DropDb();
    }
}
