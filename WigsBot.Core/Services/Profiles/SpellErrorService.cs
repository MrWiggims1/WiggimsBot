using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ISpellErrorService
    {
        /// <summary>
        /// Gives a user a count towards the number of incorrect words.
        /// </summary>
        /// <param name="discordId">The Id of the member.</param>
        /// <param name="guildId">The guilds id.</param>
        /// <param name="SpellErrorAmount">The amount of incorrect counts to give.</param>
        /// <returns></returns>
        Task GrantSpellErrorAsync(ulong discordId, ulong guildId, int SpellErrorAmount);
    }

    public class SpellErrorService : ISpellErrorService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public SpellErrorService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task GrantSpellErrorAsync(ulong discordId, ulong guildId, int SpellErrorAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.SpellErrorCount += SpellErrorAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}