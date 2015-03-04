using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Models
{
    public class DataGroup
    {
        public DataGroup()
        { }

        public DataGroup(String UniqueId, String title, String description, String imagePath)
        {
            this.UniqueId = UniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
        }
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
