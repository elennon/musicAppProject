using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMusicAPI.Models
{
    public class RadioGenre
    {
        public int RadioGenreId { get; set; }
        public string RadioGenreName { get; set; }
        public string RadioImage { get; set; }
       

        public virtual List<RadioStream> RadioColl { get; set; }
    }

    public class RadioStream
    {
        
        public int RadioStreamId { get; set; }
        public string RadioName { get; set; }
        public List<string> RadioUrl { get; set; }
        public string Image { get; set; }
    }
}