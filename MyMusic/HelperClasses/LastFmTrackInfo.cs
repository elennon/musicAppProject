﻿using MyMusic.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.HelperClasses
{

    public class Streamable
    {
        public string __invalid_name__text { get; set; }
        public string fulltrack { get; set; }
    }

    public class Artist
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
    }

    public class Image
    {
        public string __invalid_name__text { get; set; }
        public string size { get; set; }
    }

    public class Attr
    {
        public string position { get; set; }
    }

    public class Album
    {
        public string artist { get; set; }
        public string title { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public List<Image> image { get; set; }
        public Attr __invalid_name__attr { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Toptags
    {
        public List<Tag> tag { get; set; }
    }

    public class Track
    {
        public string id { get; set; }
        public string name { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public string duration { get; set; }
        public Streamable streamable { get; set; }
        public string listeners { get; set; }
        public string playcount { get; set; }
        public Artist artist { get; set; }
        public Album album { get; set; }
        public Toptags toptags { get; set; }
    }

    public class RootObject
    {
        public Track track { get; set; }
    }
}