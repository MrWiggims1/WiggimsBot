using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IQuietModeService
    {
        Task SetQuietModeAsync(ulong discordId, ulong guildId, bool enabled);
    }

    public class QuietModeService : IQuietModeService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public QuietModeService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        /// <summary>
        /// Sets weather or not if the member should recive notifications.
        /// </summary>
        /// <param name="discordId">The Id of the member</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="enabled">If notifications should be enabled or not.</param>
        /// <returns></returns>
        public async Task SetQuietModeAsync(ulong discordId, ulong guildId, bool enabled)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.QuietMode = enabled;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}