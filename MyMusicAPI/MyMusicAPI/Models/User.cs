//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyMusicAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.UserTracks = new HashSet<UserTrack>();
        }
    
        public int Id { get; set; }
        public string UserId { get; set; }
    
        public virtual ICollection<UserTrack> UserTracks { get; set; }
    }
}
