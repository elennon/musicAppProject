using MyMusicAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MyMusicAPI.Helper_Classes
{
    public static class EcheoNest
    {
        private static HttpClient client = new HttpClient();

        public async static Task<Track> GetAudioSummary(string artist, string track)
        {
            Track tr = new Track();            
            try
            {            
                client = new HttpClient();
                client.BaseAddress = new Uri("http://developer.echonest.com/api/v4/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
                string url = string.Format("song/search?api_key=R7AMAUYTTBVEBTUQ7&format=json&results=1&artist={0}&title={1}&bucket=audio_summary", artist, track);
                string resp = await client.GetStringAsync(url);                
                var res = JsonConvert.DeserializeObject<ENRootObject>(resp);
                if (res.response.songs.Count > 0)
                {
                    var r = res.response.songs.FirstOrDefault().audio_summary;
                    tr = new Track
                    {
                        Name = track,
                        acousticness = r.acousticness,
                        analysis_url = r.analysis_url,
                        ArtistName = artist,
                        audio_md5 = r.audio_md5,
                        danceability = r.danceability,
                        duration = r.duration,
                        energy = r.energy,
                        instrumentalness = r.instrumentalness,
                        liveness = r.liveness,
                        loudness = r.loudness,
                        mode = r.mode,
                        speechiness = r.speechiness,
                        tempo = r.tempo,
                        time_signature = r.time_signature,
                        valence = r.valence
                    };   
                    
                    return tr;
                }
                else
                    
                    return null;
            }
            catch (Exception)
            {
                return null;
            }            
        }
    }

    public class ENStatus
    {
        public string version { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class ENAudioSummary
    {
        public int key { get; set; }
        public string analysis_url { get; set; }
        public double energy { get; set; }
        public double liveness { get; set; }
        public double tempo { get; set; }
        public double speechiness { get; set; }
        public double acousticness { get; set; }
        public double instrumentalness { get; set; }
        public int mode { get; set; }
        public int time_signature { get; set; }
        public double duration { get; set; }
        public double loudness { get; set; }
        public string audio_md5 { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
    }

    public class ENSong
    {
        public string artist_id { get; set; }
        public string artist_name { get; set; }
        //public string id { get; set; }
        public ENAudioSummary audio_summary { get; set; }
        //public string title { get; set; }
    }

    public class ENResponse
    {
        public ENStatus status { get; set; }        
        public List<ENSong> songs { get; set; }
    }

    public class ENRootObject
    {
        public ENResponse response { get; set; }
    }



    //public class Status
    //{
    //    public string version { get; set; }
    //    public int code { get; set; }
    //    public string message { get; set; }
    //}

    //public class AudioSummary
    //{
    //    public int key { get; set; }
    //    public string analysis_url { get; set; }
    //    public double energy { get; set; }
    //    public double liveness { get; set; }
    //    public double tempo { get; set; }
    //    public double speechiness { get; set; }
    //    public double acousticness { get; set; }
    //    public double instrumentalness { get; set; }
    //    public int mode { get; set; }
    //    public int time_signature { get; set; }
    //    public double duration { get; set; }
    //    public double loudness { get; set; }
    //    public string audio_md5 { get; set; }
    //    public double valence { get; set; }
    //    public double danceability { get; set; }
    //}

    //public class Song
    //{
    //    public string artist_id { get; set; }
    //    public string artist_name { get; set; }
    //    public string id { get; set; }
    //    public AudioSummary audio_summary { get; set; }
    //    public string title { get; set; }
    //}

    //public class Response
    //{
    //    public Status status { get; set; }
    //    public List<Song> songs { get; set; }
    //}

    //public class RootObject
    //{
    //    public Response response { get; set; }
    //}
}