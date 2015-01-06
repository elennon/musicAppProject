using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyMusicAPI.Controllers
{
    public class ConnectionController : ApiController
    {
        // GET api/connection
        public string Get()
        {
            string connOK = "Your connected alright";
            return connOK;
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
