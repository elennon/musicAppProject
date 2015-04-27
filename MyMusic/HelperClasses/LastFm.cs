using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyMusic.HelperClasses
{
   
        /// <remarks/>
        //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        //[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        [XmlRoot]
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

            private string nameField;

            private string mbidField;

            private string urlField;

            private lfmTrackArtist artistField;

            private lfmTrackAlbum albumField;

            private lfmTrackTag[] toptagsField;

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
            public string mbid
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
            [System.Xml.Serialization.XmlArrayItemAttribute("tag", IsNullable = false)]
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

            private string mbidField;

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
            public string mbid
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

            private string mbidField;

            private string urlField;

            private lfmTrackAlbumImage[] imageField;

            private byte positionField;

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
            public string mbid
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
            public byte position
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
