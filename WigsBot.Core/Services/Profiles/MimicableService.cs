using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IMimicableService
    {
        /// <summary>
        /// Lets a user toggle if they should be mimicked or not.
        /// </summary>
        /// <param name="discordId">The Id of the member</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="enabled">If the member can be mimicked or not.</param>
        /// <returns></returns>
        Task SetMimicableAsync(ulong discordId, ulong guildId, bool enabled);

        /// <summary>
        /// Adds a point to hoe many times a member has been mimicked.
        /// </summary>
        /// <param name="profile">The members Profile.</param>
        /// <returns></returns>
        Task TackAMimic(Profile profile);
    }

    public class MimicableService : IMimicableService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public MimicableService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task SetMimicableAsync(ulong discordId, ulong guildId, bool enabled)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.IsMimicable = enabled;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task TackAMimic(Profile profile)
        {
            using var context = new RPGContext(_options);

            profile.TimesMimicked++;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}