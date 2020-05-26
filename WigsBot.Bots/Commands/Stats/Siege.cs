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
using WigsBot.Bot.Handlers.Dialogue;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Core.ViewModels;

namespace WigsBot.Bot.Commands.Stats
{
    [Group("Siege")]
    [RequirePrefixes("w!", "W!")]
    [Cooldown(10, 300, CooldownBucketType.Guild)]
    [Aliases("Rainbow6", "r6")]
    [Description("Shows Stats for Rainbow 6 siege.")]
    public class Siege : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IUplayIdService _uplayIdService;

        public Siege(
            IProfileService profileService,
            IUplayIdService uplayIdService
        )

        {
            _profileService = profileService;
            _uplayIdService = uplayIdService;
        }

        // Gets generic stats
        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task General(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, ctx.Member.Id, true);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Siege stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("General Stats:",
                $"Matches played: {json.Stats.generalpvp_matchlost + json.Stats.generalpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.generalpvp_matchwon} / {json.Stats.generalpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.generalpvp_kills} / {json.Stats.generalpvp_death} - headshot: {json.Stats.generalpvp_hsrate}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.generalpvp_timeplayed).TotalHours, 1)} hours\n");

            embed.AddField("Casual Stats:",
                $"Matches played: {json.Stats.casualpvp_matchlost + json.Stats.casualpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.casualpvp_matchwon} / {json.Stats.casualpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.casualpvp_kills} / {json.Stats.casualpvp_death}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.casualpvp_timeplayed).TotalHours, 1)} hours\n"
                , true);

            embed.AddField("Ranked Stats:",
                $"Matches played: {json.Stats.rankedpvp_matchlost + json.Stats.rankedpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.rankedpvp_matchwon} / {json.Stats.rankedpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.rankedpvp_kills} / {json.Stats.rankedpvp_death}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.rankedpvp_timeplayed).TotalHours, 1)} hours\n" +
                $"Max rank: {json.ranked.AS_maxrankname} - {json.ranked.AS_maxmmr} MMR\n" +
                $"current rank: {json.ranked.AS_rankname} - {json.ranked.AS_mmr} MMR\n"
                , true);

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        //for other user
        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task General(CommandContext ctx, [Description("ping or use Id")] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, member.Id, false);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Siege stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("General Stats:",
                $"Matches played: {json.Stats.generalpvp_matchlost + json.Stats.generalpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.generalpvp_matchwon} / {json.Stats.generalpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.generalpvp_kills} / {json.Stats.generalpvp_death} - headshot: {json.Stats.generalpvp_hsrate}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.generalpvp_timeplayed).TotalHours, 1)} hours\n");

            embed.AddField("Casual Stats:",
                $"Matches played: {json.Stats.casualpvp_matchlost + json.Stats.casualpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.casualpvp_matchwon} / {json.Stats.casualpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.casualpvp_kills} / {json.Stats.casualpvp_death}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.casualpvp_timeplayed).TotalHours, 1)} hours\n"
                , true);

