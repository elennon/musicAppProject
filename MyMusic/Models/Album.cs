using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{   
    
    public class Album 
    {
        [PrimaryKey, AutoIncrement]
        public int AlbumId { get; set; }
        public string Name { get; set; }
    }
}

