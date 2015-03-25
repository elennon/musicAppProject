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
            if (track.Contains('(') || track.Contains(')'))
            {
                track = track.Substring(0, track.IndexOf(" ("));
            }

            if (track.Contains('[') || track.Contains(']'))
            {
                track = track.Substring(0, track.IndexOf("["));
            }
            var pic = await Deezer.getPic(artist, track);
            Track tr = GrooveShark.getTrack(artist, track, sessionId);
            tr.Image = pic;
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
            var f = value.Split(',');
            if (f[0] == "Mark30")
            {
                GrooveShark.Mark30Seconds(f[1], f[2], f[3]);
            }
            else if (f[0] == "MarkFinished")
            {
                GrooveShark.MarkFinished(f[1], f[2], f[3], f[4]);
            }
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