            embed.AddField("Ranked Stats:",
                $"Matches played: {json.Stats.rankedpvp_matchlost + json.Stats.rankedpvp_matchwon}\n" +
                $"Win/Loss: {json.Stats.rankedpvp_matchwon} / {json.Stats.rankedpvp_matchlost}\n" +
                $"Kill/Deaths: {json.Stats.rankedpvp_kills} / {json.Stats.rankedpvp_death}\n" +
                $"Time Played: {Math.Round(TimeSpan.FromSeconds(json.Stats.rankedpvp_timeplayed).TotalHours, 1)} hours\n" +
                $"Max rank: {json.ranked.AS_maxrankname} - {json.ranked.AS_maxmmr} MMR\n" +
                $"current rank: {json.ranked.AS_rankname} - {json.ranked.AS_mmr} MMR\n"
                , true);

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        //gets season stats by season no
        [Command("season")]
        [RequirePrefixes("w!", "W!")]
        [Description("Get ranked stats from a specific season back to operation health (season 6)")]
        [Aliases("operation")]
        public async Task seasonStats(CommandContext ctx, [Description("Season number")] int season)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, ctx.Member.Id, true);

            if (!json.seasons.ContainsKey(season))
            {
                await ctx.Channel.SendMessageAsync("That season number does not exists");
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Season {season} (operation {json.seasons[season].seasonname}) stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Ranked stats:",
                $"Max rank: {json.seasons[season].maxrankname}\n" +
                $"Matches played: {json.seasons[season].AS_losses + json.seasons[season].AS_wins}\n" +
                $"Win/Loss: {json.seasons[season].AS_wins} / {json.seasons[season].AS_losses}\n" +
                $"Kill/Deaths: {json.seasons[season].AS_kills} / {json.seasons[season].AS_deaths}\n");

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // by operatrion name
        [Command("season")]
        [RequirePrefixes("w!", "W!")]
        public async Task seasonStats(CommandContext ctx, [RemainingText, Description("Operation name")] string seasonName)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, ctx.Member.Id, true);
            int Key = 0;

            try
            {
                Key = json.seasons.FirstOrDefault(x => x.Value.seasonname.ToLower() == seasonName.ToLower()).Key;
            }
            catch { await ctx.Channel.SendMessageAsync("This season does not exist , or came before operation health."); return; }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Season {Key} (operation {json.seasons[Key].seasonname}) stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Ranked stats:",
                $"Max rank: {json.seasons[Key].maxrankname}\n" +
                $"Matches played: {json.seasons[Key].AS_losses + json.seasons[Key].AS_wins}\n" +
                $"Win/Loss: {json.seasons[Key].AS_wins} / {json.seasons[Key].AS_losses}\n" +
                $"Kill/Deaths: {json.seasons[Key].AS_kills} / {json.seasons[Key].AS_deaths}\n");

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // by season no for other user
        [Command("season")]
        [RequirePrefixes("w!", "W!")]
        public async Task seasonStats(CommandContext ctx, [Description("ping or use Id")] DiscordMember member, [Description("Season number")] int season)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, member.Id, false);

            if (!json.seasons.ContainsKey(season))
            {
                await ctx.Channel.SendMessageAsync("That season number does not exists");
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Season {season} (operation {json.seasons[season].seasonname}) stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Ranked stats:",
                $"Max rank: {json.seasons[season].maxrankname}\n" +
                $"Matches played: {json.seasons[season].AS_losses + json.seasons[season].AS_wins}\n" +
                $"Win/Loss: {json.seasons[season].AS_wins} / {json.seasons[season].AS_losses}\n" +
                $"Kill/Deaths: {json.seasons[season].AS_kills} / {json.seasons[season].AS_deaths}\n");

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // by operation name for other user
        [Command("season")]
        [RequirePrefixes("w!", "W!")]
        public async Task seasonStats(CommandContext ctx, [Description("ping or use Id")] DiscordMember member, [RemainingText, Description("Operation name")] string seasonName)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, member.Id, false);
            int Key = 0;

            try
            {
                Key = json.seasons.FirstOrDefault(x => x.Value.seasonname.ToLower() == seasonName.ToLower()).Key;
            }
            catch { await ctx.Channel.SendMessageAsync("This season does not exist , or came before operation health."); return; }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Season {Key} (operation {json.seasons[Key].seasonname}) stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Ranked stats:",
                $"Max rank: {json.seasons[Key].maxrankname}\n" +
                $"Matches played: {json.seasons[Key].AS_losses + json.seasons[Key].AS_wins}\n" +
                $"Win/Loss: {json.seasons[Key].AS_wins} / {json.seasons[Key].AS_losses}\n" +
                $"Kill/Deaths: {json.seasons[Key].AS_kills} / {json.seasons[Key].AS_deaths}\n");

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // gets operator stats by name
        [Command("operator")]
        [RequirePrefixes("w!", "W!")]
        [Description("Gets stats for a specific operator in siege.")]
        [Aliases("op")]
        public async Task operatorStats(CommandContext ctx, [RemainingText, Description("Operator name `w!siege op list` to get operator names.")] string opName)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, ctx.Member.Id, true);

            siegeOp op = null;
            try
            {

                op = json.operators.First(x => x.Key.ToLower() == opName.ToLower()).Value;
            }
            catch
            {
                StringBuilder sb = new StringBuilder();
                foreach (string opname in json.operators.Keys)
                {
                    sb.Append($"{opname}, ");
                }

                await ctx.Channel.SendMessageAsync($"That operator was not found, below is a list of the operator names:\n{sb.ToString()}");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{opName.ToLower()} stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Overall Stats:", $"Win/Loss: {op.overall.wins} / {op.overall.losses}\nKills/Deaths: {op.overall.kills} / {op.overall.deaths}\nTime played: {Math.Round(TimeSpan.FromSeconds(op.overall.timeplayed).TotalHours, 1)} hours", true);

            embed.AddField("Seasonal Stats:", $"Win/Loss: {op.seasonal.wins} / {op.seasonal.losses}\nKills/Deaths: {op.seasonal.kills} / {op.seasonal.deaths}\nTime played: {Math.Round(TimeSpan.FromSeconds(op.seasonal.timeplayed).TotalHours, 1)} hours", true);

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // for other user
        [Command("operator")]
        [RequirePrefixes("w!", "W!")]
        public async Task operatorStats(CommandContext ctx, [Description("ping or use Id")] DiscordMember member, [RemainingText, Description("Operator name `w!siege op list` to get operator names.")] string opName)
        {
            await ctx.TriggerTypingAsync();
            var json = await GetPlayerStats(ctx, member.Id, false);

            siegeOp op = null;
            try
            {

                op = json.operators.First(x => x.Key.ToLower() == opName.ToLower()).Value;
            }
            catch
            {
                StringBuilder sb = new StringBuilder();
                foreach (string opname in json.operators.Keys)
                {
                    sb.Append($"{opname}, ");
                }

                await ctx.Channel.SendMessageAsync($"That operator was not found, below is a list of the operator names:\n{sb.ToString()}");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{opName.ToLower()} stats for {json.player.p_name}: level {json.Stats.level} on {json.player.p_platform}",
                Description = $"[Stats by R6Tab](https://r6tab.com/player//{ json.player.p_id })\n`w!help siege` for more options.",
                Color = ctx.Member.Color
            };

            embed.AddField("Overall Stats:", $"Win/Loss: {op.overall.wins} / {op.overall.losses}\nKills/Deaths: {op.overall.kills} / {op.overall.deaths}\nTime played: {Math.Round(TimeSpan.FromSeconds(op.overall.timeplayed).TotalHours, 1)} hours", true);

            embed.AddField("Seasonal Stats:", $"Win/Loss: {op.seasonal.wins} / {op.seasonal.losses}\nKills/Deaths: {op.seasonal.kills} / {op.seasonal.deaths}\nTime played: {Math.Round(TimeSpan.FromSeconds(op.seasonal.timeplayed).TotalHours, 1)} hours", true);

            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        // tasks

        public async Task<bool> SetId(CommandContext ctx, ulong memberId, bool userSelf)
        {
            if (userSelf == false)
            {
                await ctx.Channel.SendMessageAsync("This user has not yet set their Uplay Id, They must do this them self and cannot be done by others.").ConfigureAwait(false);
                return false;
            }

            try
            {
                var SetuplayUserStep = new TextStep("Enter your Uplay username", null);

                string uplayUsername = string.Empty;
                SetuplayUserStep.onValidResult += (result) => uplayUsername = result;

                var inputDialogueHandler = new DialogueHandler(
                    ctx.Client,
                    ctx.Channel,
                    ctx.User,
                    SetuplayUserStep
                );

                bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

                if (!succeeded) { return false; }

                string urlApi = $"https://r6.apitab.com/search/uplay/{ uplayUsername }";

                var json = GetPlayerSearch(urlApi);

                int userChoice = 0;

                if (json.players.Count > 1)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < json.players.Count; ++i)
                        {
                            sb.Append($"[`{i}`] Username: {json.players.Values.ElementAt(i).profile.p_name}\tLevel: {json.players.Values.ElementAt(i).stats.level}\n\n");
                        }

                        var userChoiceStep = new IntStep($"Enter the number of the correct profile from below:\n{sb.ToString()}", null, 0, json.players.Count - 1);

                        userChoiceStep.OnValidResult += (result) => userChoice = result;

                        var secinputDialogueHandler = new DialogueHandler(
                            ctx.Client,
                            ctx.Channel,
                            ctx.User,
                            userChoiceStep
                        );

                        bool secSucceeded = await secinputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

                        if (!secSucceeded) { return false; }
                    }
                    catch { return false; }
                }

                await ctx.TriggerTypingAsync();

                await _uplayIdService.SetUplayIdAsync(ctx.Member.Id, ctx.Guild.Id, json.players.ToList()[userChoice].Value.profile.p_user);

                return true;
            }
            catch { return false; }
        }

        public JsonPlayerSearch GetPlayerSearch(string urlApi)
        {
            WebRequest requestObjGet = WebRequest.Create(urlApi);
            requestObjGet.Method = "GET";
            HttpWebResponse responseObjGet = null;
            responseObjGet = (HttpWebResponse)requestObjGet.GetResponse();

            string stringresulttest = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                stringresulttest = sr.ReadToEnd();
                sr.Close();
            }

            JsonPlayerSearch Json = JsonConvert.DeserializeObject<JsonPlayerSearch>(stringresulttest);

            return Json;
        }

        public async Task<JsonPlayerStats> GetPlayerStats(CommandContext ctx, ulong memberId, bool userSelf)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            var configjsonfile = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                configjsonfile = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(configjsonfile);

            string UplayId = profile.UplayUsername;

            if (UplayId == "not set")
            {
                bool idSet = await SetId(ctx, memberId, userSelf);
                if (!idSet)
                {
                    await ctx.Channel.SendMessageAsync("Your id was not set.").ConfigureAwait(false);
                    return null;
                }
                var json = await GetPlayerStats(ctx, memberId, userSelf);
                return json;
            }

            string urlApi = $" https://r6.apitab.com/player/{ UplayId }&cid={configJson.r6tabtoken}";

            WebRequest requestObjGet = WebRequest.Create(urlApi);
            requestObjGet.Method = "GET";
            HttpWebResponse responseObjGet = (HttpWebResponse)requestObjGet.GetResponse();

            string stringresulttest = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                stringresulttest = sr.ReadToEnd();
                sr.Close();
            }

            JsonPlayerStats Json = JsonConvert.DeserializeObject<JsonPlayerStats>(stringresulttest);

            return Json;
        }

        public async Task AwaitReaction(CommandContext ctx, DiscordMessage message, Profile profile, DiscordMember member, List<DiscordEmoji> emojis)
        {
            try
            {
                var interactivity = ctx.Client.GetInteractivity();

                var ReactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == message &&
                    x.User == ctx.User &&
                    (emojis.Contains(x.Emoji))).ConfigureAwait(false);

                foreach (var emoji in emojis)
                {
                    await message.CreateReactionAsync(emoji);
                }

                if (ReactionResult.Result.Emoji == DiscordEmoji.FromName(ctx.Client, ":emoji:")) { /* task */ }
            }
            catch
            { await message.DeleteAllReactionsAsync(); }
        }

        public class JsonPlayerSearch
        {
            public bool foundmatch { get; set; }
            public Dictionary<String, users>  players { get; set; }
        }

        public class users
        {
            public profile profile { get; set; }
            public userRanked ranked { get; set; }
            public userStats stats { get; set; }
        }

        public class profile
        {
            public string p_name { get; set; }
            public string p_user { get; set; }
        }

        public class userRanked
        {
            public string kd { get; set; }
            //public int mmr { get; set; }
            //public int rank { get; set; }
        }

        public class userStats
        {
            public int level { get; set; }
        }

        public class JsonPlayerStats
        {
            public bool found { get; set; }
            public player player { get; set; }
            public stats Stats { get; set; }
            public ranked ranked { get; set; }
            public Dictionary<string, siegeOp> operators { get; set; }
            public Dictionary<int, season> seasons { get; set; }
        }

        public class player
        {
            public string p_id { get; set; }
            public string p_user { get; set; }
            public string p_platform { get; set; }
            public string p_name { get; set; }
        }

        public class stats
        {
            public int level { get; set; }
            public int generalpvp_headshot { get; set; }
            public int generalpvp_kills { get; set; }
            public int generalpvp_death { get; set; }
            public string generalpvp_kd { get; set; }
            public string generalpvp_hsrate { get; set; }
            public int generalpvp_killassists { get; set; }
            public int generalpvp_meleekills { get; set; }
            public int generalpvp_revive { get; set; }
            public int generalpvp_penetrationkills { get; set; }
            public int generalpvp_matchwon { get; set; }
            public int generalpvp_matchlost { get; set; }
            public string generalpvp_wl { get; set; }
            public int generalpvp_timeplayed { get; set; }

            public int casualpvp_kills { get; set; }
            public int casualpvp_death { get; set; }
            public string casualpvp_kd { get; set; }
            public int casualpvp_matchwon { get; set; }
            public int casualpvp_matchlost { get; set; }
            public string casualpvp_wl { get; set; }
            public int casualpvp_timeplayed { get; set; }

            public int rankedpvp_kills { get; set; }
            public int rankedpvp_death { get; set; }
            public string rankedpvp_kd { get; set; }
            public int rankedpvp_matchwon { get; set; }
            public int rankedpvp_matchlost { get; set; }
            public string rankedpvp_wl { get; set; }
            public int rankedpvp_timeplayed { get; set; }

            public int plantbombpvp_matchwon { get; set; }
            public int plantbombpvp_matchlost { get; set; }
            public string plantbombpvp_wl { get; set; }
            public int secureareapvp_matchwon { get; set; }
            public int secureareapvp_matchlost { get; set; }
            public string secureareapvp_wl { get; set; }
            public int rescuehostagepvp_matchwon { get; set; }
            public int rescuehostagepvp_matchlost { get; set; }
            public string rescuehostagepvp_wl { get; set; }
        }

        public class ranked
        {
            // rando stats
            public int allkills { get; set; }
            public int alldeaths { get; set; }
            public int allwins { get; set; }
            public int alllosses { get; set; }
            public int allabandons { get; set; }
            public string allkd { get; set; }
            public string allwl { get; set; }
            public int killpermatch { get; set; }
            public int deathspermatch { get; set; }

            // top region stats
            public int mmr { get; set; }
            public int maxmmr { get; set; }
            public string kd { get; set; }
            public int rank { get; set; }
            public string rankname { get; set; }
            public int maxrank { get; set; }
            public string maxrankname { get; set; }
            public int champ { get; set; }
            public string topregion { get; set; }

            // Asia ranked stats
            public int AS_kills { get; set; }
            public int AS_deaths { get; set; }
            public int AS_wins { get; set; }
            public int AS_losses { get; set; }
            public int AS_abandons { get; set; }
            public int AS_mmr { get; set; }
            public int AS_maxmmr { get; set; }
            public int AS_champ { get; set; }
            public int AS_mmrchange { get; set; }
            public int AS_actualmmr { get; set; }
            public int AS_matches { get; set; }
            public string AS_wl { get; set; }
            public string AS_kd { get; set; }
            public int AS_rank { get; set; }
            public string AS_rankname { get; set; }
            public int AS_maxrank { get; set; }
            public string AS_maxrankname { get; set; }
            public int AS_killspermatch { get; set; }
            public int AS_deathspermatch { get; set; }

            // NA ranked stats
            public int NA_kills { get; set; }
            public int NA_deaths { get; set; }
            public int NA_wins { get; set; }
            public int NA_losses { get; set; }
            public int NA_abandons { get; set; }
            public int NA_mmr { get; set; }
            public int NA_maxmmr { get; set; }
            public int NA_champ { get; set; }
            public int NA_mmrchange { get; set; }
            public int NA_actualmmr { get; set; }
            public int NA_matches { get; set; }
            public string NA_wl { get; set; }
            public string NA_kd { get; set; }
            public int NA_rank { get; set; }
            public string NA_rankname { get; set; }
            public int NA_maxrank { get; set; }
            public string NA_maxrankname { get; set; }
            public int NA_killspermatch { get; set; }
            public int NA_deathspermatch { get; set; }

            // EU ranked stats
            public int EU_kills { get; set; }
            public int EU_deaths { get; set; }
            public int EU_wins { get; set; }
            public int EU_losses { get; set; }
            public int EU_abandons { get; set; }
            public int EU_mmr { get; set; }
            public int EU_maxmmr { get; set; }
            public int EU_champ { get; set; }
            public int EU_mmrchange { get; set; }
            public int EU_actualmmr { get; set; }
            public int EU_matches { get; set; }
            public string EU_wl { get; set; }
            public string EU_kd { get; set; }
            public int EU_rank { get; set; }
            public string EU_rankname { get; set; }
            public int EU_maxrank { get; set; }
            public string EU_maxrankname { get; set; }
            public int EU_killspermatch { get; set; }
            public int EU_deathspermatch { get; set; }
        }

        public class siegeOp
        {
            public string id { get; set; }
            public string type { get; set; }
            public overall overall { get; set; }
            public seasonal seasonal { get; set; }
        }

        public class overall
        {
            public int wins { get; set; }
            public int losses { get; set; }
            public int kills { get; set; }
            public int deaths { get; set; }
            public int timeplayed { get; set; }
            public string kd { get; set; }
            public int winrate { get; set; }
        }

        public class seasonal
        {
            public int wins { get; set; }
            public int losses { get; set; }
            public int kills { get; set; }
            public int deaths { get; set; }
            public int timeplayed { get; set; }
            public string kd { get; set; }
            public int winrate { get; set; }
        }

        public class season
        {
            public string seasonname { get; set; }
            public int AS_mmr { get; set; }
            public int AS_wins { get; set; }
            public int AS_losses { get; set; }
            public int AS_abandons { get; set; }
            public int AS_kills { get; set; }
            public int AS_deaths { get; set; }
            //public int maxmmr { get; set; }
            public string maxrankname { get; set; }
        }
    }
}