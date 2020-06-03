using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.Core.ViewModels;
using WigsBot.DAL.Models.Profiles;

namespace WigsBot.Bot.Commands
{
    //[Group("gamble")]
    //[Aliases("bet", "csgolottery","lottery")]
    public class GambleCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IExperienceService _experienceService;
        private readonly IGotService _gotService;
        private readonly ILeaveService _leaveService;
        private readonly IGoldService _goldService;

        public GambleCommands(IProfileService profileService, IExperienceService experienceService, IGotService gotService, ILeaveService leaveService, IGoldService goldService)
        {
            _profileService = profileService;
            _experienceService = experienceService;
            _gotService = gotService;
            _leaveService = leaveService;
            _goldService = goldService;
        }

        [Command("roulette")]
        [RequirePrefixes("w!", "W!")]
        [Description("Test your luck and try to earn some money.")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task roulette(CommandContext ctx, [Description("Number you want to bet on. 0 - 36")] int guess,[Description("How much gold do you want to bet?")] int bet)
        {
            var random = new Random();
            int Results = random.Next(0, 37);

            if (bet < 1)
            {
                await ctx.Channel.SendMessageAsync("Your a dip shit, you cant bet less than 1 Gold.");
                return;
            }

            if (guess > 36 || 0 > guess)
            {
                await ctx.Channel.SendMessageAsync($"Please read the help message for this command... your lucky I don't just take your gold.").ConfigureAwait(false);
                return;
            }

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);
            Profile botProfile = await _profileService.GetOrCreateProfileAsync(ctx.Client.CurrentUser.Id, ctx.Guild.Id);

            if (bet > profile.Gold) { await ctx.Channel.SendMessageAsync("You cant afford that."); return; }

            if (guess == Results)
            {
                int reward = bet * 35;
                await _goldService.ProccessMemberRoulette(profile, botProfile, reward, true);
                await ctx.Channel.SendMessageAsync($"You won {reward} gold!").ConfigureAwait(false);
            }
            else
            {
                await _goldService.ProccessMemberRoulette(profile, botProfile, bet, false);
                await ctx.Channel.SendMessageAsync($"You lost {bet} gold. ball landed on {Results}, you guessed {guess}").ConfigureAwait(false);
            }
        }

        private async Task GrantGot(CommandContext ctx, int gotNum, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            GrantGotViewModel viewModel = await _gotService.GrantGotAsync(memberId, ctx.Guild.Id, gotNum).ConfigureAwait(false);

            if (!viewModel.GotLevelledUp) { return; }

            var levelUpEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName} has been dun got gitten {viewModel.Profile.Gots} times now... Dang.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color
            };

            await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
        }


        private async Task Grantgold(CommandContext ctx, int GoldNum, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];
            
            await _goldService.GrantGoldAsync(memberId, ctx.Guild.Id, GoldNum).ConfigureAwait(false);
        }
    }
}
