using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyMusic.Models
{
    public class Track
    {
        [PrimaryKey, AutoIncrement]
        public int TrackId { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }

        //public Artist Composer { get; set; }
        //public Album Album { get; set; }
        //public Genre Genre { get; set; }
    }   
}
