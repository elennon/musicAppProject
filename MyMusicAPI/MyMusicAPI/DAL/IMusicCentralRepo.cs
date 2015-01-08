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
        List<RadioStream> GetAllStations();
        List<RadioGenre> GetAllStationGenres();
        void insertGenre(RadioGenre gn);
        void insertRadioStation(RadioStream rs);
    }
}
