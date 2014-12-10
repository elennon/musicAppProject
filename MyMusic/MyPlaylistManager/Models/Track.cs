
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyPlaylistManager.Models
{
    public sealed class Track
    {
        
        public int TrackId { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }

        
        public int ArtistId { get; set; }
        
        public int AlbumId { get; set; }
        public int Plays { get; set; }
        public int Skips { get; set; }
        public int RandomPlays { get; set; }
        public int OrderNo { get; set; }
        public string ImageUri { get; set; }
        public string FileName { get; set; }

        //[ForeignKey(typeof(Track))]     // Specify the foreign key
        //public int ArtistId { get; set; }

        //[ManyToOne]
        //public Artist Artists { get; set; }
        //public Album Album { get; set; }
        //public Genre Genre { get; set; }
    }   
}
