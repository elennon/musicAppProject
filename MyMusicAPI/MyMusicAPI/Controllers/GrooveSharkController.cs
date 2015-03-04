using MyMusicAPI.Helper_Classes;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class GrooveSharkController : ApiController
    {
        public GrooveShark gShark { get; set; }

        // GET api/grooveshark
        public string GetAuthenticate(string userName, string password)
        {
            return gShark.GetSessionId(userName, password);
        }
       
        // GET api/grooveshark/5
        public string GetTrack(string artist, string track, string sessionId)
        {
            gShark = new GrooveShark();
            return gShark.getTrack(artist, track, sessionId);
        }

        public List<Artist> GetSimilarArtists(string arts, string sessionId)
        {
            gShark = new GrooveShark();
            //List<Artist> lsts = new List<Artist>();
            List<Artist> lst = gShark.getSimilarArtists(arts, sessionId);
            
            return lst;
        }

        // POST api/grooveshark
        public void Post([FromBody]string value)
        {
        }

        // PUT api/grooveshark/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/grooveshark/5
        public void Delete(int id)
        {
        }
    }
}
