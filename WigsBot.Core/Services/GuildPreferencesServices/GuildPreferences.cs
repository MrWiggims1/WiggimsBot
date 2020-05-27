using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WigsBot.DAL;
using WigsBot.DAL.Models.GuildPreferences;

namespace WigsBot.Core.Services.GuildPreferenceServices
{
    public interface IGuildPreferences
    {
        /// <summary>
        /// Will get the guilds preferences or create a Db entity if none exist.
        /// </summary>
        /// <param name="guildId">The Id of the discord guild.</param>
        /// <returns>All the preferences of the guild.</returns>
        Task<GuildPreferences> GetOrCreateGuildPreferences(ulong GuildId);

        /// <summary>
        /// Sets how much gold a member should earn for leveling up within the guild.
        /// </summary>
        /// <param name="guildId">The Id of the guild.</param>
        /// <param name="goldPerLvlUp">The amount of gold to earn.</param>
        /// <returns></returns>
        Task SetGoldPerLvlUp(ulong guildId, int goldPerLvlUp);

        /// <summary>
        /// The id of the role that users will be given if they are set to timeout.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <param name="roleId">The Id of the role.</param>
        /// <returns></returns>
        Task SetTimeoutRole(ulong guildId, ulong roleId);

        /// <summary>
        /// Sets the channel that the member who was sent to timeout can see.
        /// </summary>
        /// <param name="guildId">The Id of the guild/</param>
        /// <param name="channelId">The Id of the channel.</param>
        /// <returns></returns>
        Task SetTimeoutChannel(ulong guildId, ulong channelId);

        /// <summary>
        /// Sets the Id of the role that the bot should add to a new member automatically when they join.
        /// </summary>
        /// <param name="guildId">The Id of the guild.</param>
        /// <param name="roleId">The Id of the role.</param>
        /// <returns></returns>
        Task SetAutoRole(ulong guildId, ulong roleId);
    }

    public class GuildPreferencesService : IGuildPreferences
    {
        private readonly DbContextOptions<RPGContext> _options;

        public GuildPreferencesService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        public async Task<GuildPreferences> GetOrCreateGuildPreferences(ulong guildId)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPrefences = await context.GuildPreferences
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.GuildId == guildId).ConfigureAwait(false);

            if (guildPrefences != null) { return guildPrefences; }

            guildPrefences = new GuildPreferences
            {
                GuildId = guildId,
                XpPerMessage = 1,
                SpellingEnabled = false,
                ErrorListLength = 10,
                AssignableRoleJson = "{\"Roles\":[]}",
                GoldPerLevelUp = 50,
                IsGoldEnabled = true
            };

            context.Add(guildPrefences);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return guildPrefences;
        }

        public async Task SetGoldPerLvlUp(ulong guildId, int goldPerLvlUp)
        {
            using var context = new RPGContext(_options);

            var guildPrefs = await GetOrCreateGuildPreferences(guildId);

            guildPrefs.GoldPerLevelUp = goldPerLvlUp;

            context.GuildPreferences.Update(guildPrefs);

            await context.SaveChangesAsync();
        }

        public async Task SetTimeoutRole(ulong guildId, ulong roleId)
        {
            using var context = new RPGContext(_options);

            var guildPrefs =  await GetOrCreateGuildPreferences(guildId);

            guildPrefs.TimeoutRoleId = roleId;

            context.GuildPreferences.Update(guildPrefs);

            await context.SaveChangesAsync();
        }

        public async Task SetTimeoutChannel(ulong guildId, ulong channelId)
        {
            using var context = new RPGContext(_options);

            var guildPrefs = await GetOrCreateGuildPreferences(guildId);

            guildPrefs.TimeoutTextChannelId = channelId;

            context.GuildPreferences.Update(guildPrefs);

            await context.SaveChangesAsync();
        }

        public async Task SetAutoRole(ulong guildId, ulong roleId)
        {
            using var context = new RPGContext(_options);

            var guildPrefs = await GetOrCreateGuildPreferences(guildId);

            guildPrefs.AutoRole = roleId;

            context.GuildPreferences.Update(guildPrefs);

            await context.SaveChangesAsync();
        }
    }
}
