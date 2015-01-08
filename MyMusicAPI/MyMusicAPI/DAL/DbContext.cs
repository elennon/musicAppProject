using MyMusicAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MyMusicAPI.DAL
{
    public class MusicCentralContext : DbContext
    {
        
        public MusicCentralContext() : base("MusicCentralDB")
        {
            this.Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DbInitializer());
        }
        public DbSet<RadioGenre> RadioGenre { get; set; }
        public DbSet<RadioStream> RadioStream { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
    public class DbInitializer : System.Data.Entity.DropCreateDatabaseAlways<MusicCentralContext>
    {
    }
}
