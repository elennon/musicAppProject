using SQLite;
using SQLiteNetExtensions.Attributes;
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

        [ForeignKey(typeof(Artist))]
        public int ArtistId { get; set; }
        [ForeignKey(typeof(Album))]
        public int AlbumId { get; set; }
        public string Album { get; set; }

        [ForeignKey(typeof(Genre))]
        public int GenreId { get; set; }

        public int OrderNo { get; set; }
        public int Plays { get; set; }
        public int Skips { get; set; }
        public int RandomPlays { get; set; }
        public int PerCentRate { get; set; }
       
        private string _imageUri = "ms-appx:///Assets/radio672.png";
        public string ImageUri
        {
            get { return _imageUri; }
            set { _imageUri = value; }
        }
        

        public string FileName { get; set; }
        public DateTime DateAdded { get; set; }

        private bool inTheBin = false;
        public bool InTheBin
        {
            get { return inTheBin; }
            set { inTheBin = value; }
        }

        public bool InQuickPick { get; set; }

        public bool InEditMode { get; set; }

        //[ManyToMany(typeof(PlaylistTracks))]
        //public List<Playlist> Playlists { get; set; }
       
    }   
}
