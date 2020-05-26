using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IUplayIdService
    {
        /// <summary>
        /// Sets the uplay Id of a user.
        /// </summary>
        /// <param name="discordId">The Id of the member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="UplayId">The members uplay Id.</param>
        /// <returns></returns>
        Task SetUplayIdAsync(ulong discordId, ulong guildId, string UplayId);
    }

    public class UplayIdService : IUplayIdService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public UplayIdService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task SetUplayIdAsync(ulong discordId, ulong guildId, string UplayId)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.UplayUsername = UplayId;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}