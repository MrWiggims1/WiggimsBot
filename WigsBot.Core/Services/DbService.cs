using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL;
using WigsBot.DAL.Models.GuildPreferences;
using WigsBot.DAL.Models.Items;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services
{

   public interface IDbService
    {

    }

    public class DbService : IDbService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profilePreferences;

        public DbService(DbContextOptions<RPGContext> options, IProfileService profilePreferences)
        {
            _options = options;
            _profilePreferences = profilePreferences;
        }

        public async Task GenerateDbJsonBackup()
        {
            using var context = new RPGContext(_options);

            var profiles = await _profilePreferences.GetAllProfilesAsync();

            var backupJson = new DbJson()
            {
                Profiles = null,
                guildPreferences = null,
                robbingItems = null
            };

            foreach(var profile in profiles)
            {
                var profileBackup = new Profile()
                {
                    BattleNetUsername = profile.BattleNetUsername,
                    BeatSaberId = profile.BeatSaberId,
                    BoganCount = profile.BoganCount,
                    CODMWUsername = profile.CODMWUsername,
                    DiscordId = profile.DiscordId,
                    Gold = profile.Gold,
                    Gots = profile.Gots,
                    GuildId = profile.GuildId,
                    Id = profile.Id,
                    IsMimicable = profile.IsMimicable,
                    ItemJson = null, // need to do this as well
                    LeaveCount = profile.LeaveCount,
                    QuietMode = profile.QuietMode,
                    SpellCorrectCount = profile.SpellCorrectCount,
                    SpellErrorCount = profile.SpellErrorCount,
                    SpellErrorList = profile.SpellErrorList,
                    SteamId = profile.SteamId,
                    ToDoJson = null, // need to do this one as well
                    UplayUsername = profile.UplayUsername,
                    Xp = profile.Xp
                };  

                backupJson.Profiles.Add(profile.Id, profileBackup);
            }
        }

        public async Task RestorDbJsonBackup()
        {

        }

        public class DbJson
        {
            public Dictionary<int, RobbingItems> robbingItems { get; set; }
            public Dictionary<int, Profile> Profiles { get; set; }
            public Dictionary<int, GuildPreferences> guildPreferences { get; set; }
        }
    }
}
