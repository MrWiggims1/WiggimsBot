using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface ILeaveService
    {
        Task GrantLeaveAsync(ulong discordId, ulong guildId, int LeaveAmount);
    }

    public class LeaveService : ILeaveService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public LeaveService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        /// <summary>
        /// Add a leave to a user, this is used to track how many times a member has joined the server (more of a joke really, not very useful)
        /// </summary>
        /// <param name="discordId">The Id of the member.</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="LeaveAmount">Te number of leaves to add.</param>
        /// <returns></returns>
        public async Task GrantLeaveAsync(ulong discordId, ulong guildId, int LeaveAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.LeaveCount += LeaveAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}