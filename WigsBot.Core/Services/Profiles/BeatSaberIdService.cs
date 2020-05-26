using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IBeatSaberIdService
    {
        /// <summary>
        /// Sets the beatsaber Id of a member.
        /// </summary>
        /// <param name="discordId">The members Id.</param>
        /// <param name="guildId">The guilds Id.</param>
        /// <param name="beatSaberId">The members beatsaber Id.</param>
        /// <returns></returns>
        Task SetBeatSaberIdAsync(ulong discordId, ulong guildId, long beatSaberId);
    }

    public class BeatSaberIdService : IBeatSaberIdService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public BeatSaberIdService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task SetBeatSaberIdAsync(ulong discordId, ulong guildId, long beatSaberId)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.BeatSaberId = beatSaberId;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}