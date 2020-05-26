using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IGotService
    {
        /// <summary>
        /// Grants a got to a member.
        /// </summary>
        /// <param name="discordId">The id of the member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="GotAmount">The number of gots to add.</param>
        /// <returns>It returns a boolean on weather or not they leveled up, not sure how it works, just copy and paste the code from somewhere else.</returns>
        Task<GrantGotViewModel> GrantGotAsync(ulong discordId, ulong guildId, int GotAmount);
    }

    public class GotService : IGotService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public GotService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task<GrantGotViewModel> GrantGotAsync(ulong discordId, ulong guildId, int GotAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            int GotlevelBefore = profile.GotLevel;

            profile.Gots += GotAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            int GotlevelAfter = profile.GotLevel;

            return new GrantGotViewModel
            {
                Profile = profile,
                GotLevelledUp = GotlevelAfter > GotlevelBefore
            };
        }
    }
}