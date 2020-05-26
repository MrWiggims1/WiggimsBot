using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus;
using Newtonsoft.Json;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.Core.Services.GuildPreferencesServices;
using WigsBot.DAL.Models.GuildPreferences;
using static WigsBot.Core.Services.GuildPreferencesServices.AssignableRoleService;

namespace WigsBot.Bot.Commands
{
    public class ManagementCommands : BaseCommandModule
    {
        [Group("role")]
        [Description("Commands for leaving and joining roles.")]
        public class roles : BaseCommandModule
        {
            private readonly IGuildPreferences _guildPreferences;
            private readonly IAssignableRoleService _assignableRoleService;

            public roles(
            IGuildPreferences guildPreferences,
            IAssignableRoleService assignableRoleService
            )
            {
                _guildPreferences = guildPreferences;
                _assignableRoleService = assignableRoleService;
            }

            [Command("leave")]
            [RequirePrefixes("w!", "W!")]
            [Description("Provides a list of roles you can leave by selecting emojis.")]
            public async Task leave(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                AssignableRoleJson json = await GetRoleJson(ctx);

                DiscordEmoji doneEmoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");

                if (json.Roles.Count() == 0)
                {
                    await ctx.Channel.SendMessageAsync("There are no roles in the assignable roles list. Please ask your administrator to add roles, using the `w!admin addAssignableRole [@DiscordRole] [Emoji]`.");
                    return;
                }

                TimeSpan timeSpan = TimeSpan.FromSeconds(60);

                var joinEmbed = new DiscordEmbedBuilder
                {
                    Title = "Would you like to stop receiving pings for specific games?",
                    ThumbnailUrl = ctx.Member.AvatarUrl,
                    Color = ctx.Member.Color,
                    Description = $"Click the icons for the games that you do not wish to play anymore (only after all emojis have appeared).\nAfter 60 seconds your role changes will be applied.\n\nThis command does not allow you to join the role, if you wish to join a role please use the `w!role join` command."
                };

                StringBuilder sb = new StringBuilder();

                foreach (Roles role in json.Roles)
                {
                    try
                    {
                        DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                        string roleName = ctx.Guild.GetRole(role.RoleId).Name;

                        sb.Append(emoji + $" : {roleName}\n");
                    }
                    catch (Exception e)
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Guild.GetRole(role.RoleId).Name + "Could not be processed");
                        Console.WriteLine(e.Message);
                    }
                }

                joinEmbed.AddField("Roles:", sb.ToString());

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);
                var userid = ctx.Member.Id;

                foreach (var role in json.Roles)
                {
                    DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                    await joinMessage.CreateReactionAsync(emoji);
                }

                //await joinMessage.CreateReactionAsync(doneEmoji);

                var interactivity = ctx.Client.GetInteractivity();

                var result = await interactivity.CollectReactionsAsync(joinMessage, timeSpan);
                var distinctResult = result.Distinct().Where(x => x.Users.First() == ctx.User);

                StringBuilder sb2 = new StringBuilder();

                foreach (var selection in distinctResult)
                {
                    ulong reactionId = selection.Emoji.Id;

                    try
                    {
                        var index = json.Roles.FindIndex(x => x.EmojiId == reactionId);
                        await ctx.Member.RevokeRoleAsync(ctx.Guild.GetRole(json.Roles[index].RoleId));
                        sb2.Append($"\"{ctx.Guild.GetRole(json.Roles[index].RoleId).Name}\"\n");
                    }
                    catch { }
                }

