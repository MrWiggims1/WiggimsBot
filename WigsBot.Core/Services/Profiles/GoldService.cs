using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Gives a user gold.
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="guildId"></param>
        /// <param name="GoldAmount"></param>
        /// <returns></returns>
        Task GrantGoldAsync(ulong discordId, ulong guildId, int GoldAmount);

        /// <summary>
        /// Resets the gold of a user based on what level they are.
        /// </summary>
        /// <param name="discordId">The members discord ID.</param>
        /// <param name="guildId">The Guilds Id.</param>
        /// <param name="goldPerLevel">The gold a member should get per level.</param>
        /// <returns>The new gold amount for the user.</returns>
        Task<int> ResetGold(ulong discordId, ulong guildId, int goldPerLevel);

        /// <summary>
        /// Resets all the users gold in a guild.
        /// </summary>
        /// <param name="guildId">The Id of the guild.</param>
        /// <param name="goldPerLevel">The amount of gold per level.</param>
        /// <returns></returns>
        Task ResetAllGold(ulong guildId, int goldPerLevel);

        /// <summary>
        /// Sets a users gold to a specific amount.
        /// </summary>
        /// <param name="discordId">The members discord Id</param>
        /// <param name="guildId">The guild Id.</param>
        /// <param name="goldAmount">The amount of gold the  member should have.</param>
        /// <returns></returns>
        Task SetGold(ulong discordId, ulong guildId, int goldAmount);

        /// <summary>
        /// Transfers gold between 2 members.
        /// </summary>
        /// <param name="payer">The profile to take gold from.</param>
        /// <param name="payee">The profile to give gold to.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <param name="allowDebt">Should the payer be allowed to go into debt.</param>
        /// <returns></returns>
        Task TransferGold(Profile payer, Profile payee, int goldAmount, bool allowDebt);

        /// <summary>
        /// Transfers gold between 2 members.
        /// </summary>
        /// <param name="payerId">The discord Id of the payer.</param>
        /// <param name="payeeId">The discord Id of the payee.</param>
        /// <param name="guildId">The discord Id of the guild.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <param name="allowDebt">Should the payer be allowed to go into debt.</param>
        /// <returns></returns>
        Task TransferGold(ulong payerId, ulong payeeId, ulong guildId, int goldAmount, bool allowDebt);

        /// <summary>
        /// Transfers gold between 2 users with wiggims bot taking a cut.
        /// </summary>
        /// <param name="payer">The profile to take gold from.</param>
        /// <param name="payee">The profile to give gold to.</param>
        /// <param name="wigsBotProfile">The profile of the bot.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <param name="wigsBotTaxPercent">The % of gold to go to the bot</param>
        /// <returns></returns>
        Task TransferGoldWTax(Profile payer, Profile payee, Profile wigsBotProfile, int goldAmount, decimal wigsBotTaxPercent);

        /// <summary>
        /// Gives a member their daily and resets the cooldown time on the member.
        /// </summary>
        /// <param name="profile">The members Profile.</param>
        /// <param name="botProfile">The bots Profile.</param>
        /// <param name="goldNum">The amount of gold to give the member.</param>
        /// <returns></returns>
        Task ProccessMemberDaily(Profile profile, Profile botProfile, int goldNum);

        /// <summary>
        /// Transfers the gold between attacker and defender appropriately and resets the cooldown of the member attacking.
        /// </summary>
        /// <param name="attacker">The profile of the member attacking.</param>
        /// <param name="victim">The profile of the member defending.</param>
        /// <param name="goldNum">The amount of gold to transfer.</param>
        /// <param name="robberySuccessfull">Was the robbery successful? if so the gold will be transfered to the attacker, otherwise it goes to the defender.</param>
        /// <returns></returns>
        Task ProccessMembersRobbing(Profile attacker, Profile victim, int goldNum, bool robberySuccessfull);

        /// <summary>
        /// Transfers gold between the member and the bot, as well as tracks info within database.
        /// </summary>
        /// <param name="profile">The profile of the member.</param>
        /// <param name="botProfile"> The profile of the bot.</param>
        /// <param name="goldNum">The winnings/losses.</param>
        /// <param name="memberWonGame">Did the member win the game?</param>
        /// <returns></returns>
        Task ProccessMemberRoulette(Profile profile, Profile botProfile, int goldNum, bool memberWonGame);

        /// <summary>
        /// Transfers gold between 2 members while also tracking the stats for tracking the pays.
        /// </summary>
        /// <param name="payer">The profile to take gold from.</param>
        /// <param name="payee">The profile to give gold to.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <returns></returns>
        Task ProccessMembersPayingEachother(Profile payer, Profile payee, int goldAmount);

        /// <summary>
        /// Transfers gold between 2 members while also tracking the stats for tracking the pays.
        /// </summary>
        /// <param name="payerId">The profile to take gold from.</param>
        /// <param name="payeeId">The profile to give gold to.</param>
        /// <param name="guildId">The Id of the guild.</param>
        /// <param name="goldAmount">The amount of gold to transfer.</param>
        /// <returns></returns>
        Task ProccessMembersPayingEachother(ulong payerId, ulong payeeId, ulong guildId, int goldAmount);

        /// <summary>
        /// Transfers the gold between the member and bot as well as tracks the stats.
        /// </summary>
        /// <param name="memberId">The discord Id of the member.</param>
        /// <param name="botId">THe discord Id of the bot.</param>
        /// <param name="guildId">The discord guild Id.</param>
        /// <param name="goldNum">The amount of gold to transfer between the member and the bot.</param>
        /// <param name="memberWonGame">Did the member win the game? if so gold will be transfered from the bot otherwise the bot gains the gold.</param>
        /// <returns></returns>
        Task ProccessMemberCoinFlip(ulong memberId, ulong botId, ulong guildId, int goldNum, bool memberWonGame);

        /// <summary>
        /// Transfers the gold between the member and bot as well as tracks the stats.
        /// </summary>
        /// <param name="profile">The profile of the member.</param>
        /// <param name="botProfile">The profile of the bot.</param>
        /// <param name="goldNum">The amount of gold to transfer between the member and the bot.</param>
        /// <param name="memberWonGame">Did the member win the game? if so gold will be transfered from the bot otherwise the bot gains the gold.</param>
        /// <returns></returns>
        Task ProccessMemberCoinFlip(Profile profile, Profile botProfile, int goldNum, bool memberWonGame);
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
        
        public async Task GrantGoldAsync(ulong discordId, ulong guildId, int GoldAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            checked { profile.Gold += GoldAmount; }

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> ResetGold(ulong discordId, ulong guildId, int goldPerLevel)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);
            if (discordId == 629962329655607308U)
            {
                profile.Gold = 2000;
            }
            else
                checked { profile.Gold = (profile.Level * goldPerLevel) - _robbingItemService.GetInvWorth(profile); }

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return profile.Gold;
        }

        public async Task ResetAllGold(ulong guildId, int goldPerLevel)
        {
            using var context = new RPGContext(_options);

            List<Profile> profileList = _profileService.GetAllGuildProfiles(guildId);

            foreach (var profile in profileList)
            {
                if (profile.DiscordId == 629962329655607308U)
                {
                    profile.Gold = 2000;
                }
                else
                    checked { profile.Gold = (profile.Level * goldPerLevel) - _robbingItemService.GetInvWorth(profile); }
            }

            context.Profiles.UpdateRange(profileList);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetGold(ulong discordId, ulong guildId, int goldAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildId).ConfigureAwait(false);

            profile.Gold = goldAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

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

        public async Task TransferGold(ulong payerId, ulong payeeId, ulong guildId, int goldAmount, bool allowDebt)
        {
            using var context = new RPGContext(_options);

            var payer = await _profileService.GetOrCreateProfileAsync(payerId, guildId);
            var payee = await _profileService.GetOrCreateProfileAsync(payeeId, guildId);

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

        public async Task ProccessMemberDaily(Profile profile, Profile botProfile, int goldNum)
        {
            using var context = new RPGContext(_options);

            profile.DailyCooldown = DateTime.Now;
            profile.Gold += goldNum;
            profile.TotalDailyEarnings += goldNum;
            profile.DailiesCollected++;

            botProfile.Gold -= goldNum;

            context.Profiles.Update(profile);
            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMembersRobbing(Profile attacker, Profile victim, int goldNum, bool robberySuccessfull)
        {
            using var context = new RPGContext(_options);

            if(!robberySuccessfull)
            {
                checked { attacker.RobbingAttackLost++; }
                checked { victim.RobbingDefendWon++; }

                checked { attacker.GoldLostFines += goldNum; }
                checked { victim.GoldGainedFines += goldNum; }

                //Make sure set to negative after you track the results not before.
                goldNum = -goldNum;
            }
            else
            {
                checked { attacker.RobbingAttackWon++; }
                checked { victim.RobbingDefendLost++; }

                checked { attacker.GoldStolen += goldNum; }
                checked { victim.GoldLostFromTheft += goldNum; }
            }

            attacker.RobbingCooldown = DateTime.Now;

            checked { attacker.Gold += goldNum; }
            checked { victim.Gold -= goldNum; }

            context.Profiles.Update(attacker);
            context.Profiles.Update(victim);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMembersPayingEachother(Profile payer, Profile payee, int goldAmount)
        {
            using var context = new RPGContext(_options);

            checked { payer.Gold -= goldAmount; }

            checked { payee.Gold += goldAmount; }

            if (payee.DiscordId != 629962329655607308) // The member being paid is wiggims bot then don't include the stats to the update.
            {
                payer.TimesPayedOtherMember++;
                checked { payer.GoldPayedToMembers += goldAmount; }

                payee.TimesPayedByMember++;
                checked { payee.GoldRecivedFromMembers += goldAmount; }
            }


            context.Profiles.Update(payer);
            context.Profiles.Update(payee);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMembersPayingEachother(ulong payerId, ulong payeeId, ulong guildId, int goldAmount)
        {
            using var context = new RPGContext(_options);

            var payer = await _profileService.GetOrCreateProfileAsync(payerId, guildId);
            var payee = await _profileService.GetOrCreateProfileAsync(payeeId, guildId);

            checked { payer.Gold -= goldAmount; }

            checked { payee.Gold += goldAmount; }

            payer.TimesPayedOtherMember++;
            checked { payer.GoldPayedToMembers += goldAmount; }

            payee.TimesPayedByMember++;
            checked { payee.GoldRecivedFromMembers += goldAmount; }


            context.Profiles.Update(payer);
            context.Profiles.Update(payee);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMemberRoulette(Profile profile, Profile botProfile, int goldNum, bool memberWonGame)
        {
            using var context = new RPGContext(_options);

            if (!memberWonGame)
            {
                profile.RouletteFails++;
                profile.GoldLostRoulette += goldNum;

                profile.Gold -= goldNum;
                botProfile.Gold += goldNum;
            }
            else
            {
                profile.RouletteSuccesses++;
                profile.GoldWonRoulette += goldNum;

                profile.Gold += goldNum;
                botProfile.Gold -= goldNum;
            }

            

            context.Profiles.Update(profile);
            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMemberCoinFlip(ulong memberId, ulong botId, ulong guildId, int goldNum, bool memberWonGame)
        {
            using var context = new RPGContext(_options);

            var profile = await _profileService.GetOrCreateProfileAsync(memberId, guildId);
            var botProfile = await _profileService.GetOrCreateProfileAsync(botId, guildId);

            if (!memberWonGame)
            {
                profile.CoindFlipsLost++;
                checked { profile.GoldLostCoinFlip += goldNum; }

                //Make sure set to negative after you track the results not before.
                goldNum = -goldNum;
            }
            else
            {
                profile.CoinFilpsWon++;
                checked { profile.GoldWonCoinFlip += goldNum; }
            }


            checked { profile.Gold += goldNum; }
            checked { botProfile.Gold -= goldNum; }

            context.Profiles.Update(profile);
            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ProccessMemberCoinFlip(Profile profile, Profile botProfile, int goldNum, bool memberWonGame)
        {
            using var context = new RPGContext(_options);

            if (!memberWonGame)
            {
                profile.CoindFlipsLost++;
                profile.GoldLostCoinFlip += goldNum;

                //Make sure set to negative after you track the results not before.
                goldNum = -goldNum;
            }
            else
            {
                profile.CoinFilpsWon++;
                profile.GoldWonCoinFlip += goldNum;
            }


            profile.Gold += goldNum;
            botProfile.Gold -= goldNum;

            context.Profiles.Update(profile);
            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}