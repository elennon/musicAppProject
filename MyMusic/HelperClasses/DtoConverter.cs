using MyMusic.DAL;
using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.HelperClasses
{
    public static class DtoConverter
    {
        public static List<TrackDTO> TrackToDTO(List<Track> trks)
        {
            List<TrackDTO> dtos = new List<TrackDTO>();
            foreach (var t in trks)
            {
                TrackDTO dto = new TrackDTO
                {
                    Id = t.TrackId,
                    Name = t.Name,
                    Image = t.ImageUrl,
                    GSSongKey = t.GSSongKey,
                    GSSongKeyUrl = t.GSSongKeyUrl,
                    GSServerId = t.GSServerId,
                    ArtistName = t.ArtistName,
                    key = t.key,
                    analysis_url = t.analysis_url,
                    energy = t.energy,
                    liveness = t.liveness,
                    tempo = t.tempo,
                    speechiness = t.speechiness,
                    acousticness = t.acousticness,
                    instrumentalness = t.instrumentalness,
                    mode = t.mode,
                    time_signature = t.time_signature,
                    duration = t.duration,
                    loudness = t.loudness,
                    audio_md5 = t.audio_md5,
                    valence = t.valence,
                    danceability = t.danceability,
                    DateAdded = t.DateAdded//,
                    //Rating = t.Rating
                };
                dtos.Add(dto);
            }
            return dtos;
        }

        public static List<Track> DtoTotracks(List<TrackDTO> trks)
        {
            List<Track> dtos = new List<Track>();
            foreach (var t in trks)
            {
                Track nt = new Track
                {
                    Name = t.Name,
                    ImageUrl = t.Image,
                    GSSongKey = t.GSSongKey,
                    GSSongKeyUrl = t.GSSongKeyUrl,
                    GSServerId = t.GSServerId,
                    ArtistName = t.ArtistName,
                    key = t.key,
                    analysis_url = t.analysis_url,
                    energy = t.energy,
                    liveness = t.liveness,
                    tempo = t.tempo,
                    speechiness = t.speechiness,
                    acousticness = t.acousticness,
                    instrumentalness = t.instrumentalness,
                    mode = t.mode,
                    time_signature = t.time_signature,
                    duration = t.duration,
                    loudness = t.loudness,
                    audio_md5 = t.audio_md5,
                    valence = t.valence,
                    danceability = t.danceability,
                    DateAdded = t.DateAdded//,
                    //Rating = t.Rating
                };
                dtos.Add(nt);
            }
            return dtos;
        }

        public static Track EcheoNestRoot2Track(EcheoNestRoot t)
        {                       
            Track nt = new Track
            {
                Name = t.Name,               
                ArtistName = t.ArtistName,             
                analysis_url = t.analysis_url,
                energy = t.energy,
                liveness = t.liveness,
                tempo = t.tempo,
                speechiness = t.speechiness,
                acousticness = t.acousticness,
                instrumentalness = t.instrumentalness,
                mode = t.mode,
                time_signature = t.time_signature,
                duration = t.duration,
                loudness = t.loudness,
                audio_md5 = t.audio_md5,
                valence = t.valence,
                danceability = t.danceability  ,
                Genre = t.Genre,
                ImageUrl = t.Image
            };                
            return nt;
        }
    }
}
