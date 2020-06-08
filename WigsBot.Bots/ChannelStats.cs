using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using WigsBot.DAL.Models.GuildPreferences;
using System.Collections.Generic;
using DSharpPlus.Entities;
using WigsBot.Core.Services.GuildPreferenceServices;
using Microsoft.EntityFrameworkCore;
using WigsBot.DAL;
using WigsBot.Bot.Models;
using WigsBot.Core.Services.Profiles;
using System.Linq;

namespace WigsBot.Bot
{
    public class ChannelStats
    {
        private readonly DbContextOptions<RPGContext> _options;

        private readonly IGuildPreferences _guildPreferences;
        private readonly IProfileService _profileService;

        public ChannelStats(
            IGuildPreferences guildPreferences,
            IProfileService profileService,
            DbContextOptions<RPGContext> options
            )
        {
            _guildPreferences = guildPreferences;
            _profileService = profileService;
            _options = options;
        }

        
    }
}
