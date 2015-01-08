using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMusicAPI.DAL
{
    public class MusicCentralRepo : IMusicCentralRepo
    {
        public MusicCentralContext cd;

        public MusicCentralRepo()
        {
            cd = new MusicCentralContext();
        }

        public List<RadioStream> GetAllStations()
        {
            return (List<RadioStream>)cd.RadioStream.ToList();
        }   

        public List<RadioGenre> GetAllStationGenres()
        {
            return (List<RadioGenre>)cd.RadioGenre.ToList();
        }  

        public void insertGenre(RadioGenre gn)
        {
            cd.RadioGenre.Add(gn);
            cd.SaveChanges();
        }

        public void insertRadioStation(RadioStream rs)
        {
            cd.RadioStream.Add(rs);
            cd.SaveChanges();
        }


    }
}