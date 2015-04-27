using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyMusic.Utilities
{
    public class UserTrackList
    {
        public string UserId { get; set; }
        public List<TrackDTO> Songs { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "")]//, IsNullable = false
    public partial class lfm
    {
        private lfmTrack trackField;

        private string statusField;

        /// <remarks/>
        public lfmTrack track
        {
            get
            {
                return this.trackField;
            }
            set
            {
                this.trackField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class lfmTrack
    {

        private lfmTrackArtist artistField;

        private lfmTrackAlbum albumField;

        private lfmTrackTag[] toptagsField;

        public lfmTrackArtist artist
        {
            get
            {
                return this.artistField;
            }
            set
            {
                this.artistField = value;
            }
        }

        /// <remarks/>
        public lfmTrackAlbum album
        {
            get
            {
                return this.albumField;
            }
            set
            {
                this.albumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("tag" )]   //IsNullable = false
        public lfmTrackTag[] toptags
        {
            get
            {
                return this.toptagsField;
            }
            set
            {
                this.toptagsField = value;
            }
        }

    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class lfmTrackArtist
    {

        private string nameField;

        private object mbidField;

        private string urlField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public object mbid
        {
            get
            {
                return this.mbidField;
            }
            set
            {
                this.mbidField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class lfmTrackAlbum
    {

        private string artistField;

        private string titleField;

        private object mbidField;

        private string urlField;

        private lfmTrackAlbumImage[] imageField;

        private string positionField;

        /// <remarks/>
        public string artist
        {
            get
            {
                return this.artistField;
            }
            set
            {
                this.artistField = value;
            }
        }

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public object mbid
        {
            get
            {
                return this.mbidField;
            }
            set
            {
                this.mbidField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("image")]
        public lfmTrackAlbumImage[] image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class lfmTrackAlbumImage
    {

        private string sizeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class lfmTrackTag
    {

        private string nameField;

        private string urlField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

}

namespace Xml2CSharp
{
    //	[XmlRoot(ElementName="streamable")]
    public class Streamable
    {
        [XmlAttribute(AttributeName = "fulltrack")]
        public string Fulltrack { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    //	[XmlRoot(ElementName="artist")]
    public class Artist
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "mbid")]
        public string Mbid { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    //	[XmlRoot(ElementName="image")]
    public class Image
    {
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    //	[XmlRoot(ElementName="album")]
    public class Album
    {
        [XmlElement(ElementName = "artist")]
        public string Artist { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "mbid")]
        public string Mbid { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "image")]
        public List<Image> Image { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
    }

    //	[XmlRoot(ElementName="tag")]
    public class Tag
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    //	[XmlRoot(ElementName="toptags")]
    public class Toptags
    {
        [XmlElement(ElementName = "tag")]
        public List<Tag> Tag { get; set; }
    }

    //	[XmlRoot(ElementName="track")]
    public class Track
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "mbid")]
        public string Mbid { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "duration")]
        public string Duration { get; set; }
        [XmlElement(ElementName = "streamable")]
        public Streamable Streamable { get; set; }
        [XmlElement(ElementName = "listeners")]
        public string Listeners { get; set; }
        [XmlElement(ElementName = "playcount")]
        public string Playcount { get; set; }
        [XmlElement(ElementName = "artist")]
        public Artist Artist { get; set; }
        [XmlElement(ElementName = "album")]
        public Album Album { get; set; }
        [XmlElement(ElementName = "toptags")]
        public Toptags Toptags { get; set; }
    }

    
    [XmlRoot(ElementName = "lfm")]
    public class lfm
    {
        [XmlElement(ElementName = "track")]
        public Track Track { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

}


//namespace MyMusic.Utilities
//{


//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//    public partial class lfm
//    {
//        private lfmTrack trackField;

//        private string statusField;

//        /// <remarks/>
//        public lfmTrack track
//        {
//            get
//            {
//                return this.trackField;
//            }
//            set
//            {
//                this.trackField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string status
//        {
//            get
//            {
//                return this.statusField;
//            }
//            set
//            {
//                this.statusField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrack
//    {

//        private uint idField;

//        private string nameField;

//        private string mbidField;

//        private string urlField;

//        private uint durationField;

//        private lfmTrackStreamable streamableField;

//        private uint listenersField;

//        private uint playcountField;

//        private lfmTrackArtist artistField;

//        private lfmTrackAlbum albumField;

        
//        private lfmTrackTag[] toptagsField;

//        private lfmTrackWiki wikiField;

//        /// <remarks/>
//        public uint id
//        {
//            get
//            {
//                return this.idField;
//            }
//            set
//            {
//                this.idField = value;
//            }
//        }

//        /// <remarks/>
//        public string name
//        {
//            get
//            {
//                return this.nameField;
//            }
//            set
//            {
//                this.nameField = value;
//            }
//        }

//        /// <remarks/>
//        public string mbid
//        {
//            get
//            {
//                return this.mbidField;
//            }
//            set
//            {
//                this.mbidField = value;
//            }
//        }

//        /// <remarks/>
//        public string url
//        {
//            get
//            {
//                return this.urlField;
//            }
//            set
//            {
//                this.urlField = value;
//            }
//        }

//        /// <remarks/>
//        public uint duration
//        {
//            get
//            {
//                return this.durationField;
//            }
//            set
//            {
//                this.durationField = value;
//            }
//        }

//        /// <remarks/>
//        public lfmTrackStreamable streamable
//        {
//            get
//            {
//                return this.streamableField;
//            }
//            set
//            {
//                this.streamableField = value;
//            }
//        }

//        /// <remarks/>
//        public uint listeners
//        {
//            get
//            {
//                return this.listenersField;
//            }
//            set
//            {
//                this.listenersField = value;
//            }
//        }

//        /// <remarks/>
//        public uint playcount
//        {
//            get
//            {
//                return this.playcountField;
//            }
//            set
//            {
//                this.playcountField = value;
//            }
//        }

//        /// <remarks/>
//        public lfmTrackArtist artist
//        {
//            get
//            {
//                return this.artistField;
//            }
//            set
//            {
//                this.artistField = value;
//            }
//        }

//        /// <remarks/>
//        public lfmTrackAlbum album
//        {
//            get
//            {
//                return this.albumField;
//            }
//            set
//            {
//                this.albumField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlArrayItemAttribute("tag", IsNullable = false)]
//        public lfmTrackTag[] toptags
//        {
//            get
//            {
//                return this.toptagsField;
//            }
//            set
//            {
//                this.toptagsField = value;
//            }
//        }

//        /// <remarks/>
//        public lfmTrackWiki wiki
//        {
//            get
//            {
//                return this.wikiField;
//            }
//            set
//            {
//                this.wikiField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackStreamable
//    {

//        private byte fulltrackField;

//        private byte valueField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public byte fulltrack
//        {
//            get
//            {
//                return this.fulltrackField;
//            }
//            set
//            {
//                this.fulltrackField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlTextAttribute()]
//        public byte Value
//        {
//            get
//            {
//                return this.valueField;
//            }
//            set
//            {
//                this.valueField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackArtist
//    {

//        private string nameField;

//        private object mbidField;

//        private string urlField;

//        /// <remarks/>
//        public string name
//        {
//            get
//            {
//                return this.nameField;
//            }
//            set
//            {
//                this.nameField = value;
//            }
//        }

//        /// <remarks/>
//        public object mbid
//        {
//            get
//            {
//                return this.mbidField;
//            }
//            set
//            {
//                this.mbidField = value;
//            }
//        }

//        /// <remarks/>
//        public string url
//        {
//            get
//            {
//                return this.urlField;
//            }
//            set
//            {
//                this.urlField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackAlbum
//    {

//        private string artistField;

//        private string titleField;

//        private object mbidField;

//        private string urlField;

//        private lfmTrackAlbumImage[] imageField;

//        private string positionField;

//        /// <remarks/>
//        public string artist
//        {
//            get
//            {
//                return this.artistField;
//            }
//            set
//            {
//                this.artistField = value;
//            }
//        }

//        /// <remarks/>
//        public string title
//        {
//            get
//            {
//                return this.titleField;
//            }
//            set
//            {
//                this.titleField = value;
//            }
//        }

//        /// <remarks/>
//        public object mbid
//        {
//            get
//            {
//                return this.mbidField;
//            }
//            set
//            {
//                this.mbidField = value;
//            }
//        }

//        /// <remarks/>
//        public string url
//        {
//            get
//            {
//                return this.urlField;
//            }
//            set
//            {
//                this.urlField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("image")]
//        public lfmTrackAlbumImage[] image
//        {
//            get
//            {
//                return this.imageField;
//            }
//            set
//            {
//                this.imageField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string position
//        {
//            get
//            {
//                return this.positionField;
//            }
//            set
//            {
//                this.positionField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackAlbumImage
//    {

//        private string sizeField;

//        private string valueField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string size
//        {
//            get
//            {
//                return this.sizeField;
//            }
//            set
//            {
//                this.sizeField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlTextAttribute()]
//        public string Value
//        {
//            get
//            {
//                return this.valueField;
//            }
//            set
//            {
//                this.valueField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackTag
//    {

//        private string nameField;

//        private string urlField;

//        /// <remarks/>
//        public string name
//        {
//            get
//            {
//                return this.nameField;
//            }
//            set
//            {
//                this.nameField = value;
//            }
//        }

//        /// <remarks/>
//        public string url
//        {
//            get
//            {
//                return this.urlField;
//            }
//            set
//            {
//                this.urlField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class lfmTrackWiki
//    {

//        private string publishedField;

//        private string summaryField;

//        private string contentField;

//        /// <remarks/>
//        public string published
//        {
//            get
//            {
//                return this.publishedField;
//            }
//            set
//            {
//                this.publishedField = value;
//            }
//        }

//        /// <remarks/>
//        public string summary
//        {
//            get
//            {
//                return this.summaryField;
//            }
//            set
//            {
//                this.summaryField = value;
//            }
//        }

//        /// <remarks/>
//        public string content
//        {
//            get
//            {
//                return this.contentField;
//            }
//            set
//            {
//                this.contentField = value;
//            }
//        }
//    }


//}