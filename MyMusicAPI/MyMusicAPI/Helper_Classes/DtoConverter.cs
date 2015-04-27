using MyMusicAPI.DTO;
using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMusicAPI.Helper_Classes
{
    public static class DtoConverter
    {
        public static List<Track> DtoTotracks(List<TrackDTO> trks)
        {
            List<Track> dtos = new List<Track>();
            foreach (var t in trks)
            {
                Track nt = new Track
                {                   
                    Name = t.Name,
                    Image = t.Image,
                    Genre = t.Genre,
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

        public static List<TrackDTO> TrackToDto(List<Track> trks)
        {
            List<TrackDTO> dtos = new List<TrackDTO>();
            foreach (var t in trks)
            {
                TrackDTO nt = new TrackDTO
                {
                    Name = t.Name,
                    Image = t.Image,
                    Genre = t.Genre,
                    GSSongKey = t.GSSongKey,
                    GSSongKeyUrl = t.GSSongKeyUrl,
                    GSServerId = t.GSServerId,
                    ArtistName = t.ArtistName,
                    key = t.key ?? 0,
                    analysis_url = t.analysis_url,
                    energy = t.energy ?? 0,
                    liveness = t.liveness ?? 0,
                    tempo = t.tempo ?? 0,
                    speechiness = t.speechiness ?? 0,
                    acousticness = t.acousticness ?? 0,
                    instrumentalness = t.instrumentalness ?? 0,
                    mode = t.mode ?? 0,
                    time_signature = t.time_signature ?? 0,
                    duration = t.duration ?? 0,
                    loudness = t.loudness ?? 0,
                    audio_md5 = t.audio_md5,
                    valence = t.valence ?? 0,
                    danceability = t.danceability ?? 0,
                    DateAdded = t.DateAdded//,
                    //Rating = t.Rating
                };
                dtos.Add(nt);
            }
            return dtos;
        }
    }
}