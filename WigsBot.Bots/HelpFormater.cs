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
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;

namespace WigsBot.Bot
{
    public sealed class WiggimsBotHelpFormatter : BaseHelpFormatter
    {
        private DefaultHelpFormatter _d;

        public WiggimsBotHelpFormatter(CommandContext ctx)
            : base(ctx)
        {
            this._d = new DefaultHelpFormatter(ctx);
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            return this._d.WithCommand(command);
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            return this._d.WithSubcommands(subcommands);
        }

        public override CommandHelpMessage Build()
        {
            var hmsg = this._d.Build();
            var embed = new DiscordEmbedBuilder(hmsg.Embed)
            {
                Color = new DiscordColor(255,165,0) ,               
            };

            embed.WithFooter("If you don't understand something or have a suggestion please let Mr Wiggims know and he will see if it can be improved.");

            return new CommandHelpMessage(embed: embed);
        }
    }
}