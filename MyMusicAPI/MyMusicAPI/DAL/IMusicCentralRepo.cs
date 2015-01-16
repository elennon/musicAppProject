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
        void DropDb();
    }
}
