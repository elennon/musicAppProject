using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
    public class RadioStream
    {
        [PrimaryKey, AutoIncrement]
        public int RadioId { get; set; }
        public string RadioName { get; set; }        
        public string RadioGenreId { get; set; }
        public string RadioUrl { get; set; } 
    }
    public class RadioStreamGenre
    {
        [PrimaryKey, AutoIncrement]
        public int RadioGenreId { get; set; }
        public string RadioGenreKey { get; set; }
        public string RadioGenreName { get; set; }
    }
}
