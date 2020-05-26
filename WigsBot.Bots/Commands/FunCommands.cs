using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using WeCantSpell.Hunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.Core.ViewModels;
using WigsBot.DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text.RegularExpressions;
using System.Text;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Handlers.Dialogue;

namespace WigsBot.Bot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IExperienceService _experienceService;
        private readonly IGotService _gotService;

        public FunCommands(IProfileService profileService, IExperienceService experienceService, IGotService gotService)
        {
            _profileService = profileService;
            _experienceService = experienceService;
            _gotService = gotService;
        }

        [Command("poll")]
        [RequirePrefixes("w!", "W!")]
        [RequireOwner]
        [Description("Makes a poll using emojis.")]
        public async Task Poll(CommandContext ctx, [Description("How long to run the poll for? S Or M")] TimeSpan duration, [Description("Emojis to use in the poll.")] params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Color = DiscordColor.Orange,
                Description = string.Join("\n ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var results = distinctResult.Select(x => $"{x.Emoji.Name}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", $"{results} Votes.")).ConfigureAwait(false);
        }

        [Command("Magic8Ball")]
        [RequirePrefixes("w!", "W!")]
        [Description("Use a magic 8 ball to answer your question.")]
        [Aliases("8Ball", "m8B", "eightball","magiceightball")]
        public async Task Magic8Ball(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // Responses for 8 ball
            var response0 = "It is certain.";
            var response1 = "Without a doubt.";
            var response2 = "Yes.";
            var response3 = "Ask again later.";
            var response4 = "Cannot predict now.";
            var response5 = "Most likely.";
            var response6 = "Don't count on it.";
            var response7 = "No";
            var response8 = "I don't think so.";
            var response9 = "Im not sure... Im sure about one thing tho. you just got GOT!";

            var rnd = new Random();
            int Outcome = rnd.Next(0, 10);

            if (Outcome == 0) { await ctx.Channel.SendMessageAsync(response0).ConfigureAwait(false); }
            else if (Outcome == 1) { await ctx.Channel.SendMessageAsync(response1).ConfigureAwait(false); }
            else if (Outcome == 2) { await ctx.Channel.SendMessageAsync(response2).ConfigureAwait(false); }
            else if (Outcome == 3) { await ctx.Channel.SendMessageAsync(response3).ConfigureAwait(false); }
            else if (Outcome == 4) { await ctx.Channel.SendMessageAsync(response4).ConfigureAwait(false); }
            else if (Outcome == 5) { await ctx.Channel.SendMessageAsync(response5).ConfigureAwait(false); }
            else if (Outcome == 6) { await ctx.Channel.SendMessageAsync(response6).ConfigureAwait(false); }
            else if (Outcome == 7) { await ctx.Channel.SendMessageAsync(response7).ConfigureAwait(false); }
            else if (Outcome == 8) { await ctx.Channel.SendMessageAsync(response8).ConfigureAwait(false); }
            else { await ctx.Channel.SendMessageAsync(response9).ConfigureAwait(false); await GrantGot(ctx, 1, ctx.Member.Id); };
        }

        [Command("Magic8Ball")]
        [RequirePrefixes("w!", "W!")]
        public async Task Magic8Ball(CommandContext ctx, [Description("Question")] params string[] question)
        {
            await ctx.TriggerTypingAsync();

            // Responses for 8 ball
            var response0 = "It is certain.";
            var response1 = "Without a doubt.";
            var response2 = "Yes.";
            var response3 = "Ask again later.";
            var response4 = "Cannot predict now.";
            var response5 = "Most likely.";
            var response6 = "Don't count on it.";
            var response7 = "No";
            var response8 = "I don't think so.";
            var response9 = "Im not sure... Im sure about one thing tho. you just got GOT!";

            var rnd = new Random();
            int Outcome = rnd.Next(0, 10);

            if (Outcome == 0) { await ctx.Channel.SendMessageAsync(response0).ConfigureAwait(false); }
            else if (Outcome == 1) { await ctx.Channel.SendMessageAsync(response1).ConfigureAwait(false); }
            else if (Outcome == 2) { await ctx.Channel.SendMessageAsync(response2).ConfigureAwait(false); }
            else if (Outcome == 3) { await ctx.Channel.SendMessageAsync(response3).ConfigureAwait(false); }
            else if (Outcome == 4) { await ctx.Channel.SendMessageAsync(response4).ConfigureAwait(false); }
            else if (Outcome == 5) { await ctx.Channel.SendMessageAsync(response5).ConfigureAwait(false); }
            else if (Outcome == 6) { await ctx.Channel.SendMessageAsync(response6).ConfigureAwait(false); }
            else if (Outcome == 7) { await ctx.Channel.SendMessageAsync(response7).ConfigureAwait(false); }
            else if (Outcome == 8) { await ctx.Channel.SendMessageAsync(response8).ConfigureAwait(false); }
            else { await ctx.Channel.SendMessageAsync(response9).ConfigureAwait(false); await GrantGot(ctx, 1, ctx.Member.Id); };
        }

        [Command("ammo")]
        [RequirePrefixes("w!", "W!")]
        [Description("How much ammo you got left in your mag?")]
        public async Task ammo(CommandContext ctx)
        {
            var ammo = new Random().Next(0, 30);
            bool enemy = new Random().NextDouble() > 0.5;

            if (ammo == 0)
            {
                await ctx.Channel.SendMessageAsync($"shit ya got no ammo");
                return;
            }

            else if (ammo == 30)
            {
                await ctx.Channel.SendMessageAsync($"Your good for ammo 30 out of 30");
                return;
            }

            var msg = new TextStep($"You only have {ammo} out of 30, you think someone is around the corner. time for a reload?\nYes or No.", null, null);

            string response = string.Empty;
            msg.onValidResult += (result) => response = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                msg
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (response.ToLower() == "yes")
            {
                if (enemy)
                    await ctx.Channel.SendMessageAsync("You reload as fast as possible, but you get peaked and die to the AWP.");

                if (!enemy)
                    await ctx.Channel.SendMessageAsync("You reload as fast as possible, no one peaks. maybe you scared them off?");
            }

            else if (response.ToLower() == "no")
            {
                if (enemy)
                {
                    if (ammo < 10)
                    {
                        await ctx.Channel.SendMessageAsync($"You stand your ground, with only {ammo} bullets. But you cant aim for shit and die to the bottom frag on a P90.");
                    }
                    else
                        await ctx.Channel.SendMessageAsync($"You stand your ground, with only {ammo} bullets. Plenty to obliterate the T with an Ak.");
                }

                if (!enemy)
                    await ctx.Channel.SendMessageAsync("You stand your ground, but no enemy is around the corner, your safe for now.");
            }

            else
            {
                if (enemy)
                    await ctx.Channel.SendMessageAsync("You didn't say Yes or No. So i assume your still thinking about it, but in the mean time you let the enemy through and your team looses the round.");

                if (!enemy)
                    await ctx.Channel.SendMessageAsync("You didn't say Yes or No. So i assume your still thinking about it, luck no one was there or you probably would have died.");
            }
        }

        [Command("kill")]
        [RequirePrefixes("w!", "W!")]
        [Description("Mention the user you want to kill, Fun for all! Just don't miss.")]
        [Aliases("Murder", "Shoot")]
        public async Task kill( CommandContext ctx, [Description("User you wish to kill")] DiscordUser victim)
        {
            var killer = ctx.Message.Author.Username;
            var victimName = victim.Username;
            var random = new Random();
            int luck = random.Next(0, 10);

            if (killer == victimName)
            {
                if (luck > 5)
                {
                    await ctx.Channel.SendMessageAsync($"Its OK {killer} \n https://tenor.com/view/hug-kangen-peluk-gif-4589810").ConfigureAwait(false);
                    return;
                }
                else { await ctx.Channel.SendMessageAsync("Well done ya fucking idiot \n https://tenor.com/view/homero-simpson-out-doh-gif-15451551").ConfigureAwait(false); };
                return;
            }
            else
            {
                if (luck > 1)
                {
                    await ctx.Channel.SendMessageAsync($"{victimName} runs as fast as they can, trying to evade the bullet from the barrel of {killer}'s gun... But it was too little, too late.").ConfigureAwait(false);
                }
                else { 
                    await ctx.Channel.SendMessageAsync($"{victimName} runs as fast as they can, trying to evade the bullet from the barrel of {killer}'s gun... But {killer} has terrible aim and missed!!! And as punishment a got has been added to {killer}").ConfigureAwait(false);
                    await GrantGot(ctx, 1, ctx.Member.Id);
                };

            }
        }

        [Command("F")]
        [RequirePrefixes("w!", "W!")]
        [Description("Can we get an F in chat bois?")]
        public async Task F(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Can we get a F in chat for {ctx.Member.DisplayName}?").ConfigureAwait(false);
        }

        [Command("F")]
        [RequirePrefixes("w!", "W!")]
        [Description("Can we get an F in chat bois?")]
        public async Task F(CommandContext ctx, [Description("The user who needs the F.")] DiscordUser discorduser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Can we get a F in chat for {discorduser.Username}?").ConfigureAwait(false);
        }

        [Command("giveadmin")]
        [RequirePrefixes("w!", "W!")]
        [Description("Gives the user admin permissions. (defaults to user who calls command)")]
        public async Task giveadmin(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Got 'em.").ConfigureAwait(false);
            await GrantGot(ctx, 1, ctx.Member.Id);
        }

        [Command("giveadmin")]
        [RequirePrefixes("w!", "W!")]
        [Description("Gives the user admin permissions. (defaults to user who calls command)")]
        public async Task giveadmin(CommandContext ctx, [Description("The user to make admin.")] DiscordUser discorduser)
        {
            await ctx.Channel.SendMessageAsync($"Got 'em.").ConfigureAwait(false);
            await GrantGot(ctx, 1, ctx.Member.Id);
        }

        [Command("ScissorsPaperRock")]
        [RequirePrefixes("w!", "W!")]
        [Description("Scissors Paper Rock pretty simple")]
        [Aliases("RockScissorsPaper", "RockPaperScissors", "PaperRockScissors", "PaperScissorsRock", "ScissorsRockPaper","spr")]
        public async Task SissorsPaperRock(CommandContext ctx, [Description("Scissors Paper or Rock")] string answer)
        {
            if (answer.ToLower().Contains("scissors") == false)
            {
                if (answer.ToLower().Contains("rock") == false)
                {
                    if (answer.ToLower().Contains("paper") == false) { await ctx.Channel.SendMessageAsync("Please enter scissors, paper or rock").ConfigureAwait(false); }
                }
            }

            var random = new Random();
            int results = random.Next(1, 4);

            if (results == 1)
            {
                if (answer.ToLower() == "scissors")
                {                    
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Scissors. Tie!").ConfigureAwait(false);                    
                }
                else if (answer.ToLower() == "paper")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Scissors. {ctx.Client.CurrentUser.Username} wins!").ConfigureAwait(false);
                }
                else if (answer.ToLower() == "rock")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Scissors. {ctx.Member.Username} wins!").ConfigureAwait(false);
                }
            }
            else if (results == 2)
            {
                if (answer.ToLower() == "scissors")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Paper. {ctx.Member.Username} wins!").ConfigureAwait(false);
                }
                else if (answer.ToLower() == "paper")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Paper. Tie!").ConfigureAwait(false);
                }
                else if (answer.ToLower() == "rock")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Paper. {ctx.Client.CurrentUser.Username} wins!").ConfigureAwait(false);
                }
            }
            else if (results == 3)
            {
                if (answer.ToLower() == "scissors")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Rock.  {ctx.Client.CurrentUser.Username} wins!").ConfigureAwait(false);
                }
                else if (answer.ToLower() == "paper")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Rock. {ctx.Member.Username} wins!").ConfigureAwait(false);
                }
                else if (answer.ToLower() == "rock")
                {
                    await ctx.Channel.SendMessageAsync($"You guessed {answer}, {ctx.Client.CurrentUser.Username} guessed Rock. Tie!").ConfigureAwait(false);
                }
            } 
            else
            {                
                await ctx.Channel.SendMessageAsync("something went wrong somehow").ConfigureAwait(false);
                return;
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


    }
}