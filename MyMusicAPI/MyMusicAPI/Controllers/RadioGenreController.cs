using MyMusicAPI.DAL;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class RadioGenreController : ApiController
    {
        private IMusicCentralRepo cd;
        public RadioGenreController(IMusicCentralRepo repo)
        {
            cd = repo;
        }

        // GET api/connection
        public IEnumerable<RadioGenre> Get()
        {
            List<RadioGenre> gss = new List<RadioGenre>();
            try
            {
                var g = cd.GetAllStationGenres().ToList();//.AsEnumerable();
               
                foreach (var item in g)
                {
                    gss.Add(item);
                }
                return gss.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // GET api/connection/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/connection
        public void Post([FromBody]string value)
        {
        }

        // PUT api/connection/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/connection/5
        public void Delete(int id)
        {
        }
    }
}
