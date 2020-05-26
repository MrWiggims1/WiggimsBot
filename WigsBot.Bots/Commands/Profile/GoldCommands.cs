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

namespace WigsBot.Bot.Commands.Profilecommands
{
    public class GoldCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IGotService _gotService;
        private readonly IGoldService _goldService;

        public GoldCommands(
            IProfileService profileService,
            IGotService gotService,
            IGoldService goldService
            )
        {
            _profileService = profileService;
            _gotService = gotService;
            _goldService = goldService;
        }

        [Command("daily")]
        [RequirePrefixes("w!", "W!")]
        [Description("Get gold every 24hrs. Or give it to another user")]
        [Cooldown(1, 86400, CooldownBucketType.User)]
        [Aliases("dole")]
        public async Task Daily(CommandContext ctx)
        {
            await GiveDaily(ctx, ctx.Member).ConfigureAwait(false);
        }

        [Command("daily")]
        [RequirePrefixes("w!", "W!")]
        public async Task Daily(CommandContext ctx, DiscordMember member)
        {
            await GiveDaily(ctx, member).ConfigureAwait(false);
        }

        [Command("AddGold")]
        [RequirePrefixes("w@", "W@")]
        [Description("Adds a gold to a user, Dadmin only.")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Aliases("GrantGold", "spawngold")]
        public async Task AddGold(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("amount of gold to add or take away (negative to take).")] int goldNum)
        {
            await _goldService.GrantGoldAsync(member.Id, ctx.Guild.Id, goldNum);
            if (goldNum < 0)
            {
                await ctx.Channel.SendMessageAsync($"Through the power of dadmin abooz {member.Username} lost {- goldNum} gold.").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Through the power of dadmin abooz {member.Username} gained {goldNum} gold.").ConfigureAwait(false);
            }
        }

        [Command("pay")]
        [RequirePrefixes("w!", "W!")]
        [Description("Transfer gold from your account to someone else.")]
        [Aliases("payGold", "transfergold", "givegold")]
        public async Task pay(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("How much you want to pay?")] int goldNum)
        {
            if (ctx.Message.Author.Id == member.Id) { await ctx.Channel.SendMessageAsync("God you're a fucking nugget."); return; }

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            if (goldNum < 0) { await ctx.Channel.SendMessageAsync("https://tenor.com/view/wait-thats-illegal-halo-meme-gif-14048618"); return; }

            if (goldNum > profile.Gold) { await ctx.Channel.SendMessageAsync($"You can't afford to give { member.Username } {goldNum} gold").ConfigureAwait(false); return; }

            Profile payeeProfile = await _profileService.GetOrCreateProfileAsync(member.Id, ctx.Guild.Id);

            await _goldService.TransferGold(profile, payeeProfile, goldNum, false).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"You successfully paid {member.Username} {goldNum} gold.");
        }

        [Command("Fish")]
        [RequirePrefixes("w!", "W!")]
        [Description("Cast a rod to try to catch a fish. This is a temporary command, i am half way through a much more fleshed out version.")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        public async Task Fish(CommandContext ctx) 
        {
            var chance = new Random().Next(0, 10);
            string reply;
            int castCost = 3;
            int castEarn;

            int commonFish = 5;
            int rareFish = 10;
            int superRareFish = 20;

            ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: fish command", $"Random value was {chance}", DateTime.Now, null);

            switch (chance)
            {
                case 0:
                    castEarn = 0;
                    reply = "You caught nothing";
                    break;

                case 1:
                    castEarn = 0;
                    reply = "You caught nothing";
                    break;

                case 2:
                    castEarn = 0;
                    reply = "You caught nothing";
                    break;

                case 3:
                    castEarn = commonFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 4:
                    castEarn = commonFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 5:
                    castEarn = commonFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 6:
                    castEarn = commonFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 7:
                    castEarn = commonFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 8:
                    castEarn = rareFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 9:
                    castEarn = rareFish;
                    reply = $"You caught a common fish worth {castEarn} gold";
                    break;

                case 10:
                    castEarn = superRareFish;
                    reply = $"You caught a super rare fish worth {castEarn} gold";
                    break;

                default:
                    throw new Exception($"{chance} was not in the range.");
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"{ctx.Member.Username} went fishing",
                Description = reply + $", You spent {castCost} to cast the rod",
                Color = ctx.Member.Color
            };

            embed.WithFooter("This crappy command will not stay like this for long.");

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);
            Profile botProfile = await _profileService.GetOrCreateProfileAsync(ctx.Client.CurrentUser.Id, ctx.Guild.Id);

            await _goldService.TransferGold(botProfile, profile, castEarn - castCost, false);

            await ctx.RespondAsync(embed: embed);
        }

        //########### Tasks Below ################

        private async Task GrantGot(CommandContext ctx, int gotNum, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            GrantGotViewModel viewModel = await _gotService.GrantGotAsync(memberId, ctx.Guild.Id, gotNum).ConfigureAwait(false);

            if (profile.QuietMode == true) { return; }
            if (!viewModel.GotLevelledUp) { return; }

            var levelUpEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName} has been dun got gitten {viewModel.Profile.Gots} times now... Dang.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color
            };

            levelUpEmbed.WithFooter("w!profile togglenotifications to hide In future.");

            await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
        }  

        public async Task GiveDaily(CommandContext ctx, DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            Profile profile = await _profileService.GetOrCreateProfileAsync(member.Id, ctx.Guild.Id);
            Profile botProfile = await _profileService.GetOrCreateProfileAsync(ctx.Client.CurrentUser.Id, ctx.Guild.Id);
            var rnd = new Random();
            decimal luck = rnd.Next(10, 30);
            decimal boost = 1.6M;
            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":moneybag:");
            DiscordRole boostRole;
            try
            {
                boostRole = ctx.Guild.Roles.Where(x => x.Value.Name == "Nitro Booster").First().Value;
                if (member.Roles.Contains(boostRole))
                {
                    luck *= boost;
                    emoji = DiscordEmoji.FromName(ctx.Client, ":nitro:");
                }
            }
            catch { }

            if (profile.Gold > 0)
            {
                await ctx.Channel.SendMessageAsync($"{emoji} {member.DisplayName} gained {Convert.ToInt32(luck)} gold!").ConfigureAwait(false);
            }
            else if (profile.Gold <= 0 && profile.Gold > -200)
            {
                luck *= 1.5M;
                await ctx.Channel.SendMessageAsync($"{emoji} {emoji} {emoji} {member.DisplayName} gained {Convert.ToInt32(luck)} gold!").ConfigureAwait(false);
            }
            else if (profile.Gold < -200)
            {
                luck *= 2M;
                await ctx.Channel.SendMessageAsync($"{emoji} {emoji} {emoji} {emoji} {emoji} {member.DisplayName} gained {Convert.ToInt32(luck)} gold!").ConfigureAwait(false);
            }

            int payment = Convert.ToInt32(luck);
            await _goldService.TransferGold(botProfile, profile, payment, true);
        }
    }
}
    
