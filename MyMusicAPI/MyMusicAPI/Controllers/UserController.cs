using MyMusicAPI.DAL;
using MyMusicAPI.DTO;
using MyMusicAPI.Helper_Classes;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;

namespace MyMusicAPI.Controllers
{
    public class UserController : ApiController
    {
        private IMusicCentralRepo repo;
        public UserController(IMusicCentralRepo rpo)
        {
            repo = rpo;
        }
        
        //public IEnumerable<TrackDTO> Get()
        //{
        //    //repo.DropDb();
        //    var trs = repo.getTracks();
        //    var dtoTrs = DtoConverter.TrackToDto(trs);
        //    return dtoTrs;          
        //}

        public void Get()
        {
            repo.DropDb();
            repo.SeedDb();
            // return DtoConverter.TrackToDto(repo.getTracks());
        }

        //public IEnumerable<TrackDTO> Get()
        //{
        //    repo.DropDb();
        //    repo.SeedDb();  
        //   // return DtoConverter.TrackToDto(repo.getTracks());
        //}

        // GET api/user
        public IEnumerable<TrackDTO> Get(string id)   // calls prolog to get tracks from similar users 
        {
            //repo.FillPrologDb();
            var ts = repo.GetSameTaste(id);
            //var ts = repo.getTracks(id);

            return DtoConverter.TrackToDto(ts);
        }

        // GET api/user/5
        public bool Get(int trackCount, string userId)
        {
            bool ok = true;
            var trkCount = repo.getTrackCount(userId);
            if (trkCount == trackCount)
                return ok;
            else
                return false;
        }

        public void Post(UserTrackList userTracks)    //string userId,
        { 
            var nts = DtoConverter.DtoTotracks(userTracks.Songs);
            try
            {
                  //repo.DropDb();
                  //repo.SeedDb();
                  repo.CheckIfNewUser(userTracks.UserId);
                  repo.CheckUserTracks(userTracks.UserId, nts);
            }
            catch (Exception ex)
            {
                string h = ex.Message;
                using (Stream str = new FileStream(HttpContext.Current.Server.MapPath(@"~/App_Data/message.txt"),
                    FileMode.Create, FileAccess.Write, FileShare.None))
                {                   
                    byte[] byteData = null;
                    byteData = Encoding.ASCII.GetBytes(h);
                    str.Write(byteData, 0, byteData.Length);
                    str.Close();
                }
                throw;
            }          
        }

        // PUT api/user/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/user/5
        public void Delete(int id)
        {
        }
    }

    public class UserTrackList
    {
        public string UserId { get; set; }
        public List<TrackDTO> Songs { get; set; }
    }
}
