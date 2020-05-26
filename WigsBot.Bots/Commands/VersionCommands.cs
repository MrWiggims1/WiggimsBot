using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using WigsBot.Core.Services;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Handlers.Dialogue;

namespace WigsBot.Bot.Commands
{
    [Group("Version")]
    [RequirePrefixes("w@", "W@")]
    [Description("Create and update patch notes for wiggims bot.")]
    [RequireOwner]
    public class VersionCommands : BaseCommandModule
    {
        private readonly IVersionService _versionService;

        public VersionCommands(
            IVersionService versionService
            )
        {
            _versionService = versionService;
        }

        [GroupCommand]
        public async Task ShowVersionInfo(CommandContext ctx, [Description("The version number eg. 0.3.1")] string versionNumber)
        {
            var json = _versionService.ReadJson(versionNumber);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Version {versionNumber} - {json.Name}",
                Timestamp = json.ReleaseDate.Release,
                Color = DiscordColor.Orange
            };

            if (json.PatchNotes != "empty")
                embed.AddField("Patch notes:", json.PatchNotes, false);

            if (json.MinorNotes != "empty")
                embed.AddField("Minor changes and fixes:", json.MinorNotes);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("create")]
        [RequirePrefixes("w@", "W@")]
        [Description("Creates a new version.")]
        public async Task CreateNewVersion(CommandContext ctx, [Description("The new versions number.")] string versionNumber, [Description("The new versions name."), RemainingText] string versionName)
        {
            _versionService.CreateNewVersionJson(versionNumber, versionName);

            await ctx.RespondAsync($"{versionNumber} - {versionName}. has been created.").ConfigureAwait(false);
        }

        [Command("AddNote")]
        [RequirePrefixes("w@", "W@")]
        [Description("Add a new patch note to a version file.")]
        public async Task AddNoteToVersion(CommandContext ctx, [Description("The version number.")] string versionNumber)
        {
            var noteStep = new TextStep("What is the note you want to add.", null);
            var boolStep = new TextStep("Is this note a minor change or bug fix? True or False.", noteStep);

            bool isMinor = false;
            string newNote = string.Empty;

            boolStep.onValidResult += (result) =>
            {
                isMinor = bool.Parse(result);
            };

            noteStep.onValidResult += (result) =>
            {
                newNote = result;
            };

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                boolStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            _versionService.AddPatchNoteToVersion(versionNumber, $"- newNote", isMinor);

            await ctx.RespondAsync($"New note for version {versionNumber} was added: {newNote}").ConfigureAwait(false);
        }
    }
}