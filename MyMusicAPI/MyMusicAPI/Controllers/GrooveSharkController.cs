using MyMusicAPI.Helper_Classes;
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
        // GET api/grooveshark
        public string GetAuthenticate(string userName, string password)
        {
            GrooveShark sesId = new GrooveShark();
            return sesId.GetSessionId(userName, password);
        }
       
        // GET api/grooveshark/5
        public string GetTrack(string artist, string track, string sessionId)
        {
            GrooveShark sesId = new GrooveShark();
            return sesId.getTrack(artist, track, sessionId);
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
