using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.Model
{
    public sealed class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string GSSongKey { get; set; }
        public string GSSongKeyUrl { get; set; }
        public string GSServerId { get; set; }
        public string GSSessionKey { get; set; }
        public string GSSongId { get; set; }

        public string ArtistName { get; set; }
        
    }
}
