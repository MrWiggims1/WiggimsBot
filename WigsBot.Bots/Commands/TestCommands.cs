using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeCantSpell.Hunspell;
using WigsBot.Bot.Attributes;
using WigsBot.Bot.Handlers.Dialogue;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Models;
using WigsBot.DAL;
//using WigsBot.DAL.Models.Items;

namespace WigsBot.Bot.Commands
{
    [RequireOwner]
    [Hidden]
    public class TestCommands : BaseCommandModule
    {
        private readonly RPGContext _context;

        public TestCommands(RPGContext context)
        {
            _context = context;
        }

        [Command("categoryping")]
        [RequirePrefixes("w!", "W!")]
        [Description("Test to see if you can limit commands to categories.")]
        [RequireCatergories(ChannelCheckMode.any, "category specific commands")]
        public async Task categoryping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("pong").ConfigureAwait(false);
        }

        [Command("dialogue")]
        [RequirePrefixes("w!", "W!")]
        [Description("tells you to say yes or no (More of a proof of concept thing not too useful yet).")]
        public async Task dialogue(CommandContext ctx)
        {
            var inputStep = new TextStep("Say yes or no.", null);
            var yesStep = new TextStep("Noice", null);

            string input = string.Empty;

            inputStep.onValidResult += (result) =>
            {
                input = result;

                if (result == "yes")
                {
                    inputStep.SetNextStep(yesStep);
                }
            };

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                inputStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);
        }

        [Command("respondmessage")]
        [RequirePrefixes("w!", "W!")]
        [Description("Just repeats a message you send within the next minute.")]
        public async Task Respondmessage(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("Respondreaction")]
        [RequirePrefixes("w!", "W!")]
        [Description("Resends an emoji when you react to the command after sending it. (proof of concept not useful unless you want the bot to send an emoji)")]
        public async Task Respondreaction(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Emoji);
        }

        [Command("SpellCheck")]
        [RequirePrefixes("w!", "W!")]
        [Description("spell checks what ever you send.")]
        public async Task SpellCheck(CommandContext ctx, [Description("Sentence to spell check")] params string[] message)
        {
            ctx.Client.GetInteractivity();

            int errorCount = 0;
            int correctCount = 0;
            StringBuilder sb = new StringBuilder();

            foreach (string word in message)
            {
                var dictionary = WordList.CreateFromFiles(@"Resources/en_au.dic");
                bool notOk = dictionary.Check(word);

                if (notOk == false)
                {
                    errorCount += 1;
                    var suggestion = dictionary.Suggest(word).First().ToString();
                    sb.Append($" `{suggestion}`");
                }
                else
                {
                    correctCount += 1;
                    sb.Append($" {word}");
                }
            }

            await ctx.Channel.SendMessageAsync($"correct: {correctCount}\nIncorrect: {errorCount}\nSuggested spelling: {sb.ToString()}").ConfigureAwait(false);
        }

        [Command("AddLineToFile")]
        [RequirePrefixes("w!", "W!")]
        public async Task AddLineToFile(CommandContext ctx, string line)
        {
            string filePath = "Logs/test.log";
            await File.AppendAllTextAsync(filePath, $"\n{line}");
        }
    }
}
