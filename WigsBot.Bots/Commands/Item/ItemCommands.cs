using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeCantSpell.Hunspell;
using WigsBot.Bot.Handlers.Dialogue;
using WigsBot.Bot.Handlers.Dialogue.Steps;
using WigsBot.Bots.JsonModels;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.Core.Services.GuildPreferencesServices;
using WigsBot.Core.Services.Items;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL.Models.GuildPreferences;
using WigsBot.DAL.Models.Items;
using WigsBot.DAL.Models.Profiles;
using static WigsBot.Core.Services.GuildPreferencesServices.AssignableRoleService;

namespace WigsBot.Bot.Commands.Item
{    
    [Group("Item")]
    [Aliases("items")]
    [Description("Shows an item shop, where you can spend your gold.")]
    public class ItemCommands : BaseCommandModule
    {
        private readonly IRobbingItemService _robbingItemService;
        private readonly IGoldService _goldService;
        private readonly IProfileService _profileService;

        public ItemCommands(
            IRobbingItemService robbingItemService,
            IProfileService profileService,
            IGoldService goldService
            )
        {
            _robbingItemService = robbingItemService;
            _profileService = profileService;
            _goldService = goldService;
        }

        [GroupCommand]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows items available for purchase.")]
        public async Task groupCommands(CommandContext ctx)
        {
            await showShop(ctx);
        }

        [Command("shop")]
        [Aliases("list")]
        [RequirePrefixes("w!", "W!")]
        [Description("Shows items available for purchase.")]
        public async Task shop(CommandContext ctx)
        {
            await showShop(ctx);
        }

        [Command("buy")]
        [Aliases("purchase")]
        [RequirePrefixes("w!", "W!")]
        [Description("Use to purchase an item from the shop `w!item shop`")]
        public async Task buyItem(CommandContext ctx, [RemainingText, Description("item name")] string itemName)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            Profile botProfile = await _profileService.GetOrCreateProfileAsync(ctx.Client.CurrentUser.Id, ctx.Guild.Id).ConfigureAwait(false);

            var AllRobbingItmes = _robbingItemService.GetAllRobbingItems();

            try
            {
                var itemByName = AllRobbingItmes.SingleOrDefault(x => x.Name.ToLower() == itemName.ToLower());

                ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

                try
                {
                    if (itemsJson.Robbing.SingleOrDefault(x => x.Id == itemByName.Id).Count >= itemByName.MaxAllowed)
                    {
                        await ctx.Channel.SendMessageAsync($"You already have the maximum amount possible for this item.").ConfigureAwait(false);
                        return;
                    }
                }
                catch { }


                if (itemByName.Cost > profile.Gold)
                {
                    await ctx.Channel.SendMessageAsync($"You cannot afford this item you only have {profile.Gold} gold and need {itemByName.Cost} gold.").ConfigureAwait(false);
                    return;
                }

                if (itemByName.LvlRequired > profile.Level)
                {
                    await ctx.Channel.SendMessageAsync($"You are not the right level for this item, you need to be level {itemByName.LvlRequired} and are level {profile.Level}").ConfigureAwait(false);
                    return;
                }
            }
            catch { await ctx.Channel.SendMessageAsync("Please make sure this item actually exists."); return;  }

            await _robbingItemService.GiveUserItem(profile, botProfile, itemName).ConfigureAwait(false);

            await ctx.RespondAsync($"You successfully bought {itemName}!");
        }

        [Command("info")]
        [RequirePrefixes("w!", "W!")]
        [Description("shows information on an item")]
        [Aliases("information", "about")]
        public async Task itemInfo(CommandContext ctx, [RemainingText, Description("item name")] string itemName)
        {
            List<RobbingItems> items = _robbingItemService.GetAllRobbingItems();

            var item = items.First(x => x.Name.ToLower() == itemName.ToLower());

            await ctx.Channel.SendMessageAsync($"{item.Name}:\t\tLevel required: `{item.LvlRequired}`\t\tItem buffs: `defense: {item.DefenseBuff * 100}%`\t`attack: {item.AttackBuff * 100}%`\n{item.Description}");
        }

        [Command("Create")]
        [RequirePrefixes("w@", "W@")]
        [Description("Creates a new item for members to purchase.")]
        [RequireOwner]
        public async Task CreateNewItem(CommandContext ctx)
        {
            throw new NotImplementedException("This command is not implemented yet.");
        }

        // ####### Tasks ########

        public async Task showShop(CommandContext ctx)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            List<RobbingItems> robbingItems = _robbingItemService.GetAllRobbingItems();

            var sortedList = robbingItems.OrderBy(x => x.LvlRequired);

            StringBuilder sb = new StringBuilder();

            foreach (var item in sortedList)
            {
                if (item.MaxAllowed > 1)
                {
                    try
                    {
                        if (itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count == item.MaxAllowed)
                        {
                            sb.Append($"#[already own max allowed] {item.Name}\n\n");
                        }
                        else
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n#{itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count} out of {item.MaxAllowed} owned.\n\n");
                        }
                    }
                    catch
                    {
                        try
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n#0 out of {item.MaxAllowed} owned.\n\n");
                        }
                        catch
                        {
                            sb.Append("#An error occurred.\n\n");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (itemsJson.Robbing.FirstOrDefault(x => x.Id == item.Id).Count == item.MaxAllowed)
                        {
                            sb.Append($"#[already own max allowed] {item.Name}\n\n");
                        }
                        else
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n\n");
                        }
                    }
                    catch
                    {
                        try
                        {
                            sb.Append($"@[{item.Cost} gold] {item.Name} - Level required: {item.LvlRequired}\nItem buffs: defense: '{item.DefenseBuff * 100}%'\tattack: '{item.AttackBuff * 100}%'\n\n");
                        }
                        catch
                        {
                            sb.Append("#An error occurred.\n\n");
                        }
                    }
                }
            }

            await ctx.Channel.SendMessageAsync($"```py\n@To find more information or purchase an item use `w!item info [item name]` and `w!item buy [Item Name]`\n\nYou currently have {profile.Gold} Gold are level {profile.Level}.\n\n{sb.ToString()}\n```").ConfigureAwait(false);
        }
    }
}