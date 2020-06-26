using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Bot.Models;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.DAL;
using WigsBot.DAL.Models.GuildPreferences;

namespace WigsBot.Core.Services.GuildPreferencesServices.StatChannels
{
    public interface IStatChannelService
    {
        Task CreateOrModifyStatChannel(ulong guildId, ulong channelId, StatOption stat, string message);
        Task DeleteStatChannel(ulong guildId, StatOption stat);
        Task CheckForGuildsStatCatergoryChannel(GuildPreferences guildPreferences);
    }

    public class StatChannelService : IStatChannelService
    {

        private readonly DbContextOptions<RPGContext> _options;
        private readonly IGuildPreferences _guildPreferences;
        public StatChannelService(DbContextOptions<RPGContext> options, IGuildPreferences guildPreferences)
        {
            _options = options;
            _guildPreferences = guildPreferences;
        }

        public async Task CreateOrModifyStatChannel(ulong guildId, ulong channelId, StatOption stat, string message)
        {

            using var context = new RPGContext(_options);

            GuildPreferences guildPrefs = await _guildPreferences.GetOrCreateGuildPreferences(guildId);

            CheckForGuildsStatCatergoryChannel(guildPrefs);

            var channelStat = guildPrefs.StatChannels.Where(x => x.StatOption == stat).FirstOrDefault();

            if (channelStat == null)
            {
                if (guildPrefs.StatChannels.Count >= 3)
                {
                    throw new Exception("You can only have 3 stats at a time, to create a new one, you must delete one of the channels and type `w@guild stats update true`, then try to create your stat channel again.");
                }

                channelStat = new StatChannel()
                {
                    StatMessage = message,
                    StatOption = stat,
                    ChannelId = channelId,
                    GuildPreferencesId = guildPrefs.Id
                };
                context.Add(channelStat);
            }
            else
            {
                channelStat.StatMessage = message;
                channelStat.ChannelId = channelId;
                channelStat.StatOption = stat;
                context.Update(channelStat);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteStatChannel(ulong guildId, StatOption stat)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPrefs = await _guildPreferences.GetOrCreateGuildPreferences(guildId);

            CheckForGuildsStatCatergoryChannel(guildPrefs);

            var channelStat = guildPrefs.StatChannels.Where(x => x.StatOption == stat).FirstOrDefault();

            if (channelStat == null)
                return;

            context.Remove(channelStat);

            await context.SaveChangesAsync();
        }

        public async Task CheckForGuildsStatCatergoryChannel(GuildPreferences guildPreferences)
        {
            if (guildPreferences.StatChannelCatergoryId == 0u)
                throw new Exception("No channel category has been set for the guild, please set one now with `w@guild stats setcategory <Catergories Id>`.");
        }
    }
}
