using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WeCantSpell.Hunspell;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Net;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using System.Collections.Generic;
using DSharpPlus.Interactivity;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.Net.Models;

namespace WigsBot.Bot.Commands
{
    public class tools : BaseCommandModule
    {
        private readonly ITextProcessorService _textProcessorService;
        private readonly IProfileService _profileService;

        public tools(
            ITextProcessorService textProcessorService,
            IProfileService profileService)
        {
            _textProcessorService = textProcessorService;
            _profileService = profileService;
        }

        [Command("cancel")]
        [Hidden]
        public async Task Cancel(CommandContext ctx)
        {
            return;
        }

        [Command("ping")]
        [RequirePrefixes("w!", "W!")]
        [Description("Try it and find out.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("coin")]
        [RequirePrefixes("w!", "W!")]
        [Description("Flip a coin.")]
        public async Task coin(CommandContext ctx)
        {
            var random = new Random();
            int randomNumber = random.Next(0, 2);

            Console.WriteLine(randomNumber);

            if (randomNumber == 1)
            {
                await ctx.Channel.SendMessageAsync("The coin landed on heads!").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("The coin landed on tails!").ConfigureAwait(false);
            }
        }

        [Command("dice")]
        [RequirePrefixes("w!", "W!")]
        [Description("Roll a dice with x sides. 6 by default.")]
        [Aliases("d")]
        public async Task dice(CommandContext ctx, [Description("How many sides does your dice have?")]int dieSides = 6)
        {
            if (dieSides <= 2)
            {
                var random = new Random();
                int dieResults = random.Next(0, 6);
                await ctx.Channel.SendMessageAsync($"you can't really have a {dieSides} sided die, so we rolled a 6 sided one and it landed on {dieResults + 1}!").ConfigureAwait(false);
            }
            else
            {
                var random = new Random();
                int dieResults = random.Next(0, dieSides);
                await ctx.Channel.SendMessageAsync($":game_die: Your dice landed on {dieResults + 1}!").ConfigureAwait(false);
            }
        }

        [Command("deafen")]
        [Description("Donno how you found this but you did.")]
        [Hidden]
        public async Task deafen(CommandContext ctx)
        {
            await ctx.Member.SetDeafAsync(true, "For being an idiot who pings everyone.");
            await ctx.Channel.SendMessageAsync("For being an idiot and @ing everyone ya been server muted for 10 minutes.").ConfigureAwait(false);
            System.Threading.Thread.Sleep(600000);
            await ctx.Member.SetDeafAsync(false, "Time outs over");
        }

        [Group("invite")]
        [Description("Various invitation commands.")]
        [Aliases("invitation")]
        public class InviteCommands : BaseCommandModule
        {

            [Command("server")]
            [RequirePrefixes("w!", "W!")]
            [RequirePermissions(Permissions.CreateInstantInvite)]
            [Description("Creates a server invite. ")]
            public async Task serverinvite(CommandContext ctx, [Description("How long will the invite last? (h). Default is 24h")] int duration = 24)
            {
                var ServerInvite = await ctx.Channel.CreateInviteAsync(duration * 3600, 0, false, false, $"Created by {ctx.Message.Author.Username}");

                await ctx.Channel.SendMessageAsync($"This invite will expire in {ServerInvite.MaxAge / 3600} hours https://discord.gg/{ ServerInvite.Code }").ConfigureAwait(false);
            }

            [Command("bot")]
            [RequirePrefixes("w!", "W!")]
            [Description("Gets the link to invite the bot to another server (keep in mind this bot is developed to run on a specific server, not all commands will be available.)")]
            public async Task BotInvite(CommandContext ctx)
            {
                await ctx.RespondAsync("https://discordapp.com/oauth2/authorize?client_id=629962329655607308&scope=bot&permissions=0");
            }
        }

        [Command("Purge")]
        [RequirePrefixes("w@", "W@")]
        [Description("Deletes a large amount of messages at once, this is irreversible and should be used carefully (limited to 100 messages).")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Purge(CommandContext ctx, [Description("How many messages would you like to delete.")] int NumberToDelete)
        {
            if (NumberToDelete <= 0 || NumberToDelete > 100)
            {
                await ctx.Channel.SendMessageAsync("Please specify a positive number between 1 and 100.").ConfigureAwait(false);
                return;
            }

            var messagesToDelete = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, NumberToDelete);

            foreach (DiscordMessage mesg in messagesToDelete)
            { await mesg.DeleteAsync(); }

            await ctx.Message.DeleteAsync($"{NumberToDelete} messages purged by {ctx.Member.Username}");
        }

        [Command("Purge")]
        [RequirePrefixes("w@", "W@")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Purge(CommandContext ctx, [Description("Delete all messages up to this message Id or link.")] DiscordMessage message, [Description("Messages to be excluded."), Optional] params DiscordMessage[] discordMessages)
        {
            var messagesToDelete = await ctx.Channel.GetMessagesAfterAsync(message.Id, 100);
            int deletionCount = 0;
            bool delete;

            List<DiscordMessage> list = new List<DiscordMessage>(messagesToDelete);

            foreach (DiscordMessage mesg in list)
            {
                delete = true;

                if (discordMessages.Length > 0)
                {
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims bot: purge command by message Id with exclusions", $"{message.Id} is being searched", DateTime.Now);

                    foreach (var blackListMessage in discordMessages)
                    {
                        if (blackListMessage == mesg)
                        {
                            delete = false;
                            ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims bot: purge command by message Id with exclusions", $"{message.Id} will not be deleted.", DateTime.Now);
                        }
                    }
                }

                if (delete)
                {
                    await mesg.DeleteAsync();
                    deletionCount += 1;
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims bot: purge command", $"{message.Id} has been deleted.", DateTime.Now);
                    System.Threading.Thread.Sleep(500);
                }
            }

            var responseMessage = await ctx.Channel.SendMessageAsync($"{deletionCount} messages purged by {ctx.Member.Username}");
            System.Threading.Thread.Sleep(5000);
            await responseMessage.DeleteAsync();
        }

        [Command("HEXColour")]
        [RequirePrefixes("w!", "W!")]
        [Description("Enter a hex value to see how it looks.")]
        [Aliases("colour", "Color", "DiscordColour", "DiscordColor", "hex")]
        public async Task colour(CommandContext ctx, DiscordColor hexCode)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Your colour!",
                Color = hexCode,
                Description = $"R: {hexCode.R}\nB: {hexCode.B}\nG: {hexCode.G}"
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("AddToDictionary")]
        [RequirePrefixes("w!", "W!")]
        [Description("Adds a new word to the dictionary, note the word will be converted to lower case and have all special characters removed before being added. DO NOT USE EMOJIS WITH THIS COMMAND!")]
        public async Task AddToDictionary(CommandContext ctx, [Description("Word to be added.")] string word, [Description("retype word to be added.")] string confirmWord)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            if (profile.SpellAcc < 90)
            {
                await ctx.Channel.SendMessageAsync($"Your spelling must be above 90% to add to the dictionary, at the moment you are currently at {profile.SpellAcc}%.");
                return;
            }

            if (confirmWord != word)
            {
                await ctx.Channel.SendMessageAsync($"'{word}' and '{confirmWord}' are not the same.").ConfigureAwait(false);
                return;
            }

            string newWord = Regex.Replace(word, @"[^\w\s]", "").ToLower();
            var dictionary = WordList.CreateFromFiles(@"Resources/en_au.dic");
            bool notOk = dictionary.Check(newWord);

            if (notOk == true)
            {
                await ctx.Channel.SendMessageAsync($"{newWord} is already in the dictionary.").ConfigureAwait(false);
                return;
            }


            string dicFilePath = @"Resources/en_au.dic";
            await File.AppendAllTextAsync(dicFilePath, $"\n{newWord}");

            //
            string spellMistakesDic = "Resources/missspelledwords.dic";
            string[] Lines = File.ReadAllLines(spellMistakesDic);

            File.Delete(spellMistakesDic);

            using (StreamWriter sw = File.AppendText(spellMistakesDic))
            {
                foreach (string line in Lines)
                {
                    if (line.StartsWith(newWord))
                    {
                        continue;
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }

            string logFilePath = "Logs/dictionary.log";
            await File.AppendAllTextAsync(logFilePath, $"\n{DateTime.Now}: {ctx.Member} added a new word to the dictionary '{newWord}'.");
            await ctx.Channel.SendMessageAsync($"{newWord} was added to the dictionary.").ConfigureAwait(false);

            await _textProcessorService.RemoveDictionaryWordFromLists(newWord);
        }

        [Command("removefromdictionary")]
        [RequirePrefixes("w!", "W!")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Removes a word from the dictionary.")]
        [Aliases("rmdic", "rmfromdictionary")]
        public async Task removefromdictionary(CommandContext ctx, [Description("Word to be removed.")] string word)
        {
            string dictionary = @"Resources/en_au.dic";
            string[] Lines = File.ReadAllLines(dictionary);

            File.Delete(dictionary);

            using (StreamWriter sw = File.AppendText(dictionary))
            {
                foreach (string line in Lines)
                {
                    if (line.StartsWith(word))
                    {
                        continue;
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            string logFilePath = "Logs/dictionary.log";
            await File.AppendAllTextAsync(logFilePath, $"\n{DateTime.Now}: {ctx.Member} removed from the dictionary '{word}'.");
            await ctx.RespondAsync($"{word} was removed from the dictionary.");
        }

        [Command("stealemoji")]
        [RequirePrefixes("w!", "W!")]
        [RequireUserPermissions(Permissions.ManageEmojis)]
        [Description("Adds an emoji from another discord server.")]
        public async Task stealemoji(CommandContext ctx, [Description("Emoji you wish to steal"), Optional] DiscordEmoji emoji, [Description("new emojis name")] string emojiName = null)
        {
            if (string.IsNullOrEmpty(emojiName))
            {
                emojiName = emoji.Name;
                ctx.Client.DebugLogger.LogMessage(LogLevel.Debug, "Wiggims Bot: steal emoji", $"No emoji name given, naming emoji {emoji.Name}", DateTime.Now);
            }

            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(emoji.Url);
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.AllowReadStreamBuffering = true;
            webRequest.Timeout = 30000;

            System.Net.WebResponse webResponse = webRequest.GetResponse();

            DiscordEmoji newEmoji = await ctx.Guild.CreateEmojiAsync(emojiName, webResponse.GetResponseStream(), null, $"{ctx.User.Username} stole an emoji.");
            
            await ctx.Channel.SendMessageAsync($"Ayyyy {newEmoji} is now in our server.").ConfigureAwait(false);
        }

        [Group("Timeout")]
        [RequirePrefixes("w@", "W@")]
        [Description("Will give a user a role which will effectively set them in a corner to let them cool off.")]
        public class TimeoutCommands : BaseCommandModule
        {
            [GroupCommand]
            [RequirePrefixes("w@", "W@")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task GiveTimeout(CommandContext ctx, [Description("Mention or use ID")] DiscordMember member, [Description("Timeout duration (s, m, h) default is 10 minutes.")] TimeSpan? timeSpan = null)
            {
                await ctx.Message.DeleteAsync();

                if (timeSpan == null)
                    timeSpan = TimeSpan.FromMinutes(10);

                string filePath = MakeTimeoutJson(member);
                var roles = member.Roles.ToArray();
                DiscordRole timeoutRole = ctx.Guild.GetRole(514405137402429471);

                foreach (var role in roles)
                {
                    await member.RevokeRoleAsync(role);
                }

                await member.GrantRoleAsync(timeoutRole);

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"{member.Username} has been sent to a timeout",
                    Color = DiscordColor.Orange,
                    Description = $"To manually reset the roles of this member for whatever reason type `w@timeout restore {member.Id} {filePath}`"
                };

                await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

                System.Threading.Thread.Sleep(Convert.ToInt32(timeSpan.Value.TotalMilliseconds));

                await member.RevokeRoleAsync(timeoutRole);

                var roleJson = GetTimeoutJson(filePath);

                foreach (var roleId in roleJson.RoleId)
                {
                    DiscordRole role = ctx.Guild.GetRole(roleId);
                    await member.GrantRoleAsync(role);
                }
                await ctx.Channel.SendMessageAsync($"{member.Username}'s timeout has ended.");
            }

            [Command("restore")]
            [RequirePrefixes("w@", "W@")]
            [Description("Manually restores a users role if something goes wrong.")]
            [RequireUserPermissions(Permissions.Administrator)]
            public async Task RestoreTimeoutRoles(CommandContext ctx, DiscordMember member, string TimeoutJson)
            {
                var roleJson = GetTimeoutJson(TimeoutJson);

                foreach (var roleId in roleJson.RoleId)
                {
                    DiscordRole role = ctx.Guild.GetRole(roleId);
                    await member.GrantRoleAsync(role);
                }

                await ctx.Channel.SendMessageAsync($"{member.Username}'s roles have been manually reset.");
            }

             // #### tasks ####

            static string MakeTimeoutJson(DiscordMember member)
            {
                var roles = member.Roles.ToArray();
                var json = new TimeoutJson() { RoleId = new List<ulong>() };
                string fileName = $"Resources/RoleJSONs/{member.Username}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.json";

                if (roles.Length > 0)
                {
                    foreach (var role in roles)
                    {
                        json.RoleId.Add(role.Id);
                    }
                }

                string jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);

                File.WriteAllText($"{fileName}", jsonString);

                return fileName;
            }

            static TimeoutJson GetTimeoutJson(string filePath)
            {
                var json = string.Empty;

                using (var fs = File.OpenRead(filePath))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                TimeoutJson timeoutJson = JsonConvert.DeserializeObject<TimeoutJson>(json);

                return timeoutJson;
            }

            public class TimeoutJson
            {
                public List<ulong> RoleId { get; set; }
            }

        }
    }
}