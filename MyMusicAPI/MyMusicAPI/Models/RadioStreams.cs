using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MyMusicAPI.Models
{
    public class RadioStream
    {
        public int RadioId { get; set; }
        public string RadioName { get; set; }
        public int RadioGenreId { get; set; }
        public string RadioUrl { get; set; }
        public string Image { get; set; }
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




    public class RadioGenre
    {        
        public int RadioGenreId { get; set; }
        public string RadioGenreKey { get; set; }
        public string RadioGenreName { get; set; }
        public string RadioImage { get; set; }
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




    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Id
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
    }

    public class High
    {
        public string url { get; set; }
    }

    public class Thumbnails
    {
        public Default @default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
    }

    public class Snippet
    {
        public string publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string liveBroadcastContent { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Id id { get; set; }
        public Snippet snippet { get; set; }
    }

    public class RootObject
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }
}

