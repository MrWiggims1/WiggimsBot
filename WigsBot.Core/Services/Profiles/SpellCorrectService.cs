using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ISpellCorrectService
    {
        Task GrantSpellCorrectAsync(ulong discordId, ulong guildId, int SpellCorrectAmount);
    }

    public class SpellCorrectService : ISpellCorrectService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public SpellCorrectService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        /// <summary>
        /// Gives a user a count towards the number of correct words.
        /// </summary>
        /// <param name="discordId">The Id of the member.</param>
        /// <param name="guildId">The guilds Id.</param>
        /// <param name="SpellCorrectAmount">The amount of correct counts to give.</param>
        /// <returns></returns>
        public async Task GrantSpellCorrectAsync(ulong discordId, ulong guildId, int SpellCorrectAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.SpellCorrectCount += SpellCorrectAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}