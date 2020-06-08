using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using System.Linq;
using System;
using System.Text;
using WeCantSpell.Hunspell;
using System.Text.RegularExpressions;
using System.IO;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.DAL.Models.GuildPreferences;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WigsBot.Bot.Commands.Profilecommands
{
    public class AdminProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IGuildPreferences _guildPreferences;
        private readonly IExperienceService _experienceService;
        private readonly IGotService _gotService;
        private readonly ILeaveService _leaveService;
        private readonly ISpellErrorService _spellErrorService;
        private readonly ISpellCorrectService _spellCorrectService;
        private readonly IBoganCountService _boganCountService;
        private readonly IQuietModeService _quietModeService;
        private readonly ITextProcessorService _textProcessorService;
        private readonly IGoldService _goldService;

        public AdminProfileCommands(
            IProfileService profileService,
            IGuildPreferences guildPreferences,
            IExperienceService experienceService,
            IGotService gotService,
            ILeaveService leaveService,
            ISpellErrorService spellErrorService,
            ISpellCorrectService spellCorrectService,
            IBoganCountService boganCountService,
            IQuietModeService quietModeService,
            ITextProcessorService textProcessorService,
            IGoldService goldService
            )
        {
            _profileService = profileService;
            _guildPreferences = guildPreferences;
            _experienceService = experienceService;
            _gotService = gotService;
            _leaveService = leaveService;
            _spellErrorService = spellErrorService;
            _spellCorrectService = spellCorrectService;
            _boganCountService = boganCountService;
            _quietModeService = quietModeService;
            _textProcessorService = textProcessorService;
            _goldService = goldService;
        }

        [Command("oldProfile")]
        [RequirePrefixes("w!", "W!")]
        [RequireOwner]
        public async Task Profile(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("oldProfile")]
        [RequirePrefixes("w!", "W!")]
        [RequireOwner]
        [Description("Displays information about the user, defaults to user who used the command.")]
        public async Task Profile(CommandContext ctx, [Description("Ping or send members Id.")] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();

            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        [Command("earnxp")]
        [Hidden]
        [Cooldown(1,30,CooldownBucketType.User)]
        public async Task earnxp(CommandContext ctx)
        {
            int XpNum = 1;
            DiscordMember member = ctx.Member;

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            GrantXpViewModel viewModel = await _experienceService.GrantXpAsync(member.Id, ctx.Guild.Id, XpNum).ConfigureAwait(false);

            Console.WriteLine($"{ctx.Member.Username} earned {XpNum} Xp.");

            if (!viewModel.LevelledUp) { return; }

            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            int goldperlevelup = configJson.goldperlevelup;

            await _goldService.GrantGoldAsync(ctx.Member.Id, ctx.Guild.Id, goldperlevelup);

            if (profile.QuietMode == true) { return; }

            var levelUpEmbed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Member.DisplayName} Is Now Level {viewModel.Profile.Level}",
                ThumbnailUrl = ctx.Member.AvatarUrl,
                Color = ctx.Member.Color
            };

            if (goldperlevelup > 0)
                levelUpEmbed.WithDescription($"+ {goldperlevelup} :moneybag:");

            levelUpEmbed.WithFooter("`w!profile togglenotifications` to hide In future.");

            await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
        }

        [Command("WiggimsBotSpell")]
        [Description("used for wiggims bot, not for users, try w!addspellerror or w!addspellcorrect.")]
        [Hidden]
        [Cooldown(10, 60, CooldownBucketType.User)]
        public async Task WiggimsBotSpell(CommandContext ctx)
        {
            bool isCommand = ctx.Message.Content.ToLower().Contains("w!") || ctx.Message.Content.ToLower().Contains("w@");
            bool isBot = ctx.Message.Author.IsBot;
            bool isCodeBlock = ctx.Message.Content.Contains("```");
            string[] message = ctx.Message.Content.Split();
            int errorCount = 0;
            int correctCount = 0;
            int boganCount = 0;
            List<string> sb = new List<string> { };

            GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(ctx.Guild.Id);

            if (!isCommand && !isBot && !isCodeBlock)
            {
                foreach (string word in message)
                {
                    if (
                    string.IsNullOrWhiteSpace(word) ||
                    word.Contains("https://") ||
                    word.Length > 18 ||
                    word.Contains(":") ||
                    word.Contains("😢") ||
                    word.Contains("👍") ||
                    word.Contains("👎") ||
                    word.Contains("😀") ||
                    word.Contains("😃") ||
                    word.Contains("😄") ||
                    word.Contains("😁") ||
                    word.Contains("😆") ||
                    word.Contains("😅") ||
                    word.Contains("😂") ||
                    word.StartsWith("!") ||
                    word.StartsWith("d!") ||
                    word.StartsWith(">") ||
                    !guildPreferences.SpellingEnabled
                    )
                    { return; }

                    var dictionary = WordList.CreateFromFiles(@"Resources/en_au.dic");
                    var boganDictionary = WordList.CreateFromFiles(@"Resources/badwords.dic");
                    string cleanedWord = Regex.Replace(word.ToLower(), @"[^\w\s]", "");
                    bool notOk = dictionary.Check(cleanedWord);
                    bool Bogan = boganDictionary.Check(cleanedWord);

                    if (notOk == false)
                    {
                        errorCount += 1;
                        Console.WriteLine(cleanedWord);
                        sb.Add(cleanedWord);
                        await File.AppendAllTextAsync(@"Resources/missspelledwordsall.txt", $"\n{cleanedWord} - {word}");

                        var wrongDictionary = WordList.CreateFromFiles(@"Resources/missspelledwords.dic");
                        bool alreadyPresent = wrongDictionary.Check(cleanedWord);

                        if (alreadyPresent == false)
                        {
                            string dicFilePath = @"Resources/missspelledwords.dic";
                            await File.AppendAllTextAsync(dicFilePath, $"\n{cleanedWord}");
                        }

                        if (Bogan == true)
                        {
                            boganCount += 1;
                        }
                    }
                    else
                    {
                        correctCount += 1;

                        if (Bogan == true)
                        {
                            boganCount += 1;
                        }
                    }
                }

                Console.WriteLine($"{ctx.Message.Author.Username} sent a message in the {ctx.Guild.Name} guild. spelling stats:\nCorrect: {correctCount}\tIncorrect: {errorCount}\tBogan words: {boganCount}");

                Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

                TextProcessorViewModel viewModel = await _textProcessorService.ProcessTextAsync(ctx.Member.Id, ctx.Guild.Id, correctCount, errorCount, boganCount, 1, guildPreferences.ErrorListLength, sb).ConfigureAwait(false);

                if (!viewModel.LevelledUp) { return; }

                var json = string.Empty;

                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
                int goldperlevelup = configJson.goldperlevelup;

                await _goldService.TransferGold(ctx.Client.CurrentUser.Id, ctx.Member.Id, ctx.Guild.Id ,goldperlevelup, true);

                if (profile.QuietMode == true) { return; }
                
                var levelUpEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{ctx.Member.DisplayName} Is Now Level {viewModel.Profile.Level}",
                    ThumbnailUrl = ctx.Member.AvatarUrl,
                    Color = ctx.Member.Color
                };

                if (goldperlevelup > 0)
                    levelUpEmbed.WithDescription($"+ {goldperlevelup} :moneybag:");

                levelUpEmbed.WithFooter("`w!profile togglenotifications` to hide In future.");

                await ctx.Channel.SendMessageAsync(embed: levelUpEmbed).ConfigureAwait(false);
            }
        }

        //########### Tasks Below ################

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
            profileEmbed.AddField("Boganness", profile.Boganometer.ToString() + " %", true);
            if (profile.LeaveCount > 0)
            {
                profileEmbed.AddField("Leave count", profile.LeaveCount.ToString(), true);
            }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed);
        }
    }
}
    
