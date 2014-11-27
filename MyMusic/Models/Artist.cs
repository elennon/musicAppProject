using SQLite;
using SQLiteNetExtensions.Attributes;
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

        //List<Track> tracks
        //{
        //    get
        //    {
        //        return db.Query<Chapters>.Where(b => b.BookId == this.Id);
        //    }
        //}
        //[OneToMany]                    
        //public List<Track> tracks { get; set; }
    }
}
