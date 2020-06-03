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
using System.Web;
using WigsBot.Bot.Models;

namespace WigsBot.Bot.Commands
{
    public class memes : BaseCommandModule
    {
        [Command("flip")]
        [RequirePrefixes("w!", "W!")]
        [Description("For when you really just need to flip yo shit.")]
        public async Task flip(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("(╯°□°）╯︵ ┻━┻").ConfigureAwait(false);
        }

        [Command("unflip")]
        [RequirePrefixes("w!", "W!")]
        [Description("OK now thats out of your system, lets get back to gaming.")]
        public async Task unflip(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("┬─┬ ノ( ゜-゜ノ)").ConfigureAwait(false);
        }

        [Command("xkcd")]
        [RequirePrefixes("w!", "W!")]
        [Description("Send the latest xkcd comic.")]
        public async Task xkcd(CommandContext ctx)
        {
            WebRequest requestObjGet = WebRequest.Create("https://xkcd.com/info.0.json");
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

            XkcdJson Json = JsonConvert.DeserializeObject<XkcdJson>(stringresulttest);

            await ctx.Channel.SendMessageAsync($"{Json.Title}:\n{Json.Img}");
        }

        [Command("joke")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows a bad joke.")]
        public async Task joke(CommandContext ctx, [Description("search for joke")] params string[] keywords)
        {
            StringBuilder apiLink = new StringBuilder();

            apiLink.Append("https://sv443.net/jokeapi/v2/joke/Any");

            if (keywords != null) { apiLink.Append($"?contains={ HttpUtility.HtmlEncode(String.Join(" ", keywords)) }"); }

            WebRequest requestObjGet = WebRequest.Create(apiLink.ToString());
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

            JokeAPIJson Json = JsonConvert.DeserializeObject<JokeAPIJson>(stringresulttest);

            if (Json.error)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var reason in Json.causedBy)
                {
                    sb.Append($"{reason}\n");
                }

                await ctx.Channel.SendMessageAsync($"The request failed for the following reasons:\n{sb.ToString()}");
            }

            if (Json.type == "twopart")
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Description = $"{Json.setup}\n||{Json.delivery}||"
                };

                await ctx.Channel.SendMessageAsync(embed: embed);
                return;
            }

            if (Json.type == "single")
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Description = $"{Json.joke}"
                };

                await ctx.Channel.SendMessageAsync(embed: embed);
                return;
            }
        }
    }
}
