using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    /// <summary>
    /// Service for creating and obtaining profiles from the Database.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Gets a list of all the profiles available in the Database.
        /// </summary>
        /// <returns>A list of all profiles.</returns>
        List<Profile> GetAllProfilesAsync();

        /// <summary>
        /// Gets or creates a members profile.
        /// </summary>
        /// <param name="discordId">The Id of the member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <returns>The members profile.</returns>
        Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId);

        /// <summary>
        /// Gets all the profiles within the guild.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <returns>List of guild profiles.</returns>
        List<Profile> GetAllGuildProfiles(ulong guildId);

        /// <summary>
        /// Fixes everyones username in the guild.
        /// </summary>
        /// <param name="guildId">The guild Id</param>
        /// <param name="usernameDictionary">discord id - username.</param>
        /// <returns></returns>
        Task FixUserNames(ulong guildId, Dictionary<ulong, string> usernameDictionary);
    }

    public class ProfileService : IProfileService
    {
        private readonly DbContextOptions<RPGContext> _options;

        public ProfileService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }
        
        public List<Profile> GetAllProfilesAsync()
        {
            using var context = new RPGContext(_options);

            var profileEnumerable = context.Profiles.ToList();

            List<Profile> profiles = profileEnumerable.ToList();

            return profiles;
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId)
        {
            using var context = new RPGContext(_options);
            
            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile != null) { return profile; }

            profile = new Profile
            {
                DiscordId = discordId,
                GuildId = guildId,
                UserName = "undefined",
                Xp = 0,
                Gold = 0,
                Gots = 0,
                LeaveCount = 0,
                SpellErrorCount = 1,
                SpellCorrectCount = 1,
                BoganCount = 0,
                BeatSaberId = 0L,
                UplayUsername = "not set",
                SpellErrorList = "still, empty",
                ToDoJson = "{\"lists\":[{\"name\":\"Todo List\",\"tasks\":[{\"name\":\"Add first item to todolist!\",\"description\":\"Use the w!todo add task command to add a new task to your list\",\"done\":false}]}]}",
                IsMimicable = true,
                ItemJson = "{\"Robbing\":[{\"Id\":8,\"Count\":1},{\"Id\":12,\"Count\":0}]}",
                QuietMode = false
            };

            context.Add(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return profile;
        }

        public List<Profile> GetAllGuildProfiles(ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profileEnumerable = context.Profiles.ToList().Where(x => x.GuildId == guildId);

            List<Profile> profiles = profileEnumerable.ToList();

            return profiles;
        }

        public async Task FixUserNames(ulong guildId, Dictionary<ulong,string> usernameDictionary)
        {
            using var context = new RPGContext(_options);

            var members = GetAllGuildProfiles(guildId);

            foreach (var profile in members)
            {
                try
                {
                    profile.UserName = usernameDictionary[usernameDictionary.Keys.Where(x => x == profile.DiscordId).Single()];
                }
                catch { }
            }

            context.UpdateRange(members);

            await context.SaveChangesAsync();
        }
    }
}
