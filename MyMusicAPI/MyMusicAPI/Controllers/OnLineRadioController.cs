
using MyMusicAPI.DAL;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MyMusicAPI.Controllers
{
    public class OnLineRadioController : ApiController
    {    
       
        private IMusicCentralRepo cd;
        //private MusicCentralContext cd = new MusicCentralContext();
        public OnLineRadioController(IMusicCentralRepo repo)
        {
            cd = repo;
        }
        // GET api/onlineradio
        //public async Task<List<RadioStream>> Get()
        public async Task<List<RadioGenre>> Get()
        {
            
            //StreamFinderRadioStations sf = new StreamFinderRadioStations();
            //var radios = sf.StreamFinderRadioStationColl;
            
            //IEnumerable<StreamFinderRadioStream> rs = await RadioStations();
            //gogo(rs);
            var t = await UpdateStreamFinderStations();
            var h = cd.GetAllStationGenres();
            return h;
        }

        private async Task<List<RadioStream>> UpdateStreamFinderStations()
        {
            StreamFinderRadioStations sf = new StreamFinderRadioStations(cd);
            var streamFinderDone = await sf.Getstations();
            return cd.GetAllStations();
        }

       













        // GET api/onlineradio/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/onlineradio
        public void Post([FromBody]string value)
        {
        }

        // PUT api/onlineradio/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/onlineradio/5
        public void Delete(int id)
        {
        }

        

    }
}