                await joinMessage.DeleteAsync();

                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} has left the following roles:\n```json\n{sb2}\n```");
            }

            [Command("join")]
            [RequirePrefixes("w!", "W!")]
            [Description("Provides a list of roles you can join by selecting emojis.")]
            public async Task join(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                AssignableRoleJson json = await GetRoleJson(ctx);

                DiscordEmoji doneEmoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");

                if (json.Roles.Count() == 0)
                {
                    await ctx.Channel.SendMessageAsync("There are no roles in the assignable roles list. Please ask your administrator to add roles, using the `w!admin addAssignableRole [@DiscordRole] [Emoji]`.");
                    return;
                }

                TimeSpan timeSpan = TimeSpan.FromSeconds(60);

                var joinEmbed = new DiscordEmbedBuilder
                {
                    Title = "Would you like to receiving pings for specific games?",
                    ThumbnailUrl = ctx.Member.AvatarUrl,
                    Color = ctx.Member.Color,
                    Description = $"Click the icons for the games that you wish to play (only after all emojis have appeared).\nAfter 60 seconds your role changes will be applied.\n\nThis command does not allow you to leave the role, if you wish to join a role please use the `w!role leave` command."
                };

                StringBuilder sb = new StringBuilder();

                foreach (Roles role in json.Roles)
                {
                    try
                    {
                        DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                        string roleName = ctx.Guild.GetRole(role.RoleId).Name;

                        sb.Append(emoji + $" : {roleName}\n");
                    }
                    catch (Exception e)
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Guild.GetRole(role.RoleId).Name + "Could not be processed");
                        Console.WriteLine(e.Message);
                    }
                }

                joinEmbed.AddField("Roles:", sb.ToString());

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);
                var userid = ctx.Member.Id;

                foreach (var role in json.Roles)
                {
                    DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                    await joinMessage.CreateReactionAsync(emoji);
                }

                //await joinMessage.CreateReactionAsync(doneEmoji);

                var interactivity = ctx.Client.GetInteractivity();

                var result = await interactivity.CollectReactionsAsync(joinMessage, timeSpan);
                var distinctResult = result.Distinct().Where(x => x.Users.First() == ctx.User);

                StringBuilder sb2 = new StringBuilder();

                foreach (var selection in distinctResult)
                {
                    ulong reactionId = selection.Emoji.Id;

                    try
                    {
                        var index = json.Roles.FindIndex(x => x.EmojiId == reactionId);
                        await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(json.Roles[index].RoleId));
                        sb2.Append($"\"{ctx.Guild.GetRole(json.Roles[index].RoleId).Name}\"\n");
                    }
                    catch { }
                }

                await joinMessage.DeleteAsync();

                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} has joined the following roles:\n```json\n{sb2}\n```");
            }
            
            //######## Role Tasks #########
            public async Task<AssignableRoleJson> GetRoleJson(CommandContext ctx)
            {
                GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                AssignableRoleJson json = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

                return json;
            }

        }

        [Group("Guild")]
        [Description("Shows current wiggims bot settings for the guild.")]
        [Aliases("Guildsettings")]
        [RequireUserPermissions(Permissions.Administrator)]
        public class Guild : BaseCommandModule
        {
            private readonly IGuildPreferences _guildPreferences;
            private readonly IAssignableRoleService _assignableRoleService;
            private readonly ISpellingSettingsService _spellingSettingsService;

            public Guild(
            IGuildPreferences guildPreferences,
            IAssignableRoleService assignableRoleService,
            ISpellingSettingsService spellingSettingsService
            )
            {
                _guildPreferences = guildPreferences;
                _assignableRoleService = assignableRoleService;
                _spellingSettingsService = spellingSettingsService;
            }

            [GroupCommand]
            public async Task guildInfo(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                AssignableRoleJson json = await GetRoleJson(ctx);
                GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                StringBuilder sb = new StringBuilder();

                foreach (Roles role in json.Roles)
                {
                    DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                    string roleName = ctx.Guild.GetRole(role.RoleId).Name;

                    sb.Append(emoji + $" : {roleName}\n");
                }

                var infoEmbed = new DiscordEmbedBuilder
                {
                    Title = $"Wiggims Bot Settings for {ctx.Guild.Name}",
                    Description = "To edit any of the following settings, type `w!help guild` to see the options available.",
                    //ThumbnailUrl = ctx.Guild.IconUrl,
                    Color = DiscordColor.Orange
                };

                infoEmbed.AddField("Basic settings:",
                    $"Spelling tracking: enabled = {guildPreferences.SpellingEnabled}. list length {guildPreferences.ErrorListLength}\n" +
                    $"Xp earned per message: {guildPreferences.XpPerMessage} (For 10 messages within 60 seconds)\n" +
                    $"Auto Role: Not Implemented yet\n");

                infoEmbed.AddField("Admin Settings:",
                    $"Admin Role: Not Implemented yet\n" +
                    $"Guild Event Notifications: Not Implemented yet\n" +
                    $"Admin Notification Channel: Not Implemented yet\n" +
                    $"Punish @ every ones: Not Implemented yet\n");

                infoEmbed.AddField("Roles available through `w!role join` and `w!role leave`:", sb.ToString());

                await ctx.Channel.SendMessageAsync("",  embed: infoEmbed);
            }

            [Command("addrole")]
            [RequirePrefixes("w@", "W@")]
            [Aliases("addAssignableRole")]
            [Description("Adds a role that all users can assign themselves using w!role join")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task addAssignableRole(CommandContext ctx, [Description("Discord Role to add.")] DiscordRole discordRole, [Description("Discord Emoji to assign to role.")]DiscordEmoji emoji)
            {
                await ctx.TriggerTypingAsync();

                AssignableRoleJson json = await GetRoleJson(ctx);

                try
                {
                    DiscordEmoji testEmoji = DiscordEmoji.FromGuildEmote(ctx.Client, emoji.Id);
                }
                catch
                {
                    await ctx.Channel.SendMessageAsync("You can only use server emojis from a guild where wiggims bot is connected for this command, no default emojis either.");
                    return;
                }

                foreach (var role in json.Roles)
                {
                    if (role.RoleId == discordRole.Id || role.EmojiId == emoji.Id)
                    {
                        await ctx.Channel.SendMessageAsync("Either that role or emoji has already been used, if you wish to remove a role and add it again please user the `w!RemoveAssignableRole [Role] command`");
                        return;
                    }
                }

                await _assignableRoleService.AddRoleToAssignableRoles(ctx.Guild.Id, discordRole.Id, emoji.Id);

                await ctx.Channel.SendMessageAsync($"{discordRole.Name} was added to the assignable roles list! Please not if the role is higher up than the bots role wiggims bot will not be able to assign this role. Mr_Wiggims1 hasn't figured out how to check for this yet.");
            }

            [Command("Removerole")]
            [RequirePrefixes("w@", "W@")]
            [Aliases("RemoveAssignableRole", "rmrole")]
            [Description("removes a role from the assignable roles list.")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task RemoveAssignableRole(CommandContext ctx, [Description("Discord Role to remove.")] DiscordRole discordRole)
            {
                await ctx.TriggerTypingAsync();

                AssignableRoleJson json = await GetRoleJson(ctx);

                try
                {
                    await _assignableRoleService.RemoveRoleFromAssignableRoles(ctx.Guild.Id, discordRole.Id);
                    await ctx.Channel.SendMessageAsync($"{discordRole.Name} was removed from the assignable roles list!");
                }
                catch
                {
                    await ctx.Channel.SendMessageAsync($"There was an issue removing {discordRole.Name} from the assignable roles list.");
                }
            }

            [Command("ToggleSpelling")]
            [RequirePrefixes("w@", "W@")]
            [RequireUserPermissions(Permissions.Administrator)]
            [Description("Toggles on and off spell tracking and bogan measuring.")]
            public async Task togglespelling(CommandContext ctx)
            {
                GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                await _spellingSettingsService.ToggleSpellTracking(ctx.Guild.Id, !guildPreferences.SpellingEnabled);

                await ctx.Channel.SendMessageAsync($"Spell checking enabled: {!guildPreferences.SpellingEnabled}");
            }

            [Command("ToggleSpelling")]
            [RequirePrefixes("w@", "W@")]
            [Description("Toggles on and off spell tracking and bogan measuring.")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task togglespelling(CommandContext ctx, [Description("`True` Or `False`")]bool TrueOrFalse)
            {
                await _spellingSettingsService.ToggleSpellTracking(ctx.Guild.Id, TrueOrFalse);

                await ctx.Channel.SendMessageAsync($"Spell checking enabled: {TrueOrFalse}");
            }

            [Command("SpellErrorList")]
            [RequirePrefixes("w@", "W@")]
            [Description("Sets the length of the error list shown on a users profile (Max 20, 0 to disable)")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task spellerrorlist(CommandContext ctx, [Description("Number of words to store")] int listLength)
            {
                await _spellingSettingsService.SetSpellListLength(ctx.Guild.Id, listLength);

                await ctx.Channel.SendMessageAsync($"Spell error list set to: {listLength}");
            }

            //######## Guild Tasks ############
            public async Task<AssignableRoleJson> GetRoleJson(CommandContext ctx)
            {
                GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                AssignableRoleJson json = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

                return json;
            }
        }

    }
}