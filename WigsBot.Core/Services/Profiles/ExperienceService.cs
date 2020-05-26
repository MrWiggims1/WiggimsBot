using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IExperienceService
    {
        Task<GrantXpViewModel> GrantXpAsync(ulong discordId, ulong guildId, int xpAmount);
    }

    public class ExperienceService : IExperienceService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public ExperienceService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        /// <summary>
        /// Gives a member Experience points.
        /// </summary>
        /// <param name="discordId">The members Id.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="xpAmount">The change in the members xp.</param>
        /// <returns></returns>
        public async Task<GrantXpViewModel> GrantXpAsync(ulong discordId, ulong guildId, int xpAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            int levelBefore = profile.Level;

            profile.Xp += xpAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            int levelAfter = profile.Level;

            return new GrantXpViewModel
            {
                Profile = profile,
                LevelledUp = levelAfter > levelBefore
            };
        }
    }
}