using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IBoganCountService
    {
        /// <summary>
        /// Changes the members bogan word count.
        /// </summary>
        /// <param name="discordId">The members Id.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="BoganAmount">The change in bogan words.</param>
        /// <returns></returns>
        Task GrantBoganCountAsync(ulong discordId, ulong guildId, int BoganAmount);
    }

    public class BoganCountService : IBoganCountService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public BoganCountService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        
        public async Task GrantBoganCountAsync(ulong discordId, ulong guildId, int BoganAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.BoganCount += BoganAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}