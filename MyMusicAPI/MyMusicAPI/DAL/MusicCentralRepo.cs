using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMusicAPI.DAL
{
    public class MusicCentralRepo : IMusicCentralRepo
    {
        public MusicCentralDBEntities cd; 

        public MusicCentralRepo()
        {           
            cd = new MusicCentralDBEntities();
            //cd = new EfMusic();
        }

        public IEnumerable<RadioStream> GetAllStations()
        {
            var sts = cd.RadioStreams.Where(a => a.TestedOk == true && a.Status == 1);
            return sts;            
        }

        public void DropDb()
        {            
            var sts = cd.RadioStreams.ToList();
            var gns = cd.RadioGenres.ToList();
            foreach (var item in sts)
            {
                cd.RadioStreams.Remove(item);
            }
            foreach (var item in gns)
            {
                cd.RadioGenres.Remove(item);
            }
            cd.SaveChanges();
        }

        public IEnumerable<RadioGenre> GetAllStationGenres()
        {
            var gns = cd.RadioGenres;
            return gns;
        }

        public RadioGenre GetGenre(string gName)
        {
            var gn = cd.RadioGenres.Where(a => a.RadioGenreName == gName).FirstOrDefault();
            return gn;
        }

        public void insertGenre(RadioGenre gn)
        {
            var gns = cd.RadioGenres.Where(a => a.RadioGenreName == gn.RadioGenreName).Count();
            if (gns < 1)
            {
                cd.RadioGenres.Add(gn);
                cd.SaveChanges();
            }
        }

        public bool CheckGenre(string gn)
        {
            bool isNew = false;
            var genres = cd.RadioGenres.ToList();
            var gCount = genres.Where(a => a.RadioGenreName == gn).Count();
            if (gCount < 1) isNew = true;
            
            return isNew;
        }

        public void AddGenreKey(string name, string key)
        {
            var genres = cd.RadioGenres.ToList();
            RadioGenre g = genres.Where(a => a.RadioGenreName == name).FirstOrDefault();
            if (g != null)
            {
                g.RadioGenreKey = key;
               
                cd.SaveChanges();
            }
        }

        public void insertRadioStation(RadioStream rs)
        {
            try
            {
                var sts = cd.RadioStreams.Where(a => a.RadioUrl == rs.RadioUrl).Count();
                if (sts < 1)
                {
                    RadioGenre gn = cd.RadioGenres.Where(a => a.RadioGenreName == rs.RadioGenreName).FirstOrDefault();
                    if (gn.RadioStreams == null) { gn.RadioStreams = new List<RadioStream>(); }
                    gn.RadioStreams.Add(rs);
                    cd.RadioStreams.Add(rs);
                    cd.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStation(RadioStream st, bool isOk)
        {
            st.TestedOk = isOk;
            cd.SaveChanges();
        }

        public void DeleteGenreAndSts(string gNme)
        {
            var gnr = cd.RadioGenres.Where(a => a.RadioGenreName == gNme).FirstOrDefault();
            cd.RadioGenres.Remove(gnr);
            var sts = cd.RadioStreams.Where(a => a.RadioGenreName == gNme).ToList();
            foreach (var item in sts)
            {
                cd.RadioStreams.Remove(item);
            }
            cd.SaveChanges();
        }

        protected void Dispose(bool disposing)
        {
            cd.Dispose();
            //base.Dispose(disposing);
        }
    }
}