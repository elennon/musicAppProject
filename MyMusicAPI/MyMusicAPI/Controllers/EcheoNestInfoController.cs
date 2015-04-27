using MyMusicAPI.Helper_Classes;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class EcheoNestInfoController : ApiController
    {
        // GET api/echeonestinfo
        public async Task<Track> Get(string artist, string track)
        {            
            return await EcheoNest.GetAudioSummary(artist, track);
        }

        // GET api/echeonestinfo/5
        public async Task<string> Get(string info)
        {
            string lfm = await LastFm.getPic(info.Split(',')[0], info.Split(',')[1]);
            //if (!string.IsNullOrEmpty(lfm))
            //{
            //    tr.Image = lfm.Split(',')[0];
            //    tr.Genre = lfm.Split(',')[1];
            //}
            return lfm;
        }

        public string Post([FromBody]string value)
        {
            string artist = value.Split(',')[0];
            string track = value.Split(',')[1];
            return value;//await EcheoNest.GetAudioSummary(artist, track);
        }
        // PUT api/echeonestinfo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/echeonestinfo/5
        public void Delete(int id)
        {
        }
    }
}
