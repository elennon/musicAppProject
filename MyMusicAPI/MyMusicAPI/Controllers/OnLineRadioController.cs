
using MyMusicAPI.DAL;
using MyMusicAPI.Helper_Classes;
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
        // base.Configuration.ProxyCreationEnabled = false;     [IgnoreDataMember]
        private IMusicCentralRepo cd;
        //private MusicCentralContext cd = new MusicCentralContext();
        public OnLineRadioController(IMusicCentralRepo repo)
        {
            cd = repo;
        }
       
        private async Task FilterTest()
        {
            try
            {
                FilterStations ft = new FilterStations(cd);
                var t = cd.GetAllStations().Where(a => a.Id > 11510).ToList();
               
                foreach (var item in t)
                {
                    bool ifOk = await ft.RunFilter(item.RadioUrl);
                    cd.UpdateStation(item, ifOk);                   
                }
                var tt = cd.GetAllStations().ToList().Take(15);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task Get()
        //{
        //    await FilterTest();
        //}

        public IEnumerable<RadioStream> Get()
        {
            List<RadioStream> gss = new List<RadioStream>();
            try
            {
                //cd.DropDb();
                //DirbleRadioSt ds = new DirbleRadioSt(cd);
                //ds.AddDirbleStationsToDB();
                //ds.AddIrish();
                //ds.DeletThis("Talk & Speech");                
                //FillDownloadsRadio();


                var og = GetSmallerCollection(30);
                foreach (var item in og)
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

        private List<RadioStream> GetSmallerCollection(int howMany)
        {
            List<RadioStream> stations = new List<RadioStream>();
            var gGroups = cd.GetAllStations().GroupBy(p => p.RadioGenreId, (key, g) => new { genreId = key, stations = (g.ToList()).Take(howMany) });   // group stations by genres
            
            foreach (var item in gGroups)
            {
                stations.AddRange(item.stations);
            }            
            return stations;
        }


        private async Task<IEnumerable<RadioStream>> GetStreamFinderRadio()
        {
            StreamFinderRadioStations sf = new StreamFinderRadioStations(cd);     
            var streamFinderDone = await sf.Getstations();
            return cd.GetAllStations();
        }

        private void FillDownloadsRadio()
        {
            DownloadedStations ds = new DownloadedStations(cd);
            ds.AddDownloadsToDB();           
        }

        private void FillDirbleRadio()
        {
            DirbleRadioSt ds = new DirbleRadioSt(cd);
            ds.AddDirbleStationsToDB();
        }

        // GET api/onlineradio/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/onlineradio
        public void Post([FromUri]string value)
        {
            switch (value)
            {
                case "streamfinder":
                    Console.WriteLine(1);
                    break;
                case "download":
                    FillDownloadsRadio();
                    break;
                case "dirble":
                    FillDirbleRadio();
                    break;
            }
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
