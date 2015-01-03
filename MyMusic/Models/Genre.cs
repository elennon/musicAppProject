using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
   
    public class Genre 
    {
        [PrimaryKey, AutoIncrement]
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string TrackCount { get; set; }
    }
}
