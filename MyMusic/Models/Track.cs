using MyMusic.DAL;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace MyMusic.Models
{
    
    public class Track
    {
        private IRepository repo = new Repository();

        [PrimaryKey, AutoIncrement]
        public int TrackId { get; set; }
        public string Name { get; set; }
        public string ArtistName { get; set; }

        [ForeignKey(typeof(Artist))]
        public int ArtistId { get; set; }
        [ForeignKey(typeof(Album))]
        public int AlbumId { get; set; }
        public string Album { get; set; }

        [ForeignKey(typeof(Genre))]
        public int GenreId { get; set; }
        public string Genre { get; set; }

        public int OrderNo { get; set; }
        public int Plays { get; set; }
        public int Skips { get; set; }
        public int RandomPlays { get; set; }

        private int _perCentRate;

        public int PerCentRate
        {
            get 
            {
                _perCentRate = repo.DoPercent(this);
                return _perCentRate; 
            }
            set { _perCentRate = value; }
        }
        
        //public int PerCentRate
        //{
        //    get
        //    {
        //        return repo.DoPercent(this);
        //    }
        //    //protected set { }
        //    public set { }
        //}

        public string ImageUrl { get; set; }

        public string FileName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateAddedToQuickPick { get; set; }

        private bool inTheBin = false;
        public bool InTheBin
        {
            get { return inTheBin; }
            set { inTheBin = value; }
        }
        public bool Liked { get; set; }
        public bool DisLiked { get; set; }

        public bool InQuickPick { get; set; }

        public bool InEditMode { get; set; }
        public string GSSongKey { get; set; }
        public string GSSongKeyUrl { get; set; }
        public string GSServerId { get; set; }
        public string GSSessionKey { get; set; }
        public string listeners { get; set; }
        public string mbid { get; set; }

        public override string ToString()
        {
            //return string.Format("e: {0}, t:{1}, e+t:{2} ({3})%", energy, tempo, all,  PerCentRate);
            return string.Format("rated: {0}", PerCentRate);
        }
       
        public bool Summary { get; set; }
        public bool PicGot { get; set; }
        public int key { get; set; }
        public string analysis_url { get; set; }
        public double energy { get; set; }
        public double liveness { get; set; }
        public double tempo { get; set; }
        public double speechiness { get; set; }
        public double acousticness { get; set; }
        public double instrumentalness { get; set; }
        public int mode { get; set; }
        public int time_signature { get; set; }
        public double duration { get; set; }
        public double loudness { get; set; }
        public string audio_md5 { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
    }

    public class TrackDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime DateAdded { get; set; }
        public double Rating { get; set; }

        public string GSSongKey { get; set; }
        public string GSSongKeyUrl { get; set; }
        public string GSServerId { get; set; }

        public string ArtistName { get; set; }

        public int key { get; set; }
        public string analysis_url { get; set; }
        public double energy { get; set; }
        public double liveness { get; set; }
        public double tempo { get; set; }
        public double speechiness { get; set; }
        public double acousticness { get; set; }
        public double instrumentalness { get; set; }
        public int mode { get; set; }
        public int time_signature { get; set; }
        public double duration { get; set; }
        public double loudness { get; set; }
        public string audio_md5 { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
    }

    public class LastFmTrackDTO
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string playcount { get; set; }
        public string listeners { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public Artist artist { get; set; }
        public List<TpImage> image { get; set; }
        public TpAttr attr { get; set; }
        public string OneImage { get; set; }
    }
    public class TpArtist
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
    }
    public class TpImage
    {
        public string text { get; set; }
        public string size { get; set; }
    }
    public class TpAttr
    {
        public string rank { get; set; }
    }

    //public static class Rating
    //{
    //    public static double EnergyAve { get; set; }
    //    public static double Liveness { get; set; }
    //    public static double Tempo { get; set; }
    //    public static double Loudness { get; set; }

    //    public static Rating(double energy, double tempo, double liveness, double loudness)
    //    {
    //        Energy = energy;
    //        Tempo = tempo;
    //        Liveness = liveness;
    //        Loudness = loudness;
    //        AddedUp = rating;
    //    }
    //}
}
