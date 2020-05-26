using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using System.Linq;
using System;

namespace WigsBot.Bot.Commands.Profilecommands
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IExperienceService _experienceService;
        private readonly IGotService _gotService;
        private readonly ILeaveService _leaveService;
        private readonly ISpellErrorService _spellErrorService;
        private readonly ISpellCorrectService _spellCorrectService;
        private readonly IBoganCountService _boganCountService;
        private readonly IQuietModeService _quietModeService;
        private readonly ITextProcessorService _textProcessorService;

        public ProfileCommands(
            IProfileService profileService,
            IExperienceService experienceService,
            IGotService gotService,
            ILeaveService leaveService,
            ISpellErrorService spellErrorService,
            ISpellCorrectService spellCorrectService,
            IBoganCountService boganCountService,
            IQuietModeService quietModeService,
            ITextProcessorService textProcessorService
            )
        {
            _profileService = profileService;
            _experienceService = experienceService;
            _gotService = gotService;
            _leaveService = leaveService;
            _spellErrorService = spellErrorService;
            _spellCorrectService = spellCorrectService;
            _boganCountService = boganCountService;
            _quietModeService = quietModeService;
            _textProcessorService = textProcessorService;
        }

        [Command("ProfileInfo")]
        [RequirePrefixes("w@", "W@")]
        [Description("show all information saved by Wiggims Bot on a user (ping or use user Id).")]
        public async Task ProfileInfo(CommandContext ctx, DiscordUser user)
        {
            await ctx.TriggerTypingAsync();

            await GetProfileInfoToDisplayAsync(ctx, user.Id);
        }

        [Command("ProfileInfo")]
        [RequirePrefixes("w@", "W@")]
        [Description("show all information saved by Wiggims Bot on a user (ping or use user Id).")]
        public async Task ProfileInfo(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            await GetProfileInfoToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("ProfileAllGuild")]
        [RequirePrefixes("w@","W@")]
        [Description("Gets a profile from any guild.")]
        public async Task ProfileAllGuild(CommandContext ctx, [Description("The Id of the user")] ulong memberId, [Description("The Id of the guild")] ulong guildId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, guildId);

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{memberId}'s Profile",
                Timestamp = System.DateTime.Now
            };

            try { profileEmbed.AddField("DB Id:", profile.Id.ToString(), true); } catch { profileEmbed.AddField("Level:", "Error", true); }
            try { profileEmbed.AddField("Level:", profile.Level.ToString(), true); } catch { profileEmbed.AddField("Level:", "Error", true); }
            try { profileEmbed.AddField("xp:", profile.Xp.ToString(), true); } catch { profileEmbed.AddField("xp:", "Error", true); }
            try { profileEmbed.AddField("Gold:", profile.Gold.ToString(), true); } catch { profileEmbed.AddField("Gold:", "Error", true); }
            try { profileEmbed.AddField("Gots:", profile.Gots.ToString(), true); } catch { profileEmbed.AddField("Gots:", "Error", true); }
            try { profileEmbed.AddField("Spell Correct", profile.SpellCorrectCount.ToString(), true); } catch { profileEmbed.AddField("Spell Correct", "Error", true); }
            try { profileEmbed.AddField("Spell Error", profile.SpellErrorCount.ToString(), true); } catch { profileEmbed.AddField("Spell Error", "Error", true); }
            try { profileEmbed.AddField("Bogan Count:", profile.BoganCount.ToString(), true); } catch { profileEmbed.AddField("Bogan Count:", "Error", true); }
            try { profileEmbed.AddField("Spell Accuracy:", profile.SpellAcc.ToString() + "%", true); } catch { profileEmbed.AddField("Spell Accuracy:", "Error", true); }
            try { profileEmbed.AddField("Bogan:", profile.BoganPercent.ToString() + "%", true); } catch { profileEmbed.AddField("Bogan:", "Error", true); }
            try { profileEmbed.AddField("Boganness:", profile.Boganometer.ToString() + "%", true); } catch { profileEmbed.AddField("Boganness:", "Error", true); }
            try { profileEmbed.AddField("Steam Id:", profile.SteamId.ToString(), true); } catch { profileEmbed.AddField("Steam Id:", "Error", true); }
            try { profileEmbed.AddField("Beatsaber Id:", profile.BeatSaberId.ToString(), true); } catch { profileEmbed.AddField("Beatsaber Id:", "Error", true); }
            try { profileEmbed.AddField("Uplay Username:", profile.UplayUsername.ToString(), true); } catch { profileEmbed.AddField("Uplay Username:", "Error", true); }
            try { profileEmbed.AddField("Battle.Net Username:", profile.BattleNetUsername.ToString(), true); } catch { profileEmbed.AddField("Battle.Net Username:", "Error", true); }
            try { profileEmbed.AddField("Leave count:", profile.LeaveCount.ToString(), true); } catch { profileEmbed.AddField("Leave count:", "Error", true); }
            try { profileEmbed.AddField("Spelling error list:", profile.SpellErrorList.ToString(), true); } catch { profileEmbed.AddField("Spelling Error List:", "Error", true); }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed);
        }

        [Command("AddXp")]
        [RequirePrefixes("w@", "W@")]
        [Description("Command for giving and taking XP from a user. (Dadmin only)")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot")]
        [Aliases("GrantXp", "XpAdd", "XpGrant")]
        public async Task AddXp(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("Amount of Xp to add or take away (negative to take).")] int XpNum)
        {

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            GrantXpViewModel viewModel = await _experienceService.GrantXpAsync(member.Id, ctx.Guild.Id, XpNum).ConfigureAwait(false);
            
            if (!viewModel.LevelledUp) { return; }
            if (profile.QuietMode == true) { return; }

            var levelUpEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName} Is Now Level {viewModel.Profile.Level}",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color
            };

            levelUpEmbed.WithFooter("w!togglenotifications to hide In future.");

            await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
        }

        [Command("AddGot")]
        [RequirePrefixes("w@", "W@")]
        [Description("Adds a got to a user, Dadmin only.")]
        [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot", "Bots")]
        [Aliases("GrantGot", "Gitem", "Gotem", "Got")]
        public async Task AddGot(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("Number of gots to add or take away (negative to take).")] int gotNum, [RemainingText] string remainingText)
        {
            if (member.IsBot) { return; }

            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":cooldoge:");

            await ctx.Message.CreateReactionAsync(emoji).ConfigureAwait(false);

            await GrantGot(ctx, gotNum, member.Id);
        }

        [Command("AddGotautomaticgot")]
        [Hidden]
        public async Task AddGotsecret(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("Number of gots to add or take away (negative to take).")] int gotNum, [RemainingText] string remainingText)
        {
            if (member.IsBot) { return; }

            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":cooldoge:");

            await ctx.Message.CreateReactionAsync(emoji).ConfigureAwait(false);

            await GrantGot(ctx, gotNum, member.Id);
        }

        [Command("AddSpellError")]
        [RequirePrefixes("w@", "W@")]
        [Description("Adds or takes spelling errors to a user, Dadmin only.")]
        [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot")]
        public async Task AddSpellError(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("amount of Spelling errors to add or take away (negative to take).")] int SpellError)
        {
            await GrantSpellError(ctx, SpellError, member.Id);
            await ctx.Channel.SendMessageAsync($"{member.Username} has been given {SpellError} errors to their tally.");
        }

        [Command("AddSpellCorrect")]
        [RequirePrefixes("w@", "W@")]
        [Description("Adds or takes spelling Correct to a user, Dadmin only.")]
        [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot")]
        public async Task AddSpellCorrect(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("amount of Spelling Corrects to add or take away (negative to take).")] int SpellCorrect)
        {
            await GrantSpellCorrect(ctx, SpellCorrect, member.Id);
            await ctx.Channel.SendMessageAsync($"{member.Username} has been given {SpellCorrect} Corrects to their tally.");
        }        

        [Command("AddLeave")]
        [RequirePrefixes("w@", "W@")]
        [Description("Adds a Leave to a user, Dadmin only.")]
        [RequireRoles(RoleCheckMode.Any, "Dadmin", "Wiggims Bot")]
        [Aliases("Leave")]
        public async Task AddLeave(CommandContext ctx, [Description("Mention or use member Id.")] DiscordMember member, [Description("Number of Leaves to add or take away (negative to take).")] int LeaveNum)
        {
            await ctx.TriggerTypingAsync();

            await GrantLeave(ctx, LeaveNum, member.Id);

            if (LeaveNum > 0)
            {
                await ctx.Channel.SendMessageAsync($"Added {LeaveNum} Leaves to {member.Username}.").ConfigureAwait(false);
            }
            else if (LeaveNum == 1)
            {
                await ctx.Channel.SendMessageAsync($"Added {LeaveNum} Leave to {member.Username}.").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{LeaveNum} Leaves have been removed from {member.Username}.").ConfigureAwait(false);
            }

        }

        [Command("AddTacoLeave")]
        [Description("Adds a Leave to a user, Dadmin only.")]
        [Hidden]
        public async Task AddtacoLeave(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            await GrantLeave(ctx, 1, ctx.Member.Id);
        }

        //########### Tasks Below ################

        private async Task GetProfileInfoToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            try { profileEmbed.AddField("DB Id:", profile.Id.ToString(), true); } catch { profileEmbed.AddField("Level:", "Error", true); }
            try { profileEmbed.AddField("Level:", profile.Level.ToString(), true); } catch { profileEmbed.AddField("Level:", "Error", true); }
            try { profileEmbed.AddField("xp:", profile.Xp.ToString(), true); } catch { profileEmbed.AddField("xp:", "Error", true); }
            try { profileEmbed.AddField("Gold:", profile.Gold.ToString(), true); } catch { profileEmbed.AddField("Gold:", "Error", true); }
            try { profileEmbed.AddField("Gots:", profile.Gots.ToString(), true); } catch { profileEmbed.AddField("Gots:", "Error", true); }
            try { profileEmbed.AddField("Spell Correct", profile.SpellCorrectCount.ToString(), true); } catch { profileEmbed.AddField("Spell Correct", "Error", true); }
            try { profileEmbed.AddField("Spell Error", profile.SpellErrorCount.ToString(), true); } catch { profileEmbed.AddField("Spell Error", "Error", true); }
            try { profileEmbed.AddField("Bogan Count:", profile.BoganCount.ToString(), true); } catch { profileEmbed.AddField("Bogan Count:", "Error", true); }
            try { profileEmbed.AddField("Spell Accuracy:", profile.SpellAcc.ToString() + "%", true); } catch { profileEmbed.AddField("Spell Accuracy:", "Error", true); }
            try { profileEmbed.AddField("Bogan:", profile.BoganPercent.ToString() + "%", true); } catch { profileEmbed.AddField("Bogan:", "Error", true); }
            try { profileEmbed.AddField("Boganness:", profile.Boganometer.ToString() + "%", true); } catch { profileEmbed.AddField("Boganness:", "Error", true); }
            try { profileEmbed.AddField("Steam Id:", profile.SteamId.ToString(), true); } catch { profileEmbed.AddField("Steam Id:", "Error", true); }
            try { profileEmbed.AddField("Beatsaber Id:", profile.BeatSaberId.ToString(), true); } catch { profileEmbed.AddField("Beatsaber Id:", "Error", true); }
            try { profileEmbed.AddField("Uplay Username:", profile.UplayUsername.ToString(), true); } catch { profileEmbed.AddField("Uplay Username:", "Error", true); }
            try { profileEmbed.AddField("Battle.Net Username:", profile.BattleNetUsername.ToString(), true); } catch { profileEmbed.AddField("Battle.Net Username:", "Error", true); }
            try { profileEmbed.AddField("Leave count:", profile.LeaveCount.ToString(), true); } catch { profileEmbed.AddField("Leave count:", "Error", true); }
            try { profileEmbed.AddField("Spelling error list:", profile.SpellErrorList.ToString(), true); } catch { profileEmbed.AddField("Spelling Error List:", "Error", true); }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            profileEmbed.AddField("Level", profile.Level.ToString(), true);
            profileEmbed.AddField("Gold", ":moneybag: " + profile.Gold.ToString(), true);
            profileEmbed.AddField("Gots", profile.Gots.ToString(), true);
            if (profile.LeaveCount > 0)
            {
                profileEmbed.AddField("Leave count", profile.LeaveCount.ToString(), true);
            }

            try
            {
                profileEmbed.AddField("Boganness", profile.Boganometer.ToString() + " %", true);
            }
            catch { }
           /*            
            if (profile.Items.Count > 0)
            {
                profileEmbed.AddField("Items in Inventory", string.Join(", ", profile.Items.Select(x => x.Item.Name)));
            }
            await ctx.Channel.SendMessageAsync(embed: profileEmbed);
            */
        }

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

            levelUpEmbed.WithFooter("w!togglenotifications to hide In future.");

            await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
        }

        private async Task GrantLeave(CommandContext ctx, int LeaveNum, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await _leaveService.GrantLeaveAsync(memberId, ctx.Guild.Id, LeaveNum).ConfigureAwait(false);
        }

        private async Task GrantSpellError(CommandContext ctx, int errorCount, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];
        }

        private async Task GrantSpellCorrect(CommandContext ctx, int errorCount, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await _spellCorrectService.GrantSpellCorrectAsync(memberId, ctx.Guild.Id, errorCount).ConfigureAwait(false);
        }

        private async Task GrantBoganCount(CommandContext ctx, int BoganCount, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await _boganCountService.GrantBoganCountAsync(memberId, ctx.Guild.Id, BoganCount).ConfigureAwait(false);
        }
    }
}
    
