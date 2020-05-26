using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.DAL;
using WigsBot.DAL.Models.GuildPreferences;

namespace WigsBot.Core.Services.GuildPreferencesServices
{
    public interface ISpellingSettingsService
    {
        /// <summary>
        /// Sets if spelling should be enabled within this guild.
        /// </summary>
        /// <param name="guildId">The id of the discord guild.</param>
        /// <param name="enabled">True if you wish the guild to track spelling.</param>
        /// <returns></returns>
        public Task ToggleSpellTracking(ulong guildId, bool enabled);

        /// <summary>
        /// Change the length of the spelling error list.
        /// </summary>
        /// <param name="guildId">The id of the discord guild.</param>
        /// <param name="listLength">The number of words to keep track of.</param>
        /// <returns></returns>
        public Task SetSpellListLength(ulong guildId, int listLength);
    }

    public class SpellingSettingsService : ISpellingSettingsService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IGuildPreferences _guildPreferences;

        public SpellingSettingsService(DbContextOptions<RPGContext> options, IGuildPreferences guildPreferences)
        {
            _options = options;
            _guildPreferences = guildPreferences;
        }

        public async Task ToggleSpellTracking(ulong guildId, bool enabled)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(guildId).ConfigureAwait(false);

            guildPreferences.SpellingEnabled = enabled;

            context.GuildPreferences.Update(guildPreferences);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetSpellListLength(ulong guildId, int listLength)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(guildId).ConfigureAwait(false);

            guildPreferences.ErrorListLength = listLength;

            context.GuildPreferences.Update(guildPreferences);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
