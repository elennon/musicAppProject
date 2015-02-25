using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
    public class Playlist
    {
        [PrimaryKey, AutoIncrement]
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //[ManyToMany(typeof(PlaylistTracks))]
        //public List<Track> Tracks { get; set; }
    }

    public class PlaylistTracks
    {
        [ForeignKey(typeof(Track))]
        public int TrackId { get; set; }

        [ForeignKey(typeof(Playlist))]
        public int PlaylistId { get; set; }
    }
}
