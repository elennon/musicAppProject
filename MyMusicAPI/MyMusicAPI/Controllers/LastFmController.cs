using MyMusicAPI.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class LastFmController : ApiController
    {
        public async Task<List<LfmArtist>> GetSimilarArtists(string artist, int top)
        {
            var arts = await LastFm.GetSimilarArtists(artist, top);           
            return arts;
        }
        // GET api/lastfm
        public async Task<List<TpTrack>> GetSimilarArtistTracks(string artist, int num)
        {
            //var arts = await LastFm.GetSimilarArtists(artist, num);
            var trks = await LastFm.GetArtistsTop(artist, num);

            return trks;
        }

        // GET api/lastfm/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/lastfm
        public void Post([FromBody]string value)
        {
        }

        // PUT api/lastfm/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/lastfm/5
        public void Delete(int id)
        {
        }
    }
}
