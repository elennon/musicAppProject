using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyMusic.Models
{
   
    public class RadioStream
    {
        [PrimaryKey, AutoIncrement]
        public int RadioId { get; set; }
        public string RadioName { get; set; }        
        public string RadioGenreId { get; set; }
        public string RadioUrl { get; set; } 
    }

    [XmlRoot(ElementName = "Stations")]
    public class Stations : radioStation
    {
        public List<radioStation> stations { get; set; }
    }

    public class radioStation
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Genre { get; set; }
        [XmlElement]
        public List<Urls> Urls { get; set; }
    }

    public class Urls
    {
        [XmlElement]
        public string urlName { get; set; }
        [XmlAttribute]
        public bool isOk { get; set; }
    }




    public class RadioStreamGenre
    {
        [PrimaryKey, AutoIncrement]
        public int RadioGenreId { get; set; }
        public string RadioGenreKey { get; set; }
        public string RadioGenreName { get; set; }
    }
}
