using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.PlatformAbstractions;
using System.Threading.Tasks;

namespace WigsBot.Bot.Commands
{
    [Group("info")]
    [RequirePrefixes("w!", "W!")]
    class InfoCommands : BaseCommandModule
    {
        [Command("About")]
        [RequirePrefixes("w!", "W!")]
        [Description("Provides some more information about Wiggims Bot")]
        [Aliases("WiggimsBot","WigsBot")]
        public async Task About(CommandContext ctx)
            {
                string os = System.Environment.OSVersion.Platform.ToString();

            var embed = new DiscordEmbedBuilder
            {
                Title = ctx.Client.CurrentUser.Username,
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
                Description = "Wiggims Bot is just for a bit of fun and to be used as a bit of a learning tool for Wiggims.",
                Color = DiscordColor.Orange
            };

            embed.AddField($"Wiggims Bot Version", "2.0.0",true);
            embed.AddField($"DSharp Version", $"{ctx.Client.VersionString}",true);

            if (os == "Win32NT") { embed.AddField("Current Mode", "Development, expect lots of drops in and out as Wiggims bot is being worked on currently."); }
            else { embed.AddField("Current Mode", "Stable, Wiggims Bot should be running smoothly."); }
            await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("Server")]
        [RequirePrefixes("w!", "W!")]
        [Description("Provides some more information about the discord server.")]
        public async Task server(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = ctx.Guild.Name,
                ThumbnailUrl = ctx.Guild.IconUrl,
                Description = $"The {ctx.Guild.Name} discord server has been running since {ctx.Guild.CreationTimestamp.Year}, and currently holds {ctx.Guild.MemberCount} nerds, {ctx.Guild.Name} is primarily place to chat, meme, game and punch other nerds.",
                Color = DiscordColor.Orange
            };

            embed.AddField("Currently owned by the one and only", ctx.Guild.Owner.Username);

            await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("Channel")]
        [RequirePrefixes("w!", "W!")]
        [Description("Provides some more information about the channel.")]
        public async Task channel(CommandContext ctx)
        {
            var ChannelDescrpt = ctx.Channel.Topic;

            if (ctx.Channel.Topic == "") { ChannelDescrpt = "No description provided"; };

            var embed = new DiscordEmbedBuilder
            {
                Title = ctx.Guild.Name,
                ThumbnailUrl = ctx.Guild.IconUrl,
                Description = $"You are currently in the {ctx.Channel.Name} {ctx.Channel.Type} channel described as '{ChannelDescrpt}'",
                Color = DiscordColor.Orange
            };

            await ctx.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("Future")]
        [RequirePrefixes("w!", "W!")]
        [Description("Future plans for wiggims bot.")]
        public async Task future(CommandContext ctx)
        {
            var TickEmoji = ":white_check_mark:";
            var CrossEmoji = ":negative_squared_cross_mark:";

            var embed = new DiscordEmbedBuilder
            {
                Title = "Possible features of wiggims bot",
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
                Color = DiscordColor.Orange,
                Description = "Feel free to message wiggims if you have anything you want added."
            };

            embed.AddField("Short Term Goals",
                $"{TickEmoji} ~~Have bot running 24/7.~~\n" +
                $"{TickEmoji} ~~Manage roles through commands.~~\n" +
                $"{TickEmoji} ~~Implement got system.~~\n" + 
                $"{TickEmoji} ~~Implement Taco leaving counter.~~\n"+
                $"{TickEmoji} ~~Add purge command to delete bulk messages.~~");

            embed.AddField("Long Term Goals",
                $"{CrossEmoji} Be able to start and stop MC server with commands.\n" +
                $"{TickEmoji} ~~Spell checking API.~~\n" +
                $"{CrossEmoji} Play music and sound files.\n" +
                $"{TickEmoji} ~~Show game stats.~~\n");

            embed.AddField("Goals that would be cool but probably wont happen",
                $"{CrossEmoji} Have mini RPG game and items (maybe, all depends on if i can figure out how to do it all).\n");


            await ctx.Channel.SendMessageAsync("", embed: embed);
        }        
    }
}


