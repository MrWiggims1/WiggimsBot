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

namespace WigsBot.Bot.Commands
{
    [Group("Version")]
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
        public async Task ShowVersionInfo(CommandContext ctx, string versionNumber)
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
        [Description("Creates a new version.")]
        public async Task CreateNewVersion(CommandContext ctx, string versionNumber, string versionName)
        {
            _versionService.CreateNewVersionJson(versionNumber, versionName);

            await ctx.RespondAsync($"{versionNumber} - {versionName}. has been created.").ConfigureAwait(false);
        }
    }
}