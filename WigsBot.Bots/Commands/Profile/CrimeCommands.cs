using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using System.Linq;
using System;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.Models;
using WigsBot.Core.Services.Items;
using System.Collections.Generic;
using System.Text;

namespace WigsBot.Bot.Commands.Profilecommands
{
    public class CrimeCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IGoldService _goldService;
        private readonly IRobbingItemService _robbingItemService;

        public CrimeCommands(
            IProfileService profileService,
            IGoldService goldService,
            IRobbingItemService robbingItemService
            )
        {
            _profileService = profileService;
            _goldService = goldService;
            _robbingItemService = robbingItemService;
        }

        private decimal defualCarryPercent = .5M;
        private readonly int maxchance = 80;
        private decimal chanceMult = 1.1M;

        [Command("rob")]
        [RequirePrefixes("w!", "W!")]
        [Description("Try your luck and attempt to steal someones gold, don't get too greedy or it may cost you. Defualts to max avaliable.\n\n`Cooldown`: 90 Minutes")]
        [Aliases("StealGold", "TakeGold")]
        [Cooldown(1, 9000, CooldownBucketType.User)]
        public async Task rob(CommandContext ctx, [Description("Discord user")] DiscordMember member, [Description("How much do you want to steal")] int goldNum = 97153)
        {
            await ctx.TriggerTypingAsync();

            //###### checks before performing the heist #####
            if (ctx.Message.Author.Id == member.Id) 
            {
                throw new Exception("You cannot steal gold from yourself."); 
            }

            if (goldNum < 0)
            {
                throw new Exception("If you want to pay someone, try using `w!pay <member> <gold>`.");
            }

            if (member.IsBot)
            {
                throw new Exception("You cannot rob bots, not yet at least.");
            }

            if (!ctx.Channel.Users.Contains(member))
            {
                throw new Exception("Yeah no. Not letting you rob someone in a channel they cant even access. Not cool.");
            }

            // set variables
            Profile vicProfile = await _profileService.GetOrCreateProfileAsync(member.Id, ctx.Guild.Id).ConfigureAwait(false);
            Profile attackProfile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            StringBuilder extras = new StringBuilder();

            int ranResult = new Random().Next(0, 100);
            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, "Wiggims Bot: Robbing command debug info.", $"{ctx.Member.Username} is attempting to rob {member.Username}", DateTime.Now);

            if (vicProfile.Gold < 20)
            {
                await ctx.Channel.SendMessageAsync("To rob another user, they must have at least 20 gold.");
                ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: Robbing command info.", $"{member.Username} did not have any gold to rob.", DateTime.Now);
                return;
            }

            decimal carryBonus = _robbingItemService.GetCarryAmount(attackProfile);
            decimal maxCarryPercent = defualCarryPercent + carryBonus;
            var vicBuffs = _robbingItemService.GetBuffStats(vicProfile);
            var attackBuffs = _robbingItemService.GetBuffStats(attackProfile);

            var defStrenthAtvantage = vicBuffs.defense / attackBuffs.attack;

            int attackErn = Math.Min(Convert.ToInt32(vicProfile.Gold * maxCarryPercent), goldNum);
            int vicsGoldRatio = Convert.ToInt32(attackErn / Convert.ToDecimal(vicProfile.Gold) * 100M);
            int fine = Convert.ToInt32(attackErn * 0.3M);

            var chance = 100 - Math.Min(vicsGoldRatio * chanceMult * defStrenthAtvantage, maxchance);

            ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: Robbing command odds info.", $"chance = {chance}\tRanresult = {ranResult}", DateTime.Now);
            ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: Robbing command modifier info.", $"Defenders advantage = {defStrenthAtvantage * 100 - 100}%\tChance Modifier = {chanceMult}\tMax chance of failure = {maxchance}", DateTime.Now);
            ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: Robbing command money info.", $"Carry bonus = {carryBonus}\tEntered rob amount = {goldNum}\tEarnings = {attackErn}\t% of vics gold = {vicsGoldRatio}\tFine if failed = {fine}", DateTime.Now);

