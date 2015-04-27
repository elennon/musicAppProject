using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMusicAPI.DTO
{
    public class TrackDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Genre { get; set; }
        public DateTime DateAdded { get; set; }
        public double Rating { get; set; }

        public string GSSongKey { get; set; }
        public string GSSongKeyUrl { get; set; }
        public string GSServerId { get; set; }

        public string ArtistName { get; set; }

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
}