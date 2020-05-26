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
        Task<GuildPreferences> GetOrCreateGuildPreferences(ulong GuildId);
    }

    public class GuildPreferencesService : IGuildPreferences
    {
        private readonly DbContextOptions<RPGContext> _options;

        public GuildPreferencesService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        /// <summary>
        /// Will get the guilds prefences or create a Db entity if none exist.
        /// </summary>
        /// <param name="guildId">The Id of the discord guild.</param>
        /// <returns></returns>
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
                AssignableRoleJson = "{\"Roles\":[]}"
            };

            context.Add(guildPrefences);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return guildPrefences;
        }

    }
}
