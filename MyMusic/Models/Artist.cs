using SQLite;
//using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
    public class Artist
    {
        [PrimaryKey, AutoIncrement]
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        
    }

    
}
