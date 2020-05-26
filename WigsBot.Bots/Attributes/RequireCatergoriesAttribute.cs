using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WigsBot.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireCatergoriesAttribute : CheckBaseAttribute
    {
        public IReadOnlyList<string> CatergoryNames { get; }
        public ChannelCheckMode CheckMode { get; }
        
        public RequireCatergoriesAttribute(ChannelCheckMode checkmode, params string[] channelNames)
        {
            CheckMode = checkmode;
            CatergoryNames = new ReadOnlyCollection<string>(channelNames);
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null || ctx.Member == null)
            {
                return Task.FromResult(false);
            }

            bool contains = CatergoryNames.Contains(ctx.Channel.Parent.Name, StringComparer.OrdinalIgnoreCase);

            return CheckMode switch
            {
            ChannelCheckMode.any => Task.FromResult(contains),

            ChannelCheckMode.none => Task.FromResult(!contains),

                _ => Task.FromResult(false),
            };
        }
    }
}
