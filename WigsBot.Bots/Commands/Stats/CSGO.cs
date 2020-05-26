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
using System;

namespace WigsBot.Bot.Commands.Stats
{
    [Group("csgo")]
    [RequirePrefixes("w!", "W!")]
    [Description("Shows CSGO stats.")]
    [Aliases("CS", "counterstrike")]
    [Cooldown(10, 3600, CooldownBucketType.Global)]
    public class csgo : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly ISteamIdService _steamIdService;

        public csgo(
            IProfileService profileService,
            ISteamIdService steamIdService
        )

        {
            _profileService = profileService;
            _steamIdService = steamIdService;
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task Basic(CommandContext ctx)
        {
            string[] stringArray = { 
                "Total_Kills_Headshot",
                "Total_Kills_Enemy_Weapon",
                "Total_Money_Earned",
                "Total_Damage_Done",
                "Total_Time_Played",
                "Total_Deaths",
                "Total_Weapons_Donated",
                "Total_Dominations",
                "Total_Domination_Overkills",
                "Total_Revenges",
                "Total_Shots_Hit",
                "Total_Shots_Fired",
                "Total_Rounds_Played",
                "Total_MVPs",
                "Total_Gun_Game_Rounds_Won",
                "Total_Matches_Won",
                "Total_Matches_Played"};
            string type = "General";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        public async Task Basic(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Total_Kills_Headshot",
                "Total_Kills_Enemy_Weapon",
                "Total_Money_Earned",
                "Total_Damage_Done",
                "Total_Time_Played",
                "Total_Kills",
                "Total_Deaths",
                "Total_Weapons_Donated",
                "Total_Dominations",
                "Total_Domination_Overkills",
                "Total_Revenges",
                "Total_Shots_Hit",
                "Total_Shots_Fired",
                "Total_Rounds_Played",
                "Total_MVPs",
                "Total_Gun_Game_Rounds_Won",
                "Total_Matches_Won",
                "Total_Matches_Played" };
            string type = "General";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("pistol")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO pistol stats.")]
        public async Task pistol(CommandContext ctx)
        {
            string[] stringArray = { "Total_Wins_Pistolround",
                "Total_Kills_Glock",
                "Total_Kills_Deagle",
                "Total_Kills_Fiveseven",
                "Total_Kills_Hkp2000",
                "Total_Kills_P250", 
                "Total_Kills_Tec9" };
            string type = "Pistol";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("pistol")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO pistol stats.")]
        public async Task pistol(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = {
                "Total_Wins_Pistolround",
                "Total_Kills_Glock",
                "Total_Kills_Deagle",
                "Total_Kills_Fiveseven",
                "Total_Kills_Hkp2000", 
                "Total_Kills_P250",
                "Total_Kills_Tec9" };
            string type = "Pistol";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("smg")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO smg stats.")]
        public async Task smg(CommandContext ctx)
        {
            string[] stringArray = { 
                "Total_Kills_Mac10",
                "Total_Kills_UMP45",
                "Total_Kills_P90", 
                "Total_Kills_MP7", 
                "Total_Kills_MP9", 
                "Total_Kills_Bizon" };
            string type = "Smg";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("smg")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO smg stats.")]
        public async Task smg(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Total_Kills_Mac10", 
                "Total_Kills_UMP45", 
                "Total_Kills_P90",
                "Total_Kills_MP7", 
                "Total_Kills_MP9", 
                "Total_Kills_Bizon" };
            string type = "Smg";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("rifle")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO rifle stats.")]
        public async Task rifle(CommandContext ctx)
        {
            string[] stringArray = { 
                "Total_Kills_Awp",
                "Total_Kills_AK47", 
                "Total_Kills_Aug", 
                "Total_Kills_Famas",
                "Total_Kills_G3sg1", 
                "Total_Kills_SG556", 
                "Total_Kills_Scar20",
                "Total_Kills_SSG08", 
                "Total_Kills_M4A1", 
                "Total_Kills_Galilar" };
            string type = "Rifle";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("rifle")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO rifle stats.")]
        public async Task rifle(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = {
                "Total_Kills_Awp", 
                "Total_Kills_AK47", 
                "Total_Kills_Aug", 
                "Total_Kills_Famas", 
                "Total_Kills_G3sg1", 
                "Total_Kills_SG556", 
                "Total_Kills_Scar20", 
                "Total_Kills_SSG08", 
                "Total_Kills_M4A1",
                "Total_Kills_Galilar" };
            string type = "Rifle";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("heavy")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO heavy stats.")]
        public async Task heavy(CommandContext ctx)
        {
            string[] stringArray = {
                "Total_Kills_XM1014", 
                "Total_Kills_M249", 
                "Total_Kills_Nova", 
                "Total_Kills_Negev", 
                "Total_Kills_Sawedoff",
                "Total_Kills_Mag7" };
            string type = "Heavy";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("heavy")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO heavy stats.")]
        public async Task heavy(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Total_Kills_XM1014",
                "Total_Kills_M249",
                "Total_Kills_Nova", 
                "Total_Kills_Negev", 
                "Total_Kills_Sawedoff",
                "Total_Kills_Mag7"};
            string type = "Heavy";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("Utility")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO Utility stats.")]
        public async Task Utility(CommandContext ctx)
        {
            string[] stringArray = { 
                "Total_Kills_Knife", 
                "Total_Kills_Knife_Fight",
                "Total_Kills_Hegrenade",
                "Total_Kills_Molotov",
                "Total_Kills_Taser" };
            string type = "Utility";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("Utility")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO Utility stats.")]
        public async Task Utility(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Total_Kills_Knife", 
                "Total_Kills_Knife_Fight",
                "Total_Kills_Hegrenade",
                "Total_Kills_Molotov", 
                "Total_Kills_Taser" };
            string type = "Utility";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("obj")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO Basic stats.")]
        public async Task obj(CommandContext ctx)
        {
            string[] stringArray = { 
                "Total_Rescued_Hostages", 
                "Total_Wins",
                "Total_Defused_Bombs",
                "Rotal_Planted_Bombs", 
                "Total_Kills_Enemy_Blinded" };
            string type = "Objective";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("obj")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO Basic stats.")]
        public async Task obj(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Total_Rescued_Hostages", 
                "Total_Wins",
                "Total_Defused_Bombs", 
                "Rotal_Planted_Bombs", 
                "Total_Kills_Enemy_Blinded" };
            string type = "Objective";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("map")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO map stats.")]
        public async Task map(CommandContext ctx)
        {
            string[] stringArray = {
                "total_wins_map_cs_assault", 
                "total_wins_map_cs_italy", 
                "total_wins_map_cs_office",
                "total_wins_map_de_aztec", 
                "total_wins_map_de_cbble", 
                "total_wins_map_de_dust2", 
                "total_wins_map_de_dust", 
                "total_wins_map_de_inferno", 
                "total_wins_map_de_nuke", 
                "total_wins_map_de_train", 
                "total_wins_map_de_house", 
                "total_wins_map_de_bank",
                "total_wins_map_de_vertigo",
                "total_wins_map_ar_monastery", 
                "total_wins_map_ar_shoots", 
                "total_wins_map_ar_baggage", 
                "total_wins_map_de_lake", 
                "total_wins_map_de_sugarcane", 
                "total_wins_map_de_stmarc", 
                "total_wins_map_de_shorttrain",
                "total_wins_map_de_safehouse", 
                "total_wins_map_cs_militia" };
            string type = "map";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("map")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO map stats.")]
        public async Task map(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = {
                "total_wins_map_cs_assault",
                "total_wins_map_cs_italy", 
                "total_wins_map_cs_office",
                "total_wins_map_de_aztec",
                "total_wins_map_de_cbble",
                "total_wins_map_de_dust2", 
                "total_wins_map_de_dust", 
                "total_wins_map_de_inferno",
                "total_wins_map_de_nuke", 
                "total_wins_map_de_train",
                "total_wins_map_de_house", 
                "total_wins_map_de_bank",
                "total_wins_map_de_vertigo",
                "total_wins_map_ar_monastery",
                "total_wins_map_ar_shoots", 
                "total_wins_map_ar_baggage",
                "total_wins_map_de_lake",
                "total_wins_map_de_sugarcane",
                "total_wins_map_de_stmarc", 
                "total_wins_map_de_shorttrain", 
                "total_wins_map_de_safehouse", 
                "total_wins_map_cs_militia" };
            string type = "map";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        [Command("lastmatch")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO last match stats.")]
        public async Task lastmatch(CommandContext ctx)
        {
            string[] stringArray = { 
                "Last_Match_T_Wins", 
                "Last_Match_CT_Wins", 
                "Last_Match_Kills", 
                "Last_Match_Deaths",
                "Last_Match_Mvps", 
                "Last_Match_Damage" };
            string type = "last match";
            await csgoDynamicStats(ctx, ctx.Member.Id, true, stringArray, type);
        }

        [Command("lastmatch")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows CSGO last match stats.")]
        public async Task lastmatch(CommandContext ctx, [Description("Discord member")] DiscordUser member)
        {
            string[] stringArray = { 
                "Last_Match_T_Wins", 
                "Last_Match_CT_Wins",
                "Last_Match_Kills",
                "Last_Match_Deaths", 
                "Last_Match_Mvps", 
                "Last_Match_Damage" };
            string type = "last match";
            await csgoDynamicStats(ctx, member.Id, false, stringArray, type);
        }

        private async Task csgoDynamicStats(CommandContext ctx, ulong memberId, bool userSelf, string[] statArray, string type)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            string steamApiKey = configJson.SteamApiKey;

            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];
            
            long steamId = profile.SteamId;

            if (steamId == 0L)
            {
                await SetId(ctx, ctx.Member.Id, userSelf);
            }

            long validSteamId = profile.SteamId;

            string urlApi = $"http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=730&key={ steamApiKey }&steamid={ validSteamId }";
            string steamUserApi = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={ steamApiKey }&steamids={ validSteamId }";

            try
            {

                jsonCSGO Json = await GetJsonStringCSGO(urlApi);
                jsonSteam jsonUser = await GetJsonStringSteam(steamUserApi);
                await GetDynamicCsgoStats(ctx, urlApi, steamUserApi, userSelf, member, Json, jsonUser, statArray, type);
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("It seems that there has been an error, please make sure your steamId is correct (Should be 17 digit number) and your steam profile is not set to private, this will not work if your profile is private.").ConfigureAwait(false);
                await SetId(ctx, ctx.Member.Id, userSelf);
            }
        }

        private async Task SetId(CommandContext ctx, ulong memberId, bool userSelf)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            long steamId = profile.SteamId;

            if (userSelf == false)
            {
                await ctx.Channel.SendMessageAsync("This user has not yet set their steamId, They must do this them self and cannot be done by others.").ConfigureAwait(false);
                return;
            }

            var SetSteamIdStep = new TextStep("Provide your SteamId. e.g. '12345678912345678'\nDO NOT REPLY TO THIS MESSAGE WITH W! [SteamId] Just enter your Id.\nTo get your steam ID use this [website](https://steamidfinder.com/lookup/) and use the steamID64, if you want to find information on what information your steam ID can provide follow [this link](https://developer.valvesoftware.com/wiki/Steam_Web_API).", null);

            string newSteamId = string.Empty;
            SetSteamIdStep.onValidResult += (result) => newSteamId = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                SetSteamIdStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            try
            {
                long newSteamIdLong = long.Parse(newSteamId);

                await _steamIdService.SetSteamIdAsync(memberId, ctx.Guild.Id, newSteamIdLong).ConfigureAwait(false);

                System.Threading.Thread.Sleep(500);
                await ctx.Channel.SendMessageAsync("Steam Id Set! try using the command now.").ConfigureAwait(false);
                return;
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("That is not a valid steam Id.").ConfigureAwait(false);
                return;

            }
        }

        public async Task GetDynamicCsgoStats(CommandContext ctx, string urlApi, string steamUserApi, bool userSelf, DiscordMember member, jsonCSGO Json, jsonSteam jsonUser, string[] statArray, string type)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string stat in statArray)
            {
                try
                {
                    sb.Append(stat.Replace("_", " ").Replace("Total","").Replace("Kills","") + " : " + String.Format("{0:n0}" , Json.playerstats.stats.Find(obj => obj.name == stat.ToLower()).value) + "\n");
                }
                catch { }
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"CSGO {type} Stats",
                Description = $"SteamID: {Json.playerstats.steamID}\nProfile for {jsonUser.response.players[0].personaname}: {jsonUser.response.players[0].profileurl}\n\nIf you didnt find the stat you were looking for try 'w!help cs' to see what options there are.",
                ThumbnailUrl = jsonUser.response.players[0].avatarmedium,
                Color = ctx.Member.Color
            };

            embed.AddField($"{type} Stats:", sb.ToString());

            if (userSelf == true) { embed.WithFooter("If you wish to change your steam Id please click the gear emoji down below. (will disappear after 60 Seconds)"); }

            await ctx.Message.DeleteAsync("cleanup");
            var embedMessage = await ctx.Channel.SendMessageAsync("", embed: embed);

            await Sendreactiongear(ctx, embedMessage, userSelf);
        }

        public async Task Sendreactiongear(CommandContext ctx, DiscordMessage embedMessage, bool userSelf)
        {

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
                        await SetId(ctx, ctx.Member.Id, userSelf);
                        return;
                    }
                }
                catch
                {
                    await embedMessage.DeleteAllReactionsAsync(null);
                }
            }
        }



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<jsonCSGO> GetJsonStringCSGO(string urlApi)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

            jsonCSGO Json = JsonConvert.DeserializeObject<jsonCSGO>(stringresulttest);

            return Json;
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<jsonSteam> GetJsonStringSteam(string steamUserApi)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            WebRequest requestObjGet = WebRequest.Create(steamUserApi);
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

            jsonSteam Json = JsonConvert.DeserializeObject<jsonSteam>(stringresulttest);

            return Json;
        }

        public class jsonCSGO
        {
            public playerstats playerstats { get; set; }
        }

        public class playerstats
        {
            public long steamID { get; set; }
            public string gameName { get; set; }
            public List<stats> stats { get; set; }
            public List<achievements> achievements { get; set; }
        }


        public class stats
        {
            public string name { get; set; }
            public int value { get; set; }
        }

        public class achievements
        {
            public string name { get; set; }
            public int value { get; set; }
        }

        public class jsonSteam
        {
            public response response { get; set; }
        }

        public class response
        {
            public List<players> players { get; set; }
        }

        public class players
        {
            public string personaname { get; set; }
            public string avatarmedium { get; set; }
            public string profileurl { get; set; }
        }
    }
}



