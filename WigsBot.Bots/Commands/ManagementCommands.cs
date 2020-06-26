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
using DSharpPlus.Exceptions;
using WigsBot.Core.Services.GuildPreferencesServices.StatChannels;
using System.Runtime.CompilerServices;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Handlers.Dialogue;
using WigsBot.Bot.Models;
using Microsoft.EntityFrameworkCore;
using WigsBot.DAL;
using WigsBot.Core.Services.Profiles;

namespace WigsBot.Bot.Commands
{
    public class ManagementCommands : BaseCommandModule
    {
        [Group("role")]
        [RequirePrefixes("w!", "W!")]
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
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
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
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
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
        [RequirePrefixes("w@", "W@")]
        [Description("Shows current wiggims bot settings for the guild.")]
        [Aliases("Guildsettings")]
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
            [RequirePrefixes("w@", "W@")]
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

                string timeoutRoleName;
                try { timeoutRoleName = ctx.Guild.GetRole(guildPreferences.TimeoutRoleId).Name; }
                catch { timeoutRoleName = "not set"; }

                var infoEmbed = new DiscordEmbedBuilder
                {
                    Title = $"Wiggims Bot Settings for {ctx.Guild.Name}",
                    Description = "To edit any of the following settings, type `w!help guild` to see the options available.",
                    //Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url =  = ctx.Guild.IconUrl,
                    Color = DiscordColor.Orange
                };

                infoEmbed.AddField("Basic settings:",
                    $"Spelling tracking: enabled = {guildPreferences.SpellingEnabled}. list length {guildPreferences.ErrorListLength}\n" +
                    $"Xp earned per message: {guildPreferences.XpPerMessage} (For 10 messages within 60 seconds)\n" +
                    $"Auto Role: Not Implemented yet\n" +
                    $"Timeout role: {timeoutRoleName}");

                infoEmbed.AddField("Admin Settings:",
                    $"Admin Role: Not Implemented yet\n" +
                    $"Guild Event Notifications: Not Implemented yet\n" +
                    $"Admin Notification Channel: Not Implemented yet\n" +
                    $"Punish @ every ones: Not Implemented yet\n");

                infoEmbed.AddField("Roles available through `w!role join` and `w!role leave`:", sb.ToString());

                await ctx.Channel.SendMessageAsync("", embed: infoEmbed);
            }

            [Group("role")]
            [RequirePrefixes("w@", "W@")]
            [Description("Shows current roles available for members to join using commands, as well as allows for adding and removing roles.")]
            [Aliases("roles")]
            public class GuildRoleCommands : BaseCommandModule
            {
                private readonly IGuildPreferences _guildPreferences;
                private readonly IAssignableRoleService _assignableRoleService;

                public GuildRoleCommands(
                IGuildPreferences guildPreferences,
                IAssignableRoleService assignableRoleService

                )
                {
                    _guildPreferences = guildPreferences;
                    _assignableRoleService = assignableRoleService;
                }

                [GroupCommand]
                [RequirePrefixes("w@", "W@")]
                public async Task ShowAssignableRoles(CommandContext ctx)
                {
                    var json = await GetRoleJson(ctx).ConfigureAwait(true);


                    StringBuilder sb = new StringBuilder();

                    foreach (Roles role in json.Roles)
                    {
                        DiscordEmoji emoji = DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiId);
                        string roleName = ctx.Guild.GetRole(role.RoleId).Name;

                        sb.Append(emoji + $" : {roleName}\n");
                    }

                    var embed = new DiscordEmbedBuilder
                    {
                        Description = $"To edit the roles that are available use the `w@guild role add [role]` and `w@guild role rm [role]`.\nRoles available:\n{sb}",
                        Color = DiscordColor.Orange
                    };

                    await ctx.Channel.SendMessageAsync(" ", embed: embed);
                }

