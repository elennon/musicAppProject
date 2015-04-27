using alice.tuprolog;
using alice.tuprolog.@event;
using java.io;
using MyMusicAPI.Controllers;
using MyMusicAPI.Helper_Classes;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace MyMusicAPI.DAL
{
    public class MusicCentralRepo : IMusicCentralRepo
    {
        public MusicCentralDBEntities cd; 
        private static DirectoryInfo dir;
        private static string baseDir = Environment.CurrentDirectory;

        public MusicCentralRepo()
        {           
            cd = new MusicCentralDBEntities();
        }

        #region User tracks

        public void DropDb()
        {
            var arts = cd.Artists.ToList();
            var trs = cd.Tracks.ToList();
            var users = cd.Users.ToList();
            var userTrs = cd.UserTracks.ToList();
            foreach (var item in userTrs)
            {
                cd.UserTracks.Remove(item);
            }
            foreach (var item in users)
            {
                cd.Users.Remove(item);
            }
            foreach (var item in arts)
            {
                cd.Artists.Remove(item);
            }
            foreach (var item in trs)
            {
                cd.Tracks.Remove(item);
            }
            cd.SaveChanges();
            var axxrts = cd.Artists.ToList();
            var tcrs = cd.Tracks.ToList();
            var uers = cd.Users.ToList();
            var usverTrs = cd.UserTracks.ToList();
        }

        public void BackUpDb()
        {
            List<UserTrackList> allusers = new List<UserTrackList>();
            foreach (var user in cd.Users)
            {
                UserTrackList ut = new UserTrackList();
                var ids = cd.UserTracks.Where(a => a.UserId == user.Id).Select(b => b.TrackId).ToList();
                List<Track> trs = cd.Tracks.Where(a => ids.Contains(a.Id)).ToList();
                ut.Songs = DtoConverter.TrackToDto(trs);
                ut.UserId = user.UserId;
                allusers.Add(ut);
            }            
            XmlSerializer xs = new XmlSerializer(typeof(UserTrackList));
            using (Stream str = new FileStream(HttpContext.Current.Server.MapPath(@"~/App_Data/Backup.xml"),
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xs.Serialize(str, allusers);
            }
        }

        public void SeedDb()
        {            
            UserTrackList unt = new UserTrackList();
            XmlSerializer xs = new XmlSerializer(typeof(UserTrackList));
            using (Stream str = System.IO.File.OpenRead(HttpContext.Current.Server.MapPath(@"~/App_Data/seed.xml")))
            {
                unt = (UserTrackList)xs.Deserialize(str);
            }
            foreach (var item in DtoConverter.DtoTotracks(unt.Songs))
            {
                AddTrack(item);
            }
            List<User> users = new List<User>();
            List<Track> tracks = new List<Track>();
            for (int i = 1; i < 6; i++)
            {
                User user = new User { UserId = "user" + i.ToString() };
                AddUser(user.UserId);
                cd.SaveChanges();
            }            
            users = cd.Users.ToList();

            tracks = cd.Tracks.ToList();
            Random rnd = new Random();
            foreach (var item in users)
            {
                int rn = rnd.Next(400);                     // add a random # of tracks to each user 
                IEnumerable<Track> trks = tracks.Take(rn);
                foreach (var tr in trks)
                {
                    var ut = new UserTrack { UserId = item.Id, TrackId = tr.Id };
                    cd.UserTracks.Add(ut);
                    cd.SaveChanges();
                }
            }
        }

        public List<Track> GetSameTaste(string id)
        {
            MyOutputListener ol = new MyOutputListener();
            try
            {
                SolveInfo info;
                java.io.InputStream stream = new FileInputStream(HttpContext.Current.Server.MapPath(@"~/App_Data/sameTaste.pl"));
                Theory t = new Theory(stream);
                alice.tuprolog.Prolog engine = new alice.tuprolog.Prolog();
                engine.setTheory(t);

                engine.addOutputListener(ol);
                engine.addSpyListener(new MySpyListener());
                engine.addExceptionListener(new MyExcptioListener());

                var users = cd.Users.Where(a => a.UserId != id).ToList();
                var uts = cd.UserTracks.ToList();
                var user = cd.Users.Where(a => a.UserId == id).FirstOrDefault();
                var myTracks = cd.UserTracks.Where(a => a.UserId == user.Id).Take(10).ToList();
                
                StringBuilder sb = new StringBuilder();
                sb.Append("run([");
                foreach (var item in users)
                {
                    sb.Append("['" + item.UserId + "',[");
                    foreach (var tr in (uts.Where(a => a.UserId == item.Id).ToList()))
                    {
                        sb.Append(tr.TrackId + ",");
                    }
                    sb.Length--;
                    sb.Append("]],");
                }
                sb.Length--;
                sb.Append("]," + (myTracks.Count * 0.7) + ",[");
                foreach (var item in myTracks)
                {
                    sb.Append(item.TrackId + ",");
                }
                sb.Length--;
                sb.Append("]).");
                string query = sb.ToString();
                ol.timer.Start();
                string sts = "run([['larry', [s1,s2,s3,s4,s5,s6,s7,s9]], ['mo',    [s1,s2,s3,s24,s25,s26,s27]], ['curly', [s1,s2,s3,s4,s5,s6,s27]] ]  , 4, [s1,s2,s3,s4,s5]). ";
                info = engine.solve(query);                
                for (int i = 0; i < 10; i++)
                {
                    if (ol.strings == null)
                        Thread.Sleep(1000);
                    else
                        break;
                }
                List<int> result = new List<int>();
                for (int i = 0; i < ol.strings.Length; i++)
                {
                    string k = ol.strings[i];
                    result.Add(Convert.ToInt32(ol.strings[i]));
                }
                var allTrs = cd.Tracks.ToList();
                var trs = allTrs.Where(p => result.Any(p2 => p2 == p.Id)).ToList();     // get the tracks with list of ids returned from prolog query
                return trs;
            }
            catch (Exception ex)

            {
                throw;
            }
        }
        
        public List<Track> getTracks()
        {            
            return cd.Tracks.ToList();
        }

        public List<Track> getTracks(string id)
        {
            try
            {
                var user = cd.Users.Where(b => b.UserId == id).Select(c => c.Id).FirstOrDefault();
                var ts = cd.UserTracks.Where(a => a.UserId == user).ToList();
                var g = (from a in cd.Tracks
                        join b in cd.UserTracks on a.Id equals b.TrackId
                        where  b.UserId == user
                        select a).ToList();
                return g;
            }
            catch (Exception ex)
            {            
                throw;
            }
            
        }

        public int getTrackCount(string userId)
        {
            var user = cd.Users.Where(a => a.UserId == userId).FirstOrDefault();
            var myTracks = cd.UserTracks.Where(a => a.UserId == user.Id).ToList();
            return myTracks.Count;
        }

        public bool CheckIfNewUser(string userId)
        {
            bool isNew = false;
            var users = cd.Users.Where(a => a.UserId == userId).ToList();
            if (users.Count < 1)
            {
                User us = new User { UserId = userId };
                cd.Users.Add(us);
                cd.SaveChanges();
                isNew = true;
            }
            return isNew;
        }

        public void CheckUserTracks(string userId, List<Track> trks)
        {
            try
            {
                var user = cd.Users.Where(a => a.UserId == userId).FirstOrDefault();
                var uts = cd.UserTracks.Where(a => a.UserId == user.Id).ToList();
                RemoveTracks(trks, uts);
                foreach (var item in trks)
                {
                    if (!cd.Artists.Any(a => a.Name == item.ArtistName))
                    {
                        Artist art = AddArtist(item.ArtistName);
                        item.Artist = art;
                    }
                    else
                    {
                        Artist art = cd.Artists.Where(a => a.Name == item.ArtistName).FirstOrDefault();
                        item.Artist = art;
                    }
                    var t = cd.Tracks.Where(a => a.ArtistName == item.ArtistName && a.Name == item.Name).ToList();
                    if (t.Count() == 0)
                    {
                        var id = AddTrack(item);
                        var ut = new UserTrack { UserId = user.Id, TrackId = id };
                        cd.UserTracks.Add(ut);
                        cd.SaveChanges();
                    }
                    else
                    {
                        var ut = new UserTrack { UserId = user.Id, TrackId = t.FirstOrDefault().Id };
                        cd.UserTracks.Add(ut);
                        cd.SaveChanges();
                    }
                }        
            }
            catch (Exception)
            {                
                throw;
            }
            
        }

        private Artist AddArtist(string artist)
        {
            try
            {
                Artist art = new Artist { Name = artist };
                var arts = cd.Artists;
                arts.Add(art);
                cd.SaveChanges();
                return art;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int AddTrack(Track tr)
        {
            try
            {
                var trks = cd.Tracks;
                trks.Add(tr);
                cd.SaveChanges();
                return tr.Id;
            }
            catch (Exception ex)
            {                
                throw;
            }            
        }

        public void RemoveTracks(List<Track> trks, List<UserTrack> currentInDb)
        {
            List<UserTrack> result = currentInDb.Where(p => !trks.Any(p2 => p2.Id == p.TrackId)).ToList();
            try
            {
                foreach (var item in result)
                {
                    cd.UserTracks.Remove(item);
                    cd.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddUser(string userId)
        {
            var users = cd.Users.Where(a => a.UserId == userId).ToList();
            if(users.Count == 0)
            {
                User us = new User { UserId = userId };
                cd.Users.Add(us);
                cd.SaveChanges();
            }
        }

        #endregion

        #region Radio

        public IEnumerable<RadioStream> GetAllStations()
        {
            var sts = cd.RadioStreams.Where(a => a.TestedOk == true && a.Status == 1);
            return sts;            
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

        #endregion

        private static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        private static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }

        protected void Dispose(bool disposing)
        {
            cd.Dispose();
            //base.Dispose(disposing);
        }
    }

    public class MySpyListener : SpyListener
    {
        public void onSpy(SpyEvent se)
        {
            System.Console.WriteLine(se.toString());
        }
    }

    public class MyOutputListener : OutputListener
    {
        public System.Timers.Timer timer = new System.Timers.Timer();
        public MyOutputListener()
        {            
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1000;
            timer.Enabled = true;
        }
        private int counter=0;
        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            counter ++;
        }
        
        public string[] strings;
        public void onOutput(OutputEvent e)
        {
            timer.Stop();
            var query_took = counter + "  seconds";
            string message = e.getMsg();
            var sts = message.Replace("[", string.Empty).Replace("]", string.Empty);
            strings = sts.Split(',');
        }
    }

    public class MyExcptioListener : ExceptionListener
    {
        public void onException(ExceptionEvent e)
        {
            System.Console.WriteLine(e.getMsg());
        }
    }

    [XmlRoot]
    [Serializable]
    public class BackUpDB
    {
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        //public List<Artist> Artists { get; set; }
        
    }
}



        //public void FillPrologDb()
        //{
        //    MyOutputListener ol = new MyOutputListener();
        //    try
        //    {
        //        SolveInfo info;
                
        //        java.io.InputStream stream = new FileInputStream(HttpContext.Current.Server.MapPath(@"~/App_Data/sameTaste.pl")); //"C:\\Users\\ed\\Documents\\Prolog\\sameTaste.pl"
        //        Theory t = new Theory(stream);
        //        alice.tuprolog.Prolog engine = new alice.tuprolog.Prolog();
        //        engine.setTheory(t);
        //        engine.addOutputListener(ol);
        //        engine.addSpyListener(new MySpyListener());
        //        engine.addExceptionListener(new MyExcptioListener());

        //        var users = cd.Users.ToList();
        //        var uts = cd.UserTracks.ToList();
 
        //        StringBuilder sb = new StringBuilder();
        //        string bb = "C:\\Users\\ed\\Downloads\\4thYearProject-master\\4thYearProject-master\\MyMusicAPI\\MyMusicAPI\\App_Data\\loadprolog.txt";
                
        //        sb.Append("['" + bb + "'],");
        //        sb.Append("fillDb([");
        //        foreach (var item in users)
        //        {
        //            sb.Append("['" + item.UserId + "',[");
        //            foreach (var tr in (uts.Where(a => a.UserId == item.Id).ToList()))
        //            {
        //                sb.Append("'" + tr.TrackId + "',");
        //            }
        //            sb.Length--;
        //            sb.Append("]],");
        //        }
        //        sb.Length--;  
        //        sb.Append("]),");
        //        string wq = "loadProlog.txt";
        //        string w = @HttpContext.Current.Server.MapPath(@"~/App_Data/loadprolog.txt");
        //        string query = sb.ToString();
        //        string more = "tell('" + bb + "'), listing(user/1),listing(song/1),listing(likes/2), told.";
        //        sb.Append("listing(user/1),listing(song/1),listing(likes/2), told.");
        //        sb.Append("tell('C:\\Users\\ed\\Downloads\\4thYearProject-master\\4thYearProject-master\\MyMusicAPI\\MyMusicAPI\\App_Data\\loadprolog.txt'), listing(user/1),listing(song/1),listing(likes/2), told.");

        //        string query = sb.ToString();
        //        string jjy = "fillDb([['user1',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1890','1891','1892','1893','1894']],['user2',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1890','1891','1892','1893','1894','1895','1896','1897','1898','1899','1900','1901','1902','1903','1904','1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921','1922','1923','1924','1925','1926','1927','1928','1929','1930','1931']],['user3',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1890','1891','1892','1893','1894','1895','1896','1897','1898','1899','1900','1901','1902','1903','1904','1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921','1922','1923','1924','1925','1926','1927','1928','1929','1930','1931','1932','1933','1934','1935','1936','1937','1938','1939','1940','1941','1942','1943','1944','1945','1946','1947','1948','1949','1950','1951','1952','1953','1954','1955','1956','1957','1958']],['user4',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1890','1891','1892','1893','1894','1895','1896','1897','1898','1899','1900','1901','1902','1903','1904','1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921','1922','1923','1924','1925','1926','1927','1928','1929','1930','1931','1932']],['user5',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1890','1891','1892','1893','1894','1895','1896','1897','1898','1899','1900','1901','1902','1903','1904','1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921','1922','1923','1924','1925']],['joe',['1878','1879','1880','1881','1882','1883','1884','1885','1886','1887','1888','1889','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921','1922','1923','1924','1925']]]),tell('";
        //        string rr = @"C:\\Users\\ed\\Downloads\\4thYearProject-master\\4thYearProject-master\\MyMusicAPI\\MyMusicAPI\\App_Data\\loadprolog.txt";
        //        string jjj = "'), listing(user/1),listing(song/1),listing(likes/2), told.";
        //        string er = jjy + rr + jjj ;
        //        info = engine.solve(er);
 
        //        for (int i = 0; i < 10; i++)
        //        {
        //            if (ol.strings == null)
        //                Thread.Sleep(1000);
        //            else
        //                break;
        //        }                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


//public string[] GetSameTaste(string id)
//        {
//            MyOutputListener ol = new MyOutputListener();

//            //User u = cd.Users.Where(a => a.UserId == "joe").FirstOrDefault();
//            //List<int> jj = new List<int> { 19, 10, 191, 912, 191, 194, 195, 116, 117, 118, 191, 190, 191, 192, 192, 1924, 1925 };
//            //foreach (var item in jj)
//            //{
//            //    var ut = new UserTrack { UserId = u.Id, TrackId = item };
//            //    cd.UserTracks.Add(ut);
//            //    cd.SaveChanges();
//            //}
//            try
//            {                
//                SolveInfo info;
//                java.io.InputStream stream = new FileInputStream("C:\\Users\\ed\\Documents\\Prolog\\sameTaste.pl");
//                Theory t = new Theory(stream);
//                alice.tuprolog.Prolog engine = new alice.tuprolog.Prolog();
//                engine.setTheory(t);

//                engine.addOutputListener(ol);
//                engine.addSpyListener(new MySpyListener());
//                engine.addExceptionListener(new MyExcptioListener());

//                var users = cd.Users.Where(a => a.UserId != id).ToList();
//                var uts = cd.UserTracks.ToList();
//                var user = cd.Users.Where(a => a.UserId == id).FirstOrDefault();
//                var myTracks = cd.UserTracks.Where(a => a.UserId == user.Id).Take(10).ToList();
//                var fa = cd.Tracks.ToList();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("run([");
//                foreach (var item in users)
//                {
//                    sb.Append("['" + item.UserId + "',[");
//                    foreach (var tr in (uts.Where(a => a.UserId == item.Id).ToList()))
//                    {
//                        sb.Append("'" + tr.Track.Id + "',");
//                    }
//                    sb.Length--;
//                    sb.Append("]],");
//                }
//                sb.Length--;
//                sb.Append("]," + (myTracks.Count * 0.7) +",[");
//                foreach (var item in myTracks)
//                {
//                    sb.Append("'" + item.Track.Id + "',");
//                }
//                sb.Length--;
//                sb.Append("]).");
//                string query = sb.ToString();
//                ol.timer.Start();
//                string tell = "['a_db.txt'].";
//                info = engine.solve(tell);
//                //info = engine.solve(query);
//                //info = engine.solve("assa([['larry',['s1','s2','s3']], ['curly',['s1','s2','s3']], ['mo',['s1','s2','s3']]], L).");
//                for (int i = 0; i < 10; i++)
//                {
//                    if (ol.strings == null)
//                        Thread.Sleep(1000);
//                    else
//                        break;
//                }                               
//                return ol.strings;
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }


//public string[] GetSameTaste(string id)
//        {
//            MyOutputListener ol = new MyOutputListener();
//            try
//            {
//                StringBuilder sb = new StringBuilder();
//                SolveInfo info;
//                java.io.InputStream stream = new FileInputStream("C:\\Users\\ed\\Documents\\Prolog\\sameTaste.pl");
//                Theory t = new Theory(stream);
//                alice.tuprolog.Prolog engine = new alice.tuprolog.Prolog();
//                engine.setTheory(t);

//                engine.addOutputListener(ol);
//                engine.addSpyListener(new MySpyListener());
//                engine.addExceptionListener(new MyExcptioListener());
//                string w = @HttpContext.Current.Server.MapPath(@"~/App_Data/loadprolog.txt");
//                string b = "C:\\Users\\ed\\Downloads\\4thYearProject-master\\4thYearProject-master\\MyMusicAPI\\MyMusicAPI\\App_Data\\loadProlog.txt";
//                string bc = @"C:\\Users\\ed\\Documents\\Prolog\\loadProlog.txt";
//                sb.Append("['" + bc +  "'],");
                
//                var users = cd.Users.Where(a => a.UserId != id).ToList();
//                var uts = cd.UserTracks.ToList();
//                var user = cd.Users.Where(a => a.UserId == id).FirstOrDefault();
//                var myTracks = cd.UserTracks.Where(a => a.UserId == user.Id).Take(10).ToList();
                
//                sb.Append("same_taste([");
//                foreach (var item in myTracks)
//                {
//                    sb.Append("'" + item.TrackId + "',");
//                }
//                sb.Length--;
//                sb.Append("], " + (myTracks.Count * 0.7) + ").");               

//                string query = sb.ToString();
//                ol.timer.Start();
//                info = engine.solve(query);
//                for (int i = 0; i < 10; i++)
//                {
//                    if (ol.strings == null)
//                        Thread.Sleep(1000);
//                    else
//                        break;
//                }                               
//                return ol.strings;
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }
