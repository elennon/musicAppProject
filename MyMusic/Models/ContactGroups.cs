using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
    public class ContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public ObservableCollection<Track> Tracks { get; set; }
    }
    public class ArtistContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public List<Artist> Artists { get; set; }
    }
    public class AlbumContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public List<Album> Albums { get; set; }
    }
}
