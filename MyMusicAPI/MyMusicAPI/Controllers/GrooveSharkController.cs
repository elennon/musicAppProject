using MyMusicAPI.Helper_Classes;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class GrooveSharkController : ApiController
    {
        // base.Configuration.ProxyCreationEnabled = false;     [IgnoreDataMember]
        
        // GET api/grooveshark
        public string GetAuthenticate(string userName, string password)
        {
            return GrooveShark.GetSessionId(userName, password);
        }
       
        // GET api/grooveshark/5
        public async Task<Track> GetTrack(string artist, string track, string sessionId)
        {
            string output = "", output2 = "";
            
            if (track.Contains('(') || track.Contains(')'))
            {
                track = track.Substring(0, track.IndexOf(" ("));
            }

            if (output.Contains('[') || output.Contains(']'))
            {
                track = track.Substring(0, track.IndexOf("["));
            }
            var pic = await Deezer.getPic(artist, track);           
            string key = GrooveShark.getTrack(artist, output2, sessionId);
            Track tr = new Track { Image = pic, GSSongKey = key, ArtistName = artist, Name = track };
            return tr;
        }
        
        public List<Artist> GetSimilarArtists(string arts, string sessionId, int limit)
        {
           // gShark = new GrooveShark();
            //List<Artist> lsts = new List<Artist>();
            List<Artist> lst = GrooveShark.getSimilarArtists(arts, sessionId, limit);
            
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
