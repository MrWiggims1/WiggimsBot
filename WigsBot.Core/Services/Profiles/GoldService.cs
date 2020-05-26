﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Bot;
using WigsBot.Core.Services.Items;
using WigsBot.Core.ViewModels;
using WigsBot.DAL;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Core.Services.Profiles
{
    public interface IGoldService
    {
        Task GrantGoldAsync(ulong discordId, ulong guildId, int GoldAmount);
        Task<int> ResetGold(ulong discordId, ulong guildId, int goldPerLevel);
        Task ResetAllGold(ulong guildId, int goldPerLevel);
        Task SetGold(ulong discordId, ulong guildId, int goldAmount);
        Task TransferGoldWTax(Profile payer, Profile payee, Profile wigsBotProfile, int goldAmount, decimal wigsBotTaxPercent);
        Task TransferGold(Profile payer, Profile payee, int goldAmount, bool allowDebt);
    }

    public class GoldService : IGoldService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;
        private readonly IRobbingItemService _robbingItemService;

        public GoldService(DbContextOptions<RPGContext> options, IProfileService profileService, IRobbingItemService robbingItemService)
        {
            _options = options;
            _profileService = profileService;
            _robbingItemService = robbingItemService;
        }

        /// <summary>
        /// Gives a user gold.
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="guildId"></param>
        /// <param name="GoldAmount"></param>
        /// <returns></returns>
        public async Task GrantGoldAsync(ulong discordId, ulong guildId, int GoldAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            checked { profile.Gold += GoldAmount; }

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the gold of a user based on what level they are.
        /// </summary>
        /// <param name="discordId">The members discord ID.</param>
        /// <param name="guildId">The Guilds Id.</param>
        /// <param name="goldPerLevel">The gold a member should get per level.</param>
        /// <returns>The new gold amount for the user.</returns>
        public async Task<int> ResetGold(ulong discordId, ulong guildId, int goldPerLevel)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);
            if (discordId == 629962329655607308U)
            {
                profile.Gold = 2000;
            }
            else
                checked { profile.Gold = (profile.Level * goldPerLevel) - _robbingItemService.GetInvWorth(profile).Result; }

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return profile.Gold;
        }

        /// <summary>
        /// Resets all the users gold in a guild.
        /// </summary>
        /// <param name="guildId">The Id of the guild.</param>
        /// <param name="goldPerLevel">The amount of gold per level.</param>
        /// <returns></returns>
        public async Task ResetAllGold(ulong guildId, int goldPerLevel)
        {
            using var context = new RPGContext(_options);

            List<Profile> profileList = await _profileService.GetAllGuildProfiles(guildId);

            foreach (var profile in profileList)
            {
                if (profile.DiscordId == 629962329655607308U)
                {
                    profile.Gold = 2000;
                }
                else
                    checked { profile.Gold = (profile.Level * goldPerLevel) - await _robbingItemService.GetInvWorth(profile); }
            }

            context.Profiles.UpdateRange(profileList);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sets a users gold to a specific amount.
        /// </summary>
        /// <param name="discordId">The members discord Id</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="goldAmount">The amount of gold the  member should have.</param>
        /// <returns></returns>
        public async Task SetGold(ulong discordId, ulong guildId, int goldAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.Gold = goldAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Transfers gold between 2 members.
        /// </summary>
        /// <param name="payer">The profile to take gold from.</param>
        /// <param name="payee">The profile to give gold to.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <param name="allowDebt">Should the payer be alowed to go into debt.</param>
        /// <returns></returns>
        public async Task TransferGold(Profile payer, Profile payee, int goldAmount, bool allowDebt)
        {
            using var context = new RPGContext(_options);

            if (!allowDebt)
            {
                if (goldAmount > payer.Gold && payer.DiscordId != 629962329655607308)
                    throw new InvalidOperationException("User does not have enough gold to afford this, please add a check within the command");
            }

            checked { payer.Gold -= goldAmount; }

            checked { payee.Gold += goldAmount; }

            context.Profiles.Update(payer);
            context.Profiles.Update(payee);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Transfers gold between 2 users with wiggims bot taking a cut.
        /// </summary>
        /// <param name="payer">The profile to take gold from.</param>
        /// <param name="payee">The profile to give gold to.</param>
        /// <param name="wigsBotProfile">The profile of the bot.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <param name="wigsBotTaxPercent">The % of gold to go to the bot</param>
        /// <returns></returns>
        public async Task TransferGoldWTax(Profile payer, Profile payee, Profile wigsBotProfile, int goldAmount, decimal wigsBotTaxPercent)
        {
            using var context = new RPGContext(_options);

            checked { payer.Gold -= goldAmount; }

            checked { payee.Gold += goldAmount - Convert.ToInt32(Convert.ToDecimal(goldAmount) * wigsBotTaxPercent); }

            checked { wigsBotProfile.Gold += Convert.ToInt32(Convert.ToDecimal(goldAmount) * wigsBotTaxPercent); }

            context.Profiles.Update(payer);
            context.Profiles.Update(payee);
            context.Profiles.Update(wigsBotProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}