            //generate extras to inclue within embed
            if (goldNum > vicProfile.Gold * maxCarryPercent)
            {
                if (goldNum == 97153)
                    extras.Append($"You did not enter a value for goldNum, automattically setting it to maximumavaliable.\n");
                else
                    extras.Append($"The maximum you can steal from another member is {maxCarryPercent * 100}%, if you want to steal more visit the item shop.\n");
            }

            if (defStrenthAtvantage > 1)
                extras.Append($"The victim had a higher defense than you had attack, visit the item shop and buy some upgrades.\n");

            if (chance < 40)
                extras.Append($"You had a low chance of success, maybe next time try to steal less gold at a time.");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Member.Username} is robbing {member.Username}",
                Timestamp = DateTime.Now
            };

            // process the rob
            DiscordEmbedBuilder responseEmbed;

            if (chance > ranResult)
            {
                responseEmbed = GenerateEmbed(embed, $"The robbery of {member.Mention}'s vault was successful and {ctx.Member.Mention} came out {attackErn} gold richer.", true, extras);
                await _goldService.TransferGold(vicProfile, attackProfile, attackErn, false).ConfigureAwait(false);
            }
            else
            {
                responseEmbed = GenerateEmbed(embed, $"{ctx.Member.Mention} you were caught trying to rob {member.Username} by the police with {attackErn} gold packed away in their gold bags, you have been fined {fine} gold which goes towards {member.Username}.", false, extras);
                await _goldService.TransferGold(attackProfile, vicProfile, fine, true).ConfigureAwait(false);
            }

            await ctx.RespondAsync(embed: responseEmbed).ConfigureAwait(false);

            await ctx.Message.DeleteAsync();
        }

        [Command("analyzevaults")]
        [RequirePrefixes("w!", "W!")]
        [RequireOwner]
        [Hidden]
        [Description(" ")]
        public async Task analyzevaults(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Really sorry, havent finnished this yet");
        }


        // Tasks 

         static DiscordEmbedBuilder GenerateEmbed(DiscordEmbedBuilder embed, string message, bool robSucceed, StringBuilder extraInfoToAdd = null)
        {
            if (!robSucceed)
                embed.WithColor(DiscordColor.IndianRed);
            else
                embed.WithColor(DiscordColor.Gold);

            if (!String.IsNullOrEmpty(extraInfoToAdd.ToString()))
            {
                embed.WithFooter(extraInfoToAdd.ToString());
            }

            embed.WithDescription(message);

            return embed;
        }

        public async Task Threaten(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("500 gold has been removed from your account.");

            var interatcivity = ctx.Client.GetInteractivity();

            var msg = await interatcivity.WaitForMessageAsync(x => x.Author == ctx.User && x.Channel == ctx.Channel).ConfigureAwait(false);

            if (msg.TimedOut)
            { return; }
            
            await ctx.Channel.SendMessageAsync("At least it will be if you try that shit again.");
        }

        public async Task showOddsOnReact(CommandContext ctx, DiscordMessage msg, string message)
        {
            try
            {
                await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":gear:"));

                var interactivity = ctx.Client.GetInteractivity();

                var reaction = await interactivity.WaitForReactionAsync(
                    x => x.Message == msg &&
                    x.User.Id == 318999364192174080 &&
                    x.Emoji == DiscordEmoji.FromName(ctx.Client, ":gear:"));

                if (!reaction.TimedOut)
                {
                    DiscordDmChannel dmChannel = ctx.Guild.GetMemberAsync(318999364192174080).Result.CreateDmChannelAsync().Result;
                    await dmChannel.SendMessageAsync(message);
                }
            }
            catch { await msg.DeleteAllReactionsAsync().ConfigureAwait(false);  }
        }

        public async Task<bool> ProccessLargeRobbing(CommandContext ctx, DiscordMember vic)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var result = await interactivity.WaitForMessageAsync(x => x.Author.Id == vic.Id && x.Channel == ctx.Channel).ConfigureAwait(false);

            if (result.TimedOut)
                return false;
            else
                return true;
        }
    }
}
    
