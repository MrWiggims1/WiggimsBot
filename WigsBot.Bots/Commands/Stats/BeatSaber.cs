using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.Profiles;
using WigsBot.Core.ViewModels;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bot.Handlers.Dialogue;
using DSharpPlus.CommandsNext.Exceptions;

namespace WigsBot.Bot.Commands.Stats
{
    [Group("Beatsaber")]
    [RequirePrefixes("w!", "W!")]
    [Aliases("bs")]
    [Description("Group of commands that shows beatsaber Stats.")]
    [Cooldown(5, 600, CooldownBucketType.User)]
    public class Beatsaber : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IBeatSaberIdService _beatSaberIdService;
        private readonly ISteamIdService _steamIdService;

        public Beatsaber(
            IProfileService profileService,
            IBeatSaberIdService beatSaberIdService,
            ISteamIdService steamIdService
        )

        {
            _profileService = profileService;
            _beatSaberIdService = beatSaberIdService;
            _steamIdService = steamIdService;
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        [Command("Beatsaber")]
        [Description("Provides Stats for Beatsaber. Ping a user or defaults to yourself.")]
        [Aliases("bs")]
        public async Task beatsaber(CommandContext ctx)
        {
            await bsStats(ctx, ctx.Member.Id, true).ConfigureAwait(false);
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        [Command("BeatSaber")]
        public async Task beatsaber(CommandContext ctx, [Description("Discord member")] DiscordMember member)
        {
            if (ctx.Member.Id == member.Id)
            {
                await bsStats(ctx, member.Id, true).ConfigureAwait(false);
            }
            else
            {
                await bsStats(ctx, member.Id, false).ConfigureAwait(false);
            };
        }

        [Command("recent")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows most recently played songs.")]
        public async Task recent(CommandContext ctx)
        {
            await BsScores(ctx, ctx.Member.Id, true, "recent").ConfigureAwait(false);
        }

        [Command("recent")]
        [RequirePrefixes("w!", "W!")]
        public async Task recent(CommandContext ctx, [Description("Discord member")] DiscordMember member)
        {
            if (ctx.Member.Id == member.Id)
            {
                await BsScores(ctx, member.Id, true, "recent").ConfigureAwait(false);
            }
            else
            {
                await BsScores(ctx, member.Id, false, "recent").ConfigureAwait(false);
            };
        }

        [Command("Top")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows top played songs.")]
        public async Task top(CommandContext ctx)
        {
            await BsScores(ctx, ctx.Member.Id, true, "top").ConfigureAwait(false);
        }

        [Command("top")]
        [RequirePrefixes("w!", "W!")]
        public async Task top(CommandContext ctx, [Description("Discord member")] DiscordMember member)
        {
            if (ctx.Member.Id == member.Id)
            {
                await BsScores(ctx, member.Id, true, "top").ConfigureAwait(false);
            }
            else
            {
                await BsScores(ctx, member.Id, false, "top").ConfigureAwait(false);
            };
        }

        private async Task BsScores(CommandContext ctx, ulong memberId, bool userSelf, string recentOrTop)
        {
            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            long bsId = profile.BeatSaberId;

            if (bsId == 0L)
            {
                if (userSelf == false)
                {
                    await ctx.Channel.SendMessageAsync("This user has not yet set their BeatsaberId, They must do this them self and cannot be done by others.").ConfigureAwait(false);
                    return;
                }
                await ValidateOrSetId(ctx, ctx.Member.Id);
                return;
            }

            long validbsId = profile.BeatSaberId;

            string urlApi = $"https://new.scoresaber.com/api/player/{ validbsId }/scores/{ recentOrTop }";

            string urlApi2 = $"https://new.scoresaber.com/api/player/{ validbsId }/full";

            try
            {
                await Getrecentplays(ctx, urlApi, urlApi2, userSelf, member, recentOrTop);
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("It seems that there has been an error, please make sure your Beatsaber Id is correct (Should be 17 digit number).\nPlease keep in mind that if you have been inactive for an extended period of time in beatsaber No stats will be present for you and therefore will not work. Also this command relies on a non-official API and only works if you have played on a modded version of the game.").ConfigureAwait(false);
                await ValidateOrSetId(ctx, ctx.Member.Id);
            }
        }

        private async Task bsStats(CommandContext ctx, ulong memberId, bool userSelf)
        {
            await ctx.TriggerTypingAsync();

            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            long bsId = profile.BeatSaberId;

            if (bsId == 0L)
            {
                if (userSelf == false)
                {
                    await ctx.Channel.SendMessageAsync("This user has not yet set their BeatsaberId, They must do this them self and cannot be done by others.").ConfigureAwait(false);
                    return;
                }
                await ValidateOrSetId(ctx, ctx.Member.Id);
                return;
            }

            long validbsId = profile.BeatSaberId;

            string urlApi = $"https://new.scoresaber.com/api/player/{ validbsId }/full";

            try
            {
                await GetBasicInfo(ctx, urlApi, userSelf, member);
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("It seems that there has been an error, please make sure your Beatsaber Id is correct (Should be 17 digit number).\nPlease keep in mind that if you have been inactive for an extended period of time in beatsaber No stats will be present for you and therefore will not work. Also this command relies on a non-official API and only works if you have played on a modded version of the game.").ConfigureAwait(false);
                await ValidateOrSetId(ctx, ctx.Member.Id);
            }
        }

        private async Task SetBsId(CommandContext ctx, ulong memberId, long bsId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            await _beatSaberIdService.SetBeatSaberIdAsync(memberId, ctx.Guild.Id, bsId).ConfigureAwait(false);
        }

        private async Task ValidateOrSetId(CommandContext ctx, ulong memberId)
        {
            var SetBsIdStep = new TextStep("To find your Beatsaber Id either follow this URL replacing 'username' With your username https://scoresaber.com/global?search=username then open your profile and get your Id from the URL e.g. '76561198169909801'. Or user your steam Id if you are a steam user get your steam ID using this [website](https://steamidfinder.com/lookup/) and use the steamID64.", null);

            string newBsId = string.Empty;
            SetBsIdStep.onValidResult += (result) => newBsId = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                SetBsIdStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            try
            {
                long newSteamIdLong = long.Parse(newBsId);
                await SetBsId(ctx, ctx.Member.Id, newSteamIdLong);

                System.Threading.Thread.Sleep(500);
                await ctx.Channel.SendMessageAsync("Beatsaber Id Set! try using the command now.").ConfigureAwait(false);
                return;
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("That is not a valid Beatsaber Id.").ConfigureAwait(false);
                return;
            }
        }

        public async Task GetBasicInfo(CommandContext ctx, string urlApi, bool userSelf, DiscordMember member)
        {
            jsonBeatSaber json = GetJsonStringBsStats(urlApi);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Beatsaber stats for {json.playerInfo.name}",
                Description = $"Beatsaber Id: {json.playerInfo.playerid}\n[Score Saber Website](https://scoresaber.com/u/{ json.playerInfo.playerid }\n\nFor more stats on Beatsaber try w!beatsaber Top, or w!beatsaber Recent)",
                ThumbnailUrl = $"https://new.scoresaber.com/{ json.playerInfo.avatar }",
                Color = DiscordColor.Orange
            };

            if (userSelf == true) { embed.WithFooter("If you wish to change your Beatsaber Id please click the gear emoji down below. (will disappear after 60 Seconds)"); }

            embed.AddField(
                "Rank Status:",
                $"PP: {json.playerInfo.pp}\n" +
                $"Global Rank: {json.playerInfo.rank}\n" +
                $"Country rank in {json.playerInfo.country}: {json.playerInfo.countryRank}\n" +
                $"Average ranked accuracy: {json.scoreStats.averageRankedAccuracy}"
                , true);

            embed.AddField(
                "Score Stats",
                $"Total score: {json.scoreStats.totalScore}\n" +
                $"Total play count: {json.scoreStats.totalPlayCount}\n" +
                $"Total ranked score: {json.scoreStats.totalRankedScore}\n" +
                $"Ranked play count: {json.scoreStats.rankedPlayCount}"
                , true);

            await ctx.Message.DeleteAsync("cleanup");
            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);

            if (userSelf == true)
            {
                DiscordEmoji gearEmoji = DiscordEmoji.FromName(ctx.Client, ":gear:");
                await embedMessage.CreateReactionAsync(gearEmoji).ConfigureAwait(false);

                var interactivity = ctx.Client.GetInteractivity();

                try
                {
                    var ReactionResult = await interactivity.WaitForReactionAsync(
                        x => x.Message == embedMessage &&
                    x.User == ctx.User &&
                    (x.Emoji == gearEmoji)).ConfigureAwait(false);

                    if (ReactionResult.Result.Emoji == gearEmoji)
                    {
                        await embedMessage.DeleteAsync(null);
                        await ValidateOrSetId(ctx, ctx.Member.Id);
                        return;
                    }
                }
                catch
                {
                    await embedMessage.DeleteAllReactionsAsync(null);
                }
            }
        }

        public async Task Getrecentplays(CommandContext ctx, string urlApi, string urlApi2, bool userSelf, DiscordMember member, string recentOrTop)
        {
            jsonScores json = GetJsonStringBsScores(urlApi);
            jsonBeatSaber jsonUser = GetJsonStringBsStats(urlApi2);
            
            var embed = new DiscordEmbedBuilder
            {
                Description = $"Beatsaber Id: {jsonUser.playerInfo.playerid}\n[Score Saber Website](https://scoresaber.com/u/{ jsonUser.playerInfo.playerid })",
                ThumbnailUrl = $"https://new.scoresaber.com/{ jsonUser.playerInfo.avatar }",
                Color = DiscordColor.Orange
            };

            if (recentOrTop == "recent") { embed.WithTitle($"Most recent Beatsaber songs played for {jsonUser.playerInfo.name}"); }
            if (recentOrTop == "top") { embed.WithTitle($"Top Beatsaber songs played for {jsonUser.playerInfo.name}"); }

            foreach (scores song in json.scores)
            {
                embed.AddField(
                    $"{song.songAuthorName} - {song.name}\nMapped by {song.levelAuthorName}", 
                    $"Score: {song.score}\n" +
                    $"PP: {song.pp}\n" +
                    $"Rank: {song.rank}\n" +
                    $"Difficulty: {song.diff}");
            }

            if (userSelf == true) { embed.WithFooter("If you wish to change your Beatsaber Id please click the gear emoji down below. (will disappear after 60 Seconds)"); }

            await ctx.Message.DeleteAsync("cleanup");
            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);

            if (userSelf == true)
            {
                DiscordEmoji gearEmoji = DiscordEmoji.FromName(ctx.Client, ":gear:");
                await embedMessage.CreateReactionAsync(gearEmoji).ConfigureAwait(false);

                var interactivity = ctx.Client.GetInteractivity();

                try
                {
                    var ReactionResult = await interactivity.WaitForReactionAsync(
                        x => x.Message == embedMessage &&
                    x.User == ctx.User &&
                    (x.Emoji == gearEmoji)).ConfigureAwait(false);

                    if (ReactionResult.Result.Emoji == gearEmoji)
                    {
                        await embedMessage.DeleteAsync(null);
                        await ValidateOrSetId(ctx, ctx.Member.Id);
                        return;
                    }
                }
                catch
                {
                    await embedMessage.DeleteAllReactionsAsync(null);
                }
            }
        }

        public jsonBeatSaber GetJsonStringBsStats(string BeatsaberApi)
        {
            WebRequest requestObjGet = WebRequest.Create(BeatsaberApi);
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

            jsonBeatSaber Json = JsonConvert.DeserializeObject<jsonBeatSaber>(stringresulttest);

            return Json;
        }

        public jsonScores GetJsonStringBsScores(string BeatsaberApi)
        {
            WebRequest requestObjGet = WebRequest.Create(BeatsaberApi);
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

            jsonScores Json = JsonConvert.DeserializeObject<jsonScores>(stringresulttest);

            return Json;
        }

        public class jsonBeatSaber
        {
            public playerInfo playerInfo { get; set; }
            public scoreStats scoreStats { get; set; }
        }

        public class playerInfo
        {
            public long playerid { get; set; }
            public double pp { get; set; }
            public string name { get; set; }
            public string country { get; set; }
            public string avatar { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
        }

        public class scoreStats
        {
            public int totalScore { get; set; }
            public int totalRankedScore { get; set; }
            public double averageRankedAccuracy { get; set; }
            public int totalPlayCount { get; set; }
            public int rankedPlayCount { get; set; }
        }

        public class jsonScores
        {
            public List<scores> scores { get; set;}
        }
        
        public class scores
        {
            public int score { get; set; }
            public double pp { get; set; }
            public string name { get; set; }
            public string songAuthorName { get; set; }
            public string levelAuthorName { get; set; }
            public string diff { get; set; }
            public string rank { get; set; }
        }
    }
}

