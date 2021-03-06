﻿using DSharpPlus.CommandsNext;
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
using System.Threading;

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
        private decimal chanceMult = 1.2M;

        [Command("rob")]
        [RequirePrefixes("w!", "W!")]
        [Description("Try your luck and attempt to steal someones gold, don't get too greedy or it may cost you. Defaults to max available.\n\n`Cooldown`: 90 Minutes")]
        [Aliases("StealGold", "TakeGold")]
        [Cooldown(1, 1, CooldownBucketType.Guild)]
        public async Task rob(CommandContext ctx, [Description("Discord user")] DiscordMember member, [Description("How much do you want to steal")] int goldNum = 97153)
        {
            await ctx.TriggerTypingAsync();
            Profile vicProfile = await _profileService.GetOrCreateProfileAsync(member.Id, ctx.Guild.Id).ConfigureAwait(false);
            Profile attackProfile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            StringBuilder extras = new StringBuilder();

            //Make sure its been 24 hours since the command has been used.

            System.TimeSpan timeDiff = TimeSpan.FromHours(2).Subtract(DateTime.Now.Subtract(attackProfile.RobbingCooldown));

            if (timeDiff > TimeSpan.FromHours(0))
            {
                DiscordEmbedBuilder CooldownEmbed = new DiscordEmbedBuilder()
                {
                    Title = $"You must wait 2 hours between robberies { timeDiff.Hours }:{ timeDiff.Minutes}:{ timeDiff.Seconds} remaining.",
                    Color = DiscordColor.Red
                };

                CooldownEmbed.WithFooter("Click the clock emoji below to set a reminder for when you can next rob someone.");


                var msg = await ctx.RespondAsync(embed: CooldownEmbed);
                await SendCooldownAlarm(ctx, msg, timeDiff);
                return;
            }

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

            //generate extras to include within embed
            if (goldNum > vicProfile.Gold * maxCarryPercent)
            {
                if (goldNum == 97153)
                    extras.Append($"You did not enter a value for goldNum, automatically setting it to maximum available.\n");
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
                await _goldService.ProccessMembersRobbing(attackProfile, vicProfile, attackErn, true).ConfigureAwait(false);
            }
            else
            {
                responseEmbed = GenerateEmbed(embed, $"{ctx.Member.Mention} you were caught trying to rob {member.Username} by the police with {attackErn} gold packed away in their gold bags, you have been fined {fine} gold which goes towards {member.Username}.", false, extras);
                await _goldService.ProccessMembersRobbing(attackProfile, vicProfile, fine, false).ConfigureAwait(false);
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
            await ctx.Channel.SendMessageAsync("Really sorry, haven't finished this yet");
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

        public async Task SendCooldownAlarm(CommandContext ctx, DiscordMessage msg, TimeSpan timeSpan)
        {
            try
            {
                await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":alarm_clock:"));

                var interactivity = ctx.Client.GetInteractivity();

                var reaction = await interactivity.WaitForReactionAsync(
                    x => x.Message == msg &&
                    x.User.Id == ctx.Member.Id &&
                    x.Emoji == DiscordEmoji.FromName(ctx.Client, ":alarm_clock:"));

                if (!reaction.TimedOut)
                {
                    await msg.DeleteAllReactionsAsync();
                    DiscordDmChannel dmChannel = ctx.Guild.GetMemberAsync(ctx.Member.Id).Result.CreateDmChannelAsync().Result;

                    DiscordEmbedBuilder CooldownEmbed = new DiscordEmbedBuilder()
                    {
                        Title = $"Your timer has been set.",
                        Color = DiscordColor.Red
                    };
                    CooldownEmbed.WithFooter("Keep in mind this timer will only go off if the bot is not reset (which happens fairly often).");

                    await msg.ModifyAsync(embed: CooldownEmbed.Build());

                    System.Threading.Thread.Sleep(Convert.ToInt32(timeSpan.TotalMilliseconds));

                    await dmChannel.SendMessageAsync($"Your cooldown has ended, you can now rob to your hearts content.");
                }
                else
                {
                    await msg.DeleteAllReactionsAsync();
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
    
