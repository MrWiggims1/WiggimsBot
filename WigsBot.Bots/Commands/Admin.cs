using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeCantSpell.Hunspell;
using WigsBot.Bot.Handlers.Dialogue;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.Core.Services.GuildPreferencesServices;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.GuildPreferences;
using WigsBot.DAL.Models.Profiles;
using static WigsBot.Core.Services.GuildPreferencesServices.AssignableRoleService;

namespace WigsBot.Bot.Commands
{
    public class adminCommands : BaseCommandModule
    {
        private readonly IGuildPreferences _guildPreferences;
        private readonly IAssignableRoleService _assignableRoleService;
        private readonly IGoldService _goldService;
        private readonly IProfileService _profileService;

        public adminCommands(
            IGuildPreferences guildPreferences,
            IAssignableRoleService assignableRoleService,
            IGoldService goldService,
            IProfileService profileService
            )
        {
            _guildPreferences = guildPreferences;
            _assignableRoleService = assignableRoleService;
            _goldService = goldService;
            _profileService = profileService;
        }

        [Command("ban")]
        [RequirePrefixes("w@", "W@")]
        [Description("Bans a member.\n\n`Example`: w!ban @ Lashall#8741")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, [Description("Mention or use ID.")] DiscordMember banneduser)
        {
            await ctx.Guild.BanMemberAsync(banneduser).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"{banneduser.Username} has been shall never return again.").ConfigureAwait(false);
        }

        [Command("unban")]
        [RequirePrefixes("w@", "W@")]
        [Description("unbans a member.\n\n`Example`: w!unban @ Lashall#8741")]
        [RequireUserPermissions(Permissions.BanMembers)]
        public async Task unban(CommandContext ctx, [Description("Mention or use ID.")] DiscordMember unbanneduser)
        {
            await ctx.Guild.UnbanMemberAsync(unbanneduser).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"{unbanneduser.Username} shall return again.").ConfigureAwait(false);
        }

        [Command("MemberInfo")]
        [RequirePrefixes("w@", "W@")]
        [Description("Shows basic information about a specified Member.\n\n`Example`: w!memberinfo @ mr_wiggims1#3333")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Aliases("userinfo", "infoUser")]
        public async Task MemberInfo(CommandContext ctx, [Description("Mention or use ID.")] DiscordMember Member)
        {
            var MemberRoles = Member.Guild.Roles.Values;

            StringBuilder sb = new StringBuilder();

            foreach (var role in MemberRoles)
            {
                sb.Append($"{role.Name}\n");
            }

            // Create embed with members; username, ImageUrl and in the future possibly roles
            var embed = new DiscordEmbedBuilder
            {
                Title = $"User Name : {Member.Username}",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = Member.AvatarUrl },
                Color = Member.Color,
                Timestamp = System.DateTime.Now
            };

            // Also add fields to show; Display name, Id, is bot? and joined date
            embed.AddField("Display name: ", $"{Member.DisplayName}#{Member.Discriminator}");
            embed.AddField("Id: ", Member.Id.ToString());
            embed.AddField("Bot: ", Member.IsBot.ToString());
            embed.AddField("Joined on: ", Member.JoinedAt.LocalDateTime.ToString());
            embed.AddField("Members roles:", sb.ToString());

            // Send embed
            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("GetDicLog")]
        [RequirePrefixes("w@", "W@")]
        [Description("gets the dictionary change log.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task GetDicLog(CommandContext ctx)
        {
            await ctx.Channel.SendFileAsync("Logs/dictionary.log").ConfigureAwait(false);
        }

        [Command("getspellmistakes")]
        [RequirePrefixes("w@", "W@")]
        [Description("Gets a list of all misspelled words made.")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task getspellmistakes(CommandContext ctx)
        {
            await ctx.Channel.SendFileAsync("Resources/missspelledwords.dic").ConfigureAwait(false);
        }

        [Command("sudo")]
        [Description("Use a command on a users behalf.")]
        [RequireOwner]
        public async Task sudo(CommandContext ctx, DiscordUser user, [RemainingText, Description("Command text to execute.")] string command)
        {
            var cmd = ctx.CommandsNext.FindCommand(command, out var args);
            if (cmd == null)
                throw new CommandNotFoundException(command);

            var fctx = ctx.CommandsNext.CreateFakeContext(user, ctx.Channel, command, ctx.Prefix, cmd, args);
            await ctx.CommandsNext.ExecuteCommandAsync(fctx);
        }

        [Command("shutdown")]
        [RequirePrefixes("w@", "W@")]
        [Description("Shutdown wiggims bot.")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Shutdown(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"<@318999364192174080>, {ctx.Member.Username} has shutdown the bot.");
            Process.GetCurrentProcess().Kill();
        }

        [Command("Resetgold")]
        [RequirePrefixes("w@", "W@")]
        [Description("Resets a users gold based on their level and the items in their inventory.\n\n`Example`: @ Mr_Wiggims#3333")]
        [RequireOwner]
        public async Task resetgold(CommandContext ctx, DiscordMember member)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(member.Id, ctx.Guild.Id).ConfigureAwait(false);

            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            int goldperlevelup = configJson.goldperlevelup;

            var prevGold = profile.Gold;

            int newGold = await _goldService.ResetGold(member.Id, ctx.Guild.Id, goldperlevelup).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"{member.Username}'s gold was changed from {prevGold} to {newGold}.").ConfigureAwait(false);
        }

        [Command("fixusernames")]
        [RequirePrefixes("w@", "W@")]
        [Description("Fixes all the usernames in the database.")]
        [RequireOwner]
        public async Task FixUsernames(CommandContext ctx)
        {
            var members = ctx.Guild.Members;

            Dictionary<ulong, string> memberDictionary = new Dictionary<ulong, string>();

            foreach (var member in members.Values)
            {
                memberDictionary.Add(member.Id, $"{member.Username}#{member.Discriminator}"); 
            }

            await _profileService.FixUserNames(ctx.Guild.Id, memberDictionary);
        }

        [Command("Resetallgold")]
        [RequirePrefixes("w@", "W@")]
        [Description("Resets gold for all members within server based on their level and the items in their inventory.")]
        [RequireOwner]
        public async Task Resetallgold(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var question = await ctx.Channel.SendMessageAsync("Are you sure you want to reset every members gold? Members will have their gold set according to their level and item inventory and cannot be undone.\n\n`yes` or `no`");

            try
            {
                var message = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id);

                if (!message.TimedOut)
                {
                    if (message.Result.Content.ToLower() == "yes")
                    {
                        var json = string.Empty;

                        using (var fs = File.OpenRead("config.json"))
                        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                            json = sr.ReadToEnd();

                        var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
                        int goldperlevelup = configJson.goldperlevelup;

                        await _goldService.ResetAllGold(ctx.Guild.Id, goldperlevelup);

                        await message.Result.DeleteAsync();

                        await question.ModifyAsync("Done!");
                    }

                    else
                        await question.DeleteAsync();
                }
            }
            catch { }
        }   
    }
}