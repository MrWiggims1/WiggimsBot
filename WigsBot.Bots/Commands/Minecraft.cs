using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WigsBot.Bot.Commands
{
    public class minecraft : BaseCommandModule
    {
        [Command("Minecraft")]
        [RequirePrefixes("w!", "W!")]
        [Description("Provides some details on the BAH minecraft server.")]
        [Aliases("MC")]
        public async Task Minecraft(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            string urlApi = "https://api.mcsrvstat.us/2/wiggims.duckdns.org";
            await ctx.TriggerTypingAsync();

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

            jsonMC Json = JsonConvert.DeserializeObject<jsonMC>(stringresulttest);
            string pluginList = string.Join("\n", Json.plugins.raw);

            if (Json.online == false) { await ctx.Channel.SendMessageAsync("The server appears to be off-line at the moment."); }

            var embed = new DiscordEmbedBuilder
            {
                Title = "Minecraft server",
                ThumbnailUrl = "https://cdn.imgbin.com/11/10/13/imgbin-minecraft-pocket-edition-computer-icons-minecraft-mods-mining-Apf2GevkuW2t3yDzqmFvwEkkq.jpg",
                Description = $"BAH minecraft server, anyone can join.",
                Color = DiscordColor.Green
            };

            embed.AddField("IP:", $"[{Json.hostname}](wiggims.duckdns.org)\n{Json.motd.clean[0]}", true);
            embed.AddField("Map Link:", "[Server Map](http://wiggims.duckdns.org:8234/)",true);
            embed.AddField("Running on", Json.software);
            if (Json.players.list != null) { embed.AddField($"{Json.players.online} online out of {Json.players.max}:", string.Join(", ", Json.players.list), true); }
            else { embed.AddField($"{Json.players.online} online out of {Json.players.max}:", "No one is online at this time.", true); }

            embed.AddField("Plugins installed:", pluginList);           
            embed.WithFooter("Piggy don't forget you have to use 192.168.20.59 instead.");

            await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        public class jsonMC
        {
            public bool online { get; set; }
            public motd motd { get; set; }
            public players players { get; set; }
            public string hostname { get; set; }
            public string software { get; set; }
            public plugins plugins { get; set; }
        }

        public class motd
        {
            public List<string> clean { get; set; }
        }

        public class players
        {
            public int online { get; set; }
            public int max { get; set; }
            public List<string> list { get; set; }
        }

        public class plugins
        {
            public List<string> raw { get; set; }
        }
    }
}
