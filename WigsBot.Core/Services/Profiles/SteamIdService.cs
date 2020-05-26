using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ISteamIdService
    {
        /// <summary>
        /// Sets a users steam Id.
        /// </summary>
        /// <param name="discordId">The Id of the member</param>
        /// <param name="guildId">The guilds Id.</param>
        /// <param name="steamId">The members steam Id.</param>
        /// <returns></returns>
        Task SetSteamIdAsync(ulong discordId, ulong guildId, long steamId);
    }

    public class SteamIdService : ISteamIdService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public SteamIdService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task SetSteamIdAsync(ulong discordId, ulong guildId, long steamId)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.SteamId = steamId;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}