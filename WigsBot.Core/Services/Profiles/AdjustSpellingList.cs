using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ISpellListService
    {
        /// <summary>
        /// Modifies the members incorrect spelling list.
        /// </summary>
        /// <param name="discordId">The members Id.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="arrayLength">The length of the guilds spelling list.</param>
        /// <param name="incorrectWords">The list of incorrect words</param>
        /// <returns></returns>
        Task SetSpellListAsync(ulong discordId, ulong guildId, int arrayLength, string[] incorrectWords);
    }

    public class SpellListService : ISpellListService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public SpellListService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task SetSpellListAsync(ulong discordId, ulong guildId, int arrayLength, string[] incorrectWords)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);
            //
            string[] prevSpellList = profile.SpellErrorList.Split(", ");

            string[] newSpellList = prevSpellList.Concat(incorrectWords).Where(x => !string.IsNullOrEmpty(x)).SkipLast(arrayLength).ToArray();
           
            profile.SpellErrorList = String.Join(", ", newSpellList);
            
            //
            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}