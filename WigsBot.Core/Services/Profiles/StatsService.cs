using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IStatsService
    {
        /// <summary>
        /// Process the kill stats for a 2 members.
        /// </summary>
        /// <param name="killer">The Killers profile.</param>
        /// <param name="vicim">The victims Profile.</param>
        /// <param name="victimDie">Did the victim die?</param>
        /// <returns></returns>
        Task ProccessKillStat(Profile killer, Profile vicim, bool victimDie);
    };

    public class StatsService : IStatsService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public StatsService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task ProccessKillStat(Profile killer, Profile vicim, bool victimDie)
        {
            using var context = new RPGContext(_options);

            if (victimDie)
            {
                killer.TimesMurdered ++;
                vicim.TimesBeenMurdered ++;
            }
            else
            {
                killer.TimesFailedToMurder ++;
                vicim.TimesEscapedMurder ++;
            }

            context.Profiles.Update(killer);
            context.Profiles.Update(vicim);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
