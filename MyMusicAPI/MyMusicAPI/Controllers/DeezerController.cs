using MyMusicAPI.Helper_Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class DeezerController : ApiController
    {
       
        // GET api/deezer
        public async Task<KeyValuePair<string, List<string>>> GetTracksByArtist(string artist, int topNo)
        {
            //IEnumerable<string> result = new List<string>(); 
            KeyValuePair<string, List<string>> top = new KeyValuePair<string, List<string>>();
            try
            {
                top = await Deezer.GetArtistTop(artist, topNo);                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return top;
        }

        // GET api/deezer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/deezer
        public void Post([FromBody]string value)
        {
        }

        // PUT api/deezer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/deezer/5
        public void Delete(int id)
        {
        }
    }
   
}
