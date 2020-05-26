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
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using System.Threading;
using WigsBot.Core.ViewModels;
using WigsBot.DAL.Models.Items;
using WigsBot.Bots.JsonModels;
using WigsBot.Core.Services.Items;

namespace WigsBot.Bot.Commands.Profilecommands
{
    [Group("profile")]
    [RequirePrefixes("w!", "W!")]
    [RequireGuild]
    [Description("Shows your profile with menu for more information.")]
    [Aliases("profiles","bal","balance","money","xp","level")]
    public class ExtendedProfileCommands : BaseCommandModule
    {

        private readonly IProfileService _profileService;
        private readonly IQuietModeService _quietModeService;
        private readonly IMimicableService _mimicableService;
        private readonly IRobbingItemService _robbingItemService;

        public ExtendedProfileCommands(
            IProfileService profileService,
            IQuietModeService quietModeService,
            IMimicableService mimicableService,
            IRobbingItemService robbingItemService
            )
        {
            _profileService = profileService;
            _quietModeService = quietModeService;
            _mimicableService = mimicableService;
            _robbingItemService = robbingItemService;
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task profile(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendBasicInfo(ctx, message, profile, member);
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task profile(CommandContext ctx, [Description("Discord user")] DiscordUser discordUser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordUser.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendBasicInfo(ctx, message, profile, member);
        }

        //

        [Command("GameId")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows Game ids used for accessing stats.")]
        [Aliases("gameinfo", "gamesettings", "gameoptions", "game")]
        public async Task GameId(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendGameIdInfo(ctx, message, profile, member);
        }

        [Command("GameId")]
        [RequirePrefixes("w!", "W!")]
        public async Task GameId(CommandContext ctx, [Description("Discord user")] DiscordUser discordUser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordUser.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendGameIdInfo(ctx, message, profile, member);
        }

        [Command("spelling")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows spelling stats")]
        [Aliases("Spell", "canispell")]
        public async Task spelling(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendSpellingInfo(ctx, message, profile, member);
        }

        [Command("spelling")]
        [RequirePrefixes("w!", "W!")]
        public async Task spelling(CommandContext ctx, [Description("Discord user")] DiscordUser discordUser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordUser.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendSpellingInfo(ctx, message, profile, member);
        }

        [Command("options")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows your profile settings")]
        [Aliases("settings")]
        public async Task options(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendProfilePrefsInfo(ctx, message, profile, member);
        }

        [Command("options")]
        [RequirePrefixes("w!", "W!")]
        public async Task options(CommandContext ctx, [Description("Discord user")] DiscordUser discordUser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordUser.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendProfilePrefsInfo(ctx, message, profile, member);
        }

        [Command("Inventory")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows your inventory")]
        [Aliases("inv","items")]
        public async Task inventory(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendUserInventory(ctx, message, profile, member);
        }

        [Command("inventory")]
        [RequirePrefixes("w!", "W!")]
        public async Task inventory(CommandContext ctx, [Description("Discord user")] DiscordUser discordUser)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var message = await ctx.Channel.SendMessageAsync("Getting Profile...");

            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordUser.Id, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await SendUserInventory(ctx, message, profile, member);
        }

        //

        [Command("togglenotifications")]
        [RequirePrefixes("w!", "W!")]
        [Description("toggles level up, and got mile stones notifications.")]
        public async Task togglenotifications(CommandContext ctx)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            if (profile.QuietMode)
            {
                await _quietModeService.SetQuietModeAsync(ctx.Member.Id, ctx.Guild.Id, false);
                await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has enabled notifications.").ConfigureAwait(false);
            }
            else if (!profile.QuietMode)
            {
                await _quietModeService.SetQuietModeAsync(ctx.Member.Id, ctx.Guild.Id, true);
                await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has disabled notifications.").ConfigureAwait(false);
            }
        }

        [Command("togglenotifications")]
        [RequirePrefixes("w!", "W!")]
        public async Task togglenotifications(CommandContext ctx, [Description("True Or False")] bool trueOrFalse)
        {
            await _quietModeService.SetQuietModeAsync(ctx.Member.Id, ctx.Guild.Id, trueOrFalse);

            await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has muted notifications: {trueOrFalse}.").ConfigureAwait(false);
        }

        [Command("togglemimicking")]
        [RequirePrefixes("w!", "W!")]
        [Description("toggles level up, and got mile stones notifications.")]
        public async Task toggletogglemimics(CommandContext ctx)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            await _mimicableService.SetMimicableAsync(ctx.Member.Id, ctx.Guild.Id, !profile.IsMimicable);

            await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has set allow mimicking to {!profile.IsMimicable}.").ConfigureAwait(false);
        }

        [Command("togglemimicking")]
        [RequirePrefixes("w!", "W!")]
        public async Task toggletogglemimics(CommandContext ctx, [Description("True Or False")] bool trueOrFalse)
        {
            await _mimicableService.SetMimicableAsync(ctx.Member.Id, ctx.Guild.Id, trueOrFalse).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} has set allow mimicking to {trueOrFalse}.").ConfigureAwait(false);
        }

        // ########### Profile Tasks ##############

        public async Task AwaitReaction(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member, List<DiscordEmoji> emojis)
        {
            try
            {
                var interactivity = ctx.Client.GetInteractivity();

                foreach (var emoji in emojis)
                {
                    await message.CreateReactionAsync(emoji);
                }

                var ReactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == message &&
                    x.User == ctx.User &&
                    (emojis.Contains(x.Emoji))).ConfigureAwait(false);

                if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":abc:")) { await SendSpellingInfo(ctx, message, profile, member); }
                else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":joystick:")) { await SendGameIdInfo(ctx, message, profile, member); }
                else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":gear:")) { await SendProfilePrefsInfo(ctx, message, profile, member); }
                else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":smiley:")) { await SendBasicInfo(ctx, message, profile, member); }
                else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":scroll:")) { await SendUserInventory(ctx, message, profile, member); }
                else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":shopping_cart:")) { await showShop(ctx, message, profile, member); }
            }
            catch
            { await message.DeleteAllReactionsAsync(); }
        }

        public async Task SendBasicInfo(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            profileEmbed.AddField("Level", $"{profile.Level.ToString()} - `{profile.Xp} xp`", true);
            profileEmbed.AddField("Gold", ":moneybag: " + profile.Gold.ToString(), true);
            profileEmbed.AddField("Gots", profile.Gots.ToString(), true);
            profileEmbed.AddField("Boganness", profile.Boganometer.ToString() + " %", true);
            if (profile.LeaveCount > 0)
            {
                profileEmbed.AddField("Leave count", profile.LeaveCount.ToString(), true);
            }

            await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":joystick:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gear:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":scroll:"));
            if (member.Id == ctx.Member.Id)
            {
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":shopping_cart:"));
            }

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        public async Task SendSpellingInfo(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s spelling stats.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            profileEmbed.AddField("Words Spelled Correctly:", profile.SpellCorrectCount.ToString(), true);
            profileEmbed.AddField("Words Spelled Incorrectly:", profile.SpellErrorCount.ToString(), true);
            profileEmbed.AddField("Spelling Accuracy:", profile.SpellAcc.ToString() + " %", true);
            profileEmbed.AddField("Most Recent Spelling Mistakes (not working):", profile.SpellErrorList.ToString(), true);

            profileEmbed.WithFooter("Please use `w!addtodictionary` if you wish, don't abuse, you will be black listed from the command.");

            await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":smiley:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":joystick:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gear:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":scroll:"));
            if (member.Id == ctx.Member.Id)
            {
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":shopping_cart:"));
            }

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        public async Task SendGameIdInfo(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Game Ids.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            profileEmbed.AddField("Steam Id:", profile.SteamId.ToString(), true);
            profileEmbed.AddField("Uplay Id:", profile.UplayUsername.ToString(), true);
            profileEmbed.AddField("Beatsaber Id:", profile.BeatSaberId.ToString(), true);
            profileEmbed.AddField("Battle.net Id:", profile.BattleNetUsername.ToString(), true);

            await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":smiley:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gear:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":scroll:"));
            if (member.Id == ctx.Member.Id)
            {
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":shopping_cart:"));
            }

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        public async Task SendProfilePrefsInfo(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile Information.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            profileEmbed.AddField("User Id:", profile.DiscordId.ToString(), true);
            profileEmbed.AddField("DB Id:", profile.Id.ToString(), true);
            profileEmbed.AddField("Notifications muted:", profile.QuietMode.ToString(), true);
            profileEmbed.AddField("Allow mimicking:", profile.IsMimicable.ToString(), true);

            await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":smiley:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":joystick:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":scroll:"));
            if (member.Id == ctx.Member.Id)
            {
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":shopping_cart:"));
            }

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        public async Task SendUserInventory(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            var ItemsIdList = new List<int>();

            foreach (var item in itemsJson.Robbing)
            {
                ItemsIdList.Add(item.Id);
            }

            var items = await _robbingItemService.GetRobbingItems(ItemsIdList);
            var userBuffStats = await _robbingItemService.GetBuffStats(profile);

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Inventory.",
                Description = $"Current robbing strength:\nAttack: {userBuffStats.attack * 100}%\nDefense: {userBuffStats.defense*100}%\nInventory is worth {await _robbingItemService.GetInvWorth(profile)} Gold.",
                ThumbnailUrl = member.AvatarUrl,
                Color = member.Color,
                Timestamp = System.DateTime.Now
            };

            string itemTitle = new string(string.Empty);

            foreach (var item in items)
            {
                if (item.MaxAllowed > 1)
                {
                    var count = itemsJson.Robbing.SingleOrDefault(x => x.Id == item.Id).Count;
                    itemTitle = $"{item.Name} - {count} out of {item.MaxAllowed} in inventory";
                }
                else
                {
                    itemTitle = $"{item.Name}";
                }
                profileEmbed.AddField(itemTitle, $"{item.Description} \n Attack: {item.AttackBuff * 100}%\tDefense: {item.DefenseBuff * 100}", false);
            }

            await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":joystick:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gear:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":smiley:"));

            if (member.Id == ctx.Member.Id)
            {
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":shopping_cart:"));
            }

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        public async Task showShop(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member)
        {
            await message.DeleteAllReactionsAsync();

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            List<RobbingItems> robbingItems = await _robbingItemService.GetAllRobbingItems().ConfigureAwait(false);

            var sortedList = robbingItems.OrderBy(x => x.LvlRequired);

            StringBuilder sb = new StringBuilder();

            foreach (var item in sortedList)
            {
                if (item.MaxAllowed > 1)
                {
                    try
                    {
                        if (itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count == item.MaxAllowed)
                        {
                            sb.Append($"#[already own max allowed] {item.Name}\n\n");
                        }
                        else
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n#{itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count} out of {item.MaxAllowed} owned.\n\n");
                        }
                    }
                    catch
                    {
                        try
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n#0 out of {item.MaxAllowed} owned.\n\n");
                        }
                        catch
                        {
                            sb.Append("#An error occurred.\n\n");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count == item.MaxAllowed)
                        {
                            sb.Append($"#[already own max allowed] {item.Name}\n\n");
                        }
                        else
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n\n");
                        }
                    }
                    catch
                    {
                        try
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n\n");
                        }
                        catch
                        {
                            sb.Append("#An error occurred.\n\n");
                        }
                    }
                }
            }

            await message.ModifyAsync($"```py\n@To find more information or purchase an item use `w!item info [item name]` and `w!item buy [Item Name]`\n\nYou currently have {profile.Gold} Gold are level {profile.Level}.\n\n{sb.ToString()}\n```", null).ConfigureAwait(false);

            var emojiList = new List<DiscordEmoji>();
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":smiley:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":joystick:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gear:"));
            emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":scroll:"));

            foreach (var emoji in emojiList)
            {
                await message.CreateReactionAsync(emoji);
            }

            await AwaitReaction(ctx, message, profile, member, emojiList).ConfigureAwait(false);
        }

        [Group("top")]
        [Aliases("best", "highest", "most", "sort", "list")]
        [Description("shows the top stats held by wiggims bot for all users in the discord server.")]
        public class TopProfileCommands : BaseCommandModule
        {
            private readonly IProfileService _profileService;
            private readonly IRobbingItemService _robbingItemService;

            public TopProfileCommands(
                IProfileService profileService,
                IRobbingItemService robbingItemService
                )
            {
                _profileService = profileService;
                _robbingItemService = robbingItemService;
            }

            [GroupCommand]
            [RequirePrefixes("w!", "W!")]
            public async Task top(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopXp(ctx, message, profiles, ctx.Member);
            }

            [Command("xp")]
            [RequirePrefixes("w!", "W!")]
            [Aliases("experience")]
            [Description("Shows the users with the most Xp.")]
            public async Task topxp(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopXp(ctx, message, profiles, ctx.Member);
            }

            [Description("Shows the users with the most money")]
            [Command("gold")]
            [RequirePrefixes("w!", "W!")]
            [Aliases("money")]
            public async Task topgold(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopGold(ctx, message, profiles, ctx.Member);
            }

            [Command("gots")]
            [RequirePrefixes("w!", "W!")]
            [Aliases("got")]
            [Description("Shows the users with the most gots.")]
            public async Task topgots(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopGots(ctx, message, profiles, ctx.Member);
            }

            [Command("spelling")]
            [RequirePrefixes("w!", "W!")]
            [Aliases("spell")]
            [Description("Shows the users with the highest spelling stats.")]
            public async Task topspelling(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopSpelling(ctx, message, profiles, ctx.Member);
            }

            [Command("bogan")]
            [RequirePrefixes("w!", "W!")]
            [Aliases("boganometer", "boganness")]
            [Description("Shows who is the most bogan.")]
            public async Task topBogan(CommandContext ctx)
            {
                await ctx.Message.DeleteAsync().ConfigureAwait(false);

                var message = await ctx.Channel.SendMessageAsync("Getting Profiles...");

                await ctx.TriggerTypingAsync();

                List<Profile> profiles = await _profileService.GetAllGuildProfiles(ctx.Guild.Id);

                await SendTopBogan(ctx, message, profiles, ctx.Member);
            }

            // ########### top profile tasks ##############
            public async Task AwaitTopReaction(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member, List<DiscordEmoji> emojis)
            {
                try
                {
                    var interactivity = ctx.Client.GetInteractivity();

                    foreach (var emoji in emojis)
                    {
                        await message.CreateReactionAsync(emoji);
                    }

                    var ReactionResult = await interactivity.WaitForReactionAsync(
                        x => x.Message == message &&
                        x.User == ctx.User &&
                        (emojis.Contains(x.Emoji))).ConfigureAwait(false);

                    if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":moneybag:")) { await SendTopGold(ctx, message, profiles, member); }
                    else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":abc:")) { await SendTopSpelling(ctx, message, profiles, member); }
                    else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:")) { await SendTopXp(ctx, message, profiles, member); }
                    else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":gotem:")) { await SendTopGots(ctx, message, profiles, member); }
                    else if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":beer:")) { await SendTopBogan(ctx, message, profiles, member); }
                }
                catch
                { await message.DeleteAllReactionsAsync(); }
            }

            public async Task SendTopXp(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member)
            {
                await message.DeleteAllReactionsAsync();

                List<ProfilesList> profilesList = new List<ProfilesList>();

                foreach (Profile profile in profiles)
                {
                    profilesList.Add(new ProfilesList
                    {
                        DiscordId = profile.DiscordId,
                        Xp = profile.Xp,
                        Level = profile.Level
                    });
                }

                var sortedList = profilesList.OrderByDescending(x => x.Xp).ToList();

                StringBuilder sb = new StringBuilder();

                int place = 1;

                foreach (var user in sortedList)
                {
                    try
                    {
                        if (user.Xp != 0)
                        {
                            sb.Append($"[{place}]\t> #{ ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username}\n\t\tTotal xp: {user.Xp} Level: {user.Level}\n\n");
                            place += 1;
                        }
                    }
                    catch { }
                }

                var profileEmbed = new DiscordEmbedBuilder
                {
                    Timestamp = System.DateTime.Now,
                    Title = "Users with most xp:",
                    Description = $"```py\n{sb.ToString()}-------------------------------------------------------\n@ To see more select an emoji below.\n```",
                    Color = DiscordColor.Orange
                };

                await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

                var emojiList = new List<DiscordEmoji>();
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":moneybag:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gotem:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":beer:"));

                foreach (var emoji in emojiList)
                {
                    await message.CreateReactionAsync(emoji);
                }

                await AwaitTopReaction(ctx, message, profiles, member, emojiList).ConfigureAwait(false);
            }

            public async Task SendTopGold(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member)
            {
                await message.DeleteAllReactionsAsync();

                List<ProfilesList> profilesList = new List<ProfilesList>();

                foreach (Profile profile in profiles)
                {
                    profilesList.Add(new ProfilesList
                    {
                        DiscordId = profile.DiscordId,
                        Gold = profile.Gold,
                        Level = profile.Level
                    });
                }

                var sortedList = profilesList.OrderByDescending(x => x.Gold).ToList();

                StringBuilder sb = new StringBuilder();

                int place = 1;

                foreach (var user in sortedList)
                {
                    try
                    {
                        if (user.Level >= 1)
                        {
                            sb.Append($"[{place}]\t> #{ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username}\n\t\tGold: {user.Gold}\n\n"); 
                            place += 1;
                        }
                    }
                    catch { }
                }

                var profileEmbed = new DiscordEmbedBuilder
                {
                    Timestamp = System.DateTime.Now,
                    Title = "Users with most gold:",
                    Description = $"```py\n{sb.ToString()}-------------------------------------------------------\n@This list only shows users who are are level 1+, all others probably have 0 gold\n@ To see more select an emoji below.\n```",
                    Color = DiscordColor.Orange
                };

                await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

                var emojiList = new List<DiscordEmoji>();
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gotem:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":beer:"));

                foreach (var emoji in emojiList)
                {
                    await message.CreateReactionAsync(emoji);
                }

                await AwaitTopReaction(ctx, message, profiles, member, emojiList).ConfigureAwait(false);
            }

            public async Task SendTopSpelling(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member)
            {
                await message.DeleteAllReactionsAsync();

                List<ProfilesList> profilesList = new List<ProfilesList>();

                foreach (Profile profile in profiles)
                {
                    profilesList.Add(new ProfilesList
                    {
                        DiscordId = profile.DiscordId,
                        SpellAcc = profile.SpellAcc
                    });
                }

                var sortedList = profilesList.OrderByDescending(x => x.SpellAcc).ToList();

                StringBuilder sb = new StringBuilder();

                int place = 1;

                foreach (var user in sortedList)
                {
                    try
                    {
                        if (user.SpellAcc != 100)
                        {                            
                            sb.Append($"[{place}]\t> #{ ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username}\n\t\tAccuracy: {user.SpellAcc}%\n\n");
                            place += 1;
                        }
                    }
                    catch { }
                }

                var profileEmbed = new DiscordEmbedBuilder
                {
                    Timestamp = System.DateTime.Now,
                    Title = "Users with highest spelling accuracy:",
                    Description = $"```py\n{sb.ToString()}-------------------------------------------------------\n@ To see more select an emoji below.\n```",
                    Color = DiscordColor.Orange
                };

                await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

                var emojiList = new List<DiscordEmoji>();
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":moneybag:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gotem:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":beer:"));

                foreach (var emoji in emojiList)
                {
                    await message.CreateReactionAsync(emoji);
                }

                await AwaitTopReaction(ctx, message, profiles, member, emojiList).ConfigureAwait(false);
            }

            public async Task SendTopGots(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member)
            {
                await message.DeleteAllReactionsAsync();

                List<ProfilesList> profilesList = new List<ProfilesList>();

                foreach (Profile profile in profiles)
                {
                    profilesList.Add(new ProfilesList
                    {
                        DiscordId = profile.DiscordId,
                        Gots = profile.Gots,
                        gotWordRatio = profile.gotWordRatio,
                        Xp = profile.Xp
                    });
                }

                var sortedList = profilesList.OrderByDescending(x => x.gotWordRatio).ToList();

                StringBuilder sb = new StringBuilder();

                int place = 1;

                foreach (var user in sortedList)
                {
                    try
                    {
                        if (user.Gots != 0 && user.Xp != 0)
                        {                           
                            sb.Append($"[{ place}]\t > #{ ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username}\n\t\tGot ratio: {user.gotWordRatio}% - { user.Gots} gots total\n\n");
                            Console.WriteLine($"{ place} > #{ ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username} Got ratio: {user.gotWordRatio}% - { user.Gots} gots total");
                            place += 1;                        
                        }
                    }
                    catch { }
                }

                var profileEmbed = new DiscordEmbedBuilder
                {
                    Timestamp = System.DateTime.Now,
                    Title = "Users with most gots per message sent:",
                    Description = $"```py\n{sb.ToString()}-------------------------------------------------------\n@ To see more select an emoji below.\n```",
                    Color = DiscordColor.Orange
                };

                await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

                var emojiList = new List<DiscordEmoji>();
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":moneybag:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":beer:"));

                foreach (var emoji in emojiList)
                {
                    await message.CreateReactionAsync(emoji);
                }

                await AwaitTopReaction(ctx, message, profiles, member, emojiList).ConfigureAwait(false);
            }

            public async Task SendTopBogan(CommandContext ctx, DiscordMessage message, List<Profile> profiles, DiscordMember member)
            {
                await message.DeleteAllReactionsAsync();

                List<ProfilesList> profilesList = new List<ProfilesList>();

                foreach (Profile profile in profiles)
                {
                    profilesList.Add(new ProfilesList
                    {
                        DiscordId = profile.DiscordId,
                        Boganometer = profile.Boganometer
                    });
                }

                var sortedList = profilesList.OrderByDescending(x => x.Boganometer).ToList();

                StringBuilder sb = new StringBuilder();

                int place = 1;

                foreach (var user in sortedList)
                {
                    try
                    {
                        if (user.Boganometer > 2)
                        {
                            sb.Append($"[{ place}]\t > #{ ctx.Guild.GetMemberAsync(user.DiscordId).Result.Username}\n\t\tBoganometer: { user.Boganometer}%\n\n");
                            place += 1;
                        }
                    }
                    catch { }
                }

                var profileEmbed = new DiscordEmbedBuilder
                {
                    Timestamp = System.DateTime.Now,
                    Title = "Whos the most bogan:",
                    Description = $"```py\n{sb.ToString()}-------------------------------------------------------\n@ To see more select an emoji below.\n```",
                    Color = DiscordColor.Orange
                };

                await message.ModifyAsync(content: "     ", embed: profileEmbed.Build());

                var emojiList = new List<DiscordEmoji>();
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":moneybag:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":abc:"));
                emojiList.Add(DiscordEmoji.FromName(ctx.Client, ":gotem:"));

                foreach (var emoji in emojiList)
                {
                    await message.CreateReactionAsync(emoji);
                }

                await AwaitTopReaction(ctx, message, profiles, member, emojiList).ConfigureAwait(false);
            }

            public class ProfilesList
            {
                public int Xp { get; set; }
                public ulong DiscordId { get; set; }
                public int Gold { get; set; }
                public int Gots { get; set; }
                public double SpellAcc { get; set; }
                public double gotWordRatio { get; set; }
                public double Boganometer { get; set; }
                public int Level { get; set; }
            }
        }
    }
}