                [Command("add")]
                [RequirePrefixes("w@", "W@")]
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
                            await ctx.Channel.SendMessageAsync("Either that role or emoji has already been used, if you wish to remove a role and add it again please use the `w@guild role rm [Role] command`");
                            return;
                        }
                    }

                    await _assignableRoleService.AddRoleToAssignableRoles(ctx.Guild.Id, discordRole.Id, emoji.Id);

                    await ctx.Channel.SendMessageAsync($"{discordRole.Name} was added to the assignable roles list! Please not if the role is higher up than the bots role wiggims bot will not be able to assign this role. Mr_Wiggims1 hasn't figured out how to check for this yet.");
                }

                [Command("Remove")]
                [RequirePrefixes("w@", "W@")]
                [Aliases("rm")]
                [Description("removes a role from the assignable roles list.")]
                [RequireUserPermissions(Permissions.Administrator)]
                public async Task RemoveAssignableRole(CommandContext ctx, [Description("Discord Role to remove.")] DiscordRole discordRole)
                {
                    await ctx.TriggerTypingAsync();

                    AssignableRoleJson json = await GetRoleJson(ctx);

                    if (!json.Roles.Exists(x => x.RoleId == discordRole.Id))
                    {
                        throw new Exception("This role does not exist in the list for this command.");
                    }

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

                //######## Guild role Tasks ############
                public async Task<AssignableRoleJson> GetRoleJson(CommandContext ctx)
                {
                    GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                    AssignableRoleJson json = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

                    return json;
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

            [Command("SetTimeoutRole")]
            [RequirePrefixes("w@", "W@")]
            [Description("Sets the role that the timeout command will give the target member.")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task SetTimeoutRole(CommandContext ctx, [Description("The role to be given during timeouts.")]DiscordRole discordRole)
            {
                if (discordRole.Name.ToLower() == "timeout")
                {
                    await ctx.RespondAsync("Please do not use 'timeout' as the name as when you try to execute the `w@timeout` command you will ping the role rather than execute the command.").ConfigureAwait(false);
                    return;
                }

                await _guildPreferences.SetTimeoutRole(ctx.Guild.Id, discordRole.Id).ConfigureAwait(true);

                await ctx.RespondAsync($"{discordRole.Name} has been set as the timeout role for {ctx.Guild.Name}.").ConfigureAwait(false);
            }

            [Group("Stats")]
            [RequirePrefixes("w@", "W@")]
            [Description("Used for configuring the stats show withing channels.")]
            [Aliases("stat")]
            public class Stat : BaseCommandModule
            {
                private readonly DbContextOptions<RPGContext> _options;

                private readonly IGuildPreferences _guildPreferences;
                private readonly IProfileService _profileService;
                private readonly IStatChannelService _statChannelService;

                public Stat(
                    IGuildPreferences guildPreferences,
                    IProfileService profileService,
                    IStatChannelService statChannelService,
                    DbContextOptions<RPGContext> options
                    )
                {
                    _guildPreferences = guildPreferences;
                    _profileService = profileService;
                    _statChannelService = statChannelService;
                    _options = options;
                }

                [Command("setcategory")]
                [RequirePrefixes("w@", "W@")]
                [Description("Sets the channel category that will show stats from wiggims bot.")]
                [RequireUserPermissions(Permissions.Administrator)]
                public async Task SetStatChannelCatergory(CommandContext ctx, [Description("The category that will house all the stat channels.")]DiscordChannel discordChannel)
                {
                    if (!discordChannel.IsCategory)
                        await ctx.RespondAsync("You must use a channel category or else this will not work.");

                    await _guildPreferences.SetStatChannelCategory(ctx.Guild.Id, discordChannel.Id).ConfigureAwait(true);

                    await ctx.RespondAsync($"{discordChannel.Name} has been set as the category that will wiggims bot will use to show stats to the rest of the server.").ConfigureAwait(false);
                }
            
                [GroupCommand]
                [RequireUserPermissions(Permissions.Administrator)]
                public async Task AddOrModify(CommandContext ctx)
                {
                    var guildPrefs = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);
                    DiscordChannel parentChannel = ctx.Guild.GetChannel(guildPrefs.StatChannelCatergoryId);

                    if (parentChannel == null)
                    {
                        throw new Exception("You have not yet added a category where stats will be shown under. `w@guild stat setcategory <Channel category>`.");
                    }

                    var messageStep = new TextStep("What would you like the channel name to say? Will show as `<Your text here>: [stat] [Members name]` the limited number of characters is set low to prevent the stat from being hidden.", null, 0, 12);
                    var statStep = new IntStep("What kind of stat would you like to track you can only have one of each type, to modify an existing one just select the same option? Please enter the corresponding number.\nMost gold - 0\nRobbing attack success rate - 1\nRobbing defense success rate - 2\nXp - 3\nGots - 4\nBoganness - 5\nGold Stolen - 6\nSpelling accuracy - 7\nGold Lost From Theft - 8\nRobbing Attack Wins - 9\nRobbing Defend Wins - 10\nHeads or tails wins - 11\nHeads or tails losses - 12\nCoin flip Win Rate - 13", messageStep, 0, 13);

                    int statInput;
                    StatOption stat = StatOption.Gold;

                    string msgInput;
                    string message = "1";

                    statStep.OnValidResult += (result) =>
                    {
                        statInput = result;

                        stat = (StatOption)result;

                        statStep.SetNextStep(messageStep);
                    };

                    messageStep.onValidResult += (result) =>
                    {
                        msgInput = result;

                        message = result;
                    };

                    var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

                    var inputDialogueHandler = new DialogueHandler(
                        ctx.Client,
                        ctx.Channel,
                        ctx.User,
                        statStep
                    );

                    bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

                    if (!succeeded) { return; }

                    ulong channelId;

                    if (guildPrefs.StatChannels.Exists(x => x.StatOption == stat))
                    {
                        channelId = guildPrefs.StatChannels.Where(x => x.StatOption == stat).FirstOrDefault().ChannelId;
                    }
                    else
                    {
                        channelId = ctx.Guild.CreateVoiceChannelAsync("waiting for update...", parentChannel).Result.Id;
                    }

                    await _statChannelService.CreateOrModifyStatChannel(ctx.Guild.Id, channelId, stat, message);

                    await ctx.Channel.SendMessageAsync("There should be a new text channel in the specified category and will be updated on the next stat update cycle.").ConfigureAwait(false);
                }

                private Dictionary<ulong,bool> updateNeeded = new Dictionary<ulong, bool>();

                [Command("queueupdate")]
                [Hidden]
                public async Task QueueUpdateChannelStats(CommandContext ctx)
                {
                    if (!updateNeeded.ContainsKey(ctx.Guild.Id))
                    {
                        ctx.Client.DebugLogger.LogMessage(LogLevel.Warning, ctx.Client.CurrentApplication.Name, $"{ctx.Guild.Name} needs stats to be updated, created dictionary instance and update queued.", DateTime.Now);
                        updateNeeded.Add(ctx.Guild.Id, true);

                        var guildPrefs = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                        await ctx.Guild.GetChannel(guildPrefs.StatChannelCatergoryId).ModifyAsync(x => x.Name = $"Server Stats - Updates at :x{DateTime.Now.Minute % 10}");
                    }
                    else
                    {
                        if (updateNeeded[ctx.Guild.Id] == true)
                            return;

                        updateNeeded[ctx.Guild.Id] = true;
                        ctx.Client.DebugLogger.LogMessage(LogLevel.Warning, ctx.Client.CurrentApplication.Name, $"{ctx.Guild.Name} needs stats to be updated, update queued.", DateTime.Now);
                    }
                }

                [Command("update")]
                [Hidden]
                public async Task UpdateChannelStats(CommandContext ctx, bool force = false)
                {
                    if (!updateNeeded.ContainsKey(ctx.Guild.Id))
                    {
                        ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, ctx.Client.CurrentApplication.Name, $"{ctx.Guild.Name} not found in updateNeeded dictionary creating new instance.", DateTime.Now);
                        updateNeeded.Add(ctx.Guild.Id, true);
                    }

                    if (!updateNeeded[ctx.Guild.Id] && !force)
                    {
                        ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, ctx.Client.CurrentApplication.Name, $"{ctx.Guild.Name} Does not need to update stats.", DateTime.Now);
                        return;
                    }

                    ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, ctx.Client.CurrentApplication.Name, $"{ctx.Guild.Name} needs stats to be updating, updating now...", DateTime.Now);

                    updateNeeded[ctx.Guild.Id] = false;
                    

                    using var context = new RPGContext(_options);

                    var guildPrefs = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                    guildPrefs.TotalCommandsExecuted++;
                    context.Update(guildPrefs);
                    await context.SaveChangesAsync();

                    if (guildPrefs.StatChannels.Count == 0)
                    {
                        ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Debug, ctx.Client.CurrentApplication.Name, "This guild is not configured to update a stat channel, updating other stats and returning.", DateTime.Now);
                        return;
                    }

                    foreach (var statChannel in guildPrefs.StatChannels)
                    {
                        DiscordChannel channel = null;

                        channel = ctx.Guild.GetChannel(statChannel.ChannelId);

                        if (channel == null)
                        {
                            ctx.Client.DebugLogger.LogMessage(LogLevel.Warning, ctx.Client.CurrentApplication.Name, $"The stat channel for stat option {statChannel.StatOption} in the guild '{ctx.Guild.Name}' no longer exists and the data base entry will be removed.", DateTime.Now);
                            await _statChannelService.DeleteStatChannel(ctx.Guild.Id, statChannel.StatOption);
                            break;
                        }

                        await channel.ModifyAsync(x => x.Name = $"{statChannel.StatMessage}: { GetStat(ctx, statChannel.StatOption) }").ConfigureAwait(false);
                    }
                }

                private string GetStat(CommandContext ctx, StatOption option)
                {
                    var list = _profileService.GetAllGuildProfiles(ctx.Guild.Id);
                    var memberList = ctx.Guild.GetAllMembersAsync().Result;
                    memberList = new List<DiscordMember>(memberList);
                    var memberIdList = memberList.Where(x => !x.IsBot).Select(x => x.Id).ToList();

                    try
                    {
                        switch (option)
                        {
                            case StatOption.Boganness:
                                {
                                    var returnMember = list.OrderByDescending(x => x.Boganometer).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{Math.Round(returnMember.Boganometer)}% {userName}";
                                }
                            case StatOption.Gold:
                                {
                                    var returnMember = list.OrderByDescending(x => x.Gold).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.Gold} {userName}";
                                }
                            case StatOption.GoldStolen:
                                {
                                    var returnMember = list.OrderByDescending(x => x.GoldStolen).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.GoldStolen} {userName}";
                                }
                            case StatOption.Gots:
                                {
                                    var returnMember = list.OrderByDescending(x => x.Gots).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.Gots} {userName}";
                                }
                            case StatOption.RobAttackSuccessRate:
                                {
                                    var returnMember = list.Where(x => x.RobbingAttackWon + x.RobbingAttackLost > 10).OrderByDescending(x => x.RobAttackSuccessRate).Where(x => memberIdList.Contains(x.DiscordId) && x.RobAttackSuccessRate > 0).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{Math.Round(returnMember.RobAttackSuccessRate)}% {userName}";
                                }
                            case StatOption.RobDefendSuccessRate:
                                {
                                    var returnMember = list.Where(x => x.RobbingDefendWon + x.RobbingDefendLost > 10).OrderByDescending(x => x.RobDefendSuccessRate).Where(x => memberIdList.Contains(x.DiscordId) && x.RobDefendSuccessRate > 0).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{Math.Round(returnMember.RobDefendSuccessRate)}% {userName}";
                                }
                            case StatOption.SpellAcc:
                                {
                                    var returnMember = list.OrderByDescending(x => x.SpellAcc).Where(x => memberIdList.Contains(x.DiscordId) && x.SpellAcc > 0).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{Math.Round(returnMember.SpellAcc)}% {userName}";
                                }
                            case StatOption.Xp:
                                {
                                    var returnMember = list.OrderByDescending(x => x.Xp).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.Xp} {userName}";
                                }
                            case StatOption.GoldLostFromTheft:
                                {
                                    var returnMember = list.OrderByDescending(x => x.GoldLostFromTheft).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.GoldLostFromTheft} {userName}";
                                }
                            case StatOption.RobbingAttackWon:
                                {
                                    var returnMember = list.OrderByDescending(x => x.RobbingAttackWon).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.RobbingAttackWon} {userName}";
                                }
                            case StatOption.RobbingDefendWon:
                                {
                                    var returnMember = list.OrderByDescending(x => x.RobbingDefendWon).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.RobbingDefendWon} {userName}";
                                }
                            case StatOption.HeadsOrTailsLosses:
                                {
                                    var returnMember = list.OrderByDescending(x => x.CoindFlipsLost).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.CoindFlipsLost} {userName}";
                                }
                            case StatOption.HeadsOrTailsWins:
                                {
                                    var returnMember = list.OrderByDescending(x => x.CoinFilpsWon).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.CoinFilpsWon} {userName}";
                                }
                            case StatOption.CoinflipWinRate:
                                {
                                    var returnMember = list.Where(x => x.CoinFilpsWon + x.CoindFlipsLost > 10).OrderByDescending(x => x.CoinFlipWinRate).Where(x => memberIdList.Contains(x.DiscordId)).First();
                                    string userName = ctx.Guild.GetMemberAsync(returnMember.DiscordId).Result.Username;
                                    return $"{returnMember.CoinFlipWinRate}% {userName}";
                                }
                            default:
                                {
                                    return "Error stat type not found.";
                                }
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        return "Not Enough Data";
                    }
                }

            }
            //######## Guild role Tasks ############
            public async Task<AssignableRoleJson> GetRoleJson(CommandContext ctx)
            {
                GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

                AssignableRoleJson json = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

                return json;
            }
        }

    }
}