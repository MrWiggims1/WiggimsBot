using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WigsBot.DAL.Models.GuildPreferences;
using WigsBot.DAL.Models.Items;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<GuildPreferences> GuildPreferences { get; set; }
        public DbSet<RobbingItems> RobbingItems { get; set; }
        public DbSet<StatChannel> StatChannels { get; set; }
    }
}
