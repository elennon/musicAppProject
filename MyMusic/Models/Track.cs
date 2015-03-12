using MyMusic.DAL;
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
        private IRepository repo = new Repository();

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
        //public int PerCentRate { get; set; }
        public int PerCentRate
        {
            get
            {
                return repo.DoPercent(this);
            }
            protected set { }
        }
        public string ImageUrl { get; set; }

        //private string _imageUrl = "ms-appx:///Assets/radio672.png";
        //public string ImageUrl
        //{
        //    get { return _imageUrl; }
        //    set { _imageUrl = value; }
        //}
        

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

        public string GSId { get; set; }

        public override string ToString()
        {
            return string.Format(" Play count:  {0} ({1}) %", Plays + RandomPlays, PerCentRate);
        }

        //[ManyToMany(typeof(PlaylistTracks))]
        //public List<Playlist> Playlists { get; set; }
        
    }

    public class TrackDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public object ArtistId { get; set; }
        public string GSSongKey { get; set; }
        public string ArtistName { get; set; }
        public object Artist { get; set; }
    }
}
