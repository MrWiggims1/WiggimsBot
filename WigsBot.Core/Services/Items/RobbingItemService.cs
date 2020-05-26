using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Bots.JsonModels;
using WigsBot.Core.Services.Profiles;
using WigsBot.DAL;
using WigsBot.DAL.Models.Items;
using WigsBot.DAL.Models.Profiles;
using static WigsBot.Core.Services.Items.RobbingItemService;

namespace WigsBot.Core.Services.Items
{
    public interface IRobbingItemService
    {
        /// <summary>
        /// Gets a list of items specified by the list if item Id's provided.
        /// </summary>
        /// <param name="idList">List of items you wish to retrieve.</param>
        /// <returns>The list of times.</returns>
        List<RobbingItems> GetRobbingItems(List<int> idList);

        /// <summary>
        /// Just gets a list of all available items.
        /// </summary>
        /// <returns>A list of all items in the Db</returns>
        List<RobbingItems> GetAllRobbingItems();

        /// <summary>
        /// Gets a members information on how items in their inventory affects their robbing chances.
        /// </summary>
        /// <param name="profile">The profile of the user.</param>
        /// <returns>The buff numbers for attack and defense.</returns>
        BuffStats GetBuffStats(Profile profile);

        /// <summary>
        /// Calculate how much a users inventory is worth.
        /// </summary>
        /// <param name="profile">The members profile.</param>
        /// <returns>How much gold a users profile is worth.</returns>
        int GetInvWorth(Profile profile);

        /// <summary>
        /// Gives a user an item by its DB Id.
        /// </summary>
        /// <param name="profile">The profile of the user that will be getting th item.</param>
        /// <param name="botProfile">The profile of the Bot.</param>
        /// <param name="itemId">The Id of the item being given.</param>
        Task GiveUserItem(Profile profile, Profile botProfile, int itemId);

        /// <summary>
        /// Gives an item to a user based on its name and subtracts the correct amount of money.
        /// </summary>
        /// <param name="profile">The profile of the user that will receive the item</param>
        /// <param name="botProfile">The profile of the bot.</param>
        /// <param name="itemName">The name of the item being given.</param>
        /// <returns></returns>
        Task GiveUserItem(Profile profile, Profile botProfile, string itemName);

        /// <summary>
        /// Get the amount of gold a member can carry based on how many bags of gold they have.
        /// </summary>
        /// <param name="profile">The members profile.</param>
        /// <returns>Carry amount for user.</returns>
        decimal GetCarryAmount(Profile profile);

        /// <summary>
        /// Removes an item from a user
        /// </summary>
        /// <param name="profile">Profile of the user</param>
        /// <param name="itemId">Item id to remove</param>
        /// <param name="refundGold">Should the user be given their gold.</param>
        /// <returns></returns>
        Task RemoveUserItemById(Profile profile, int itemId, bool refundGold);

        /// <summary>
        /// Creates a new item that users can purchase.
        /// </summary>
        /// <param name="itemName">The name of the new item.</param>
        /// <param name="itemDescription">The description of the item.</param>
        /// <param name="itemCost">The cost of the new item.</param>
        /// <param name="maxAllowed">The maximum number of this item any one member can own.</param>
        /// <param name="levelRequired">The members level required to own this item.</param>
        /// <param name="defenseBuff">The buff this item provides to the member while defending (between -0.5 and 0.5).</param>
        /// <param name="attackBuff">The buff this item provides to the member while attacking (between -0.5 and 0.5).</param>
        /// <param name="allowHeist">Does this item give a member the ability to perform a heist?</param>
        /// <returns></returns>
        Task CreateNewItem(string itemName, string itemDescription, int itemCost, int maxAllowed, int levelRequired, decimal defenseBuff, decimal attackBuff, bool allowHeist = false);
    }

    public class RobbingItemService : IRobbingItemService
    {
        //private readonly IGoldService _goldService;
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profilePreferences;

        public RobbingItemService(DbContextOptions<RPGContext> options, IProfileService profilePreferences)
        {
            _options = options;
            _profilePreferences = profilePreferences;
        }

        public List<RobbingItems> GetRobbingItems(List<int> idList)
        {
            using var context = new RPGContext(_options);

            List<RobbingItems> list = new List<RobbingItems>();

            foreach (var id in idList)
            {
                 list.Add( context.RobbingItems.FirstOrDefault(x => x.Id == id));
            }
            return list;
        }

        public List<RobbingItems> GetAllRobbingItems()
        {
            using var context = new RPGContext(_options);

            List<RobbingItems> robbingItems = context.RobbingItems.ToList();

            return robbingItems;
        }

        public BuffStats GetBuffStats(Profile profile)
        {
            using var context = new RPGContext(_options);

            var usersItems = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            List<int> idList = new List<int>();

            foreach (var item in usersItems.Robbing)
            {
                if (item.Count > 0)
                    idList.Add(item.Id);
            }

            var itemList = GetRobbingItems(idList);

            BuffStats stats = new BuffStats { attack = 1M, defense = 1M };

            foreach (var item in itemList)
            {
                stats.attack += item.AttackBuff * usersItems.Robbing.FirstOrDefault(x => x.Id == item.Id).Count;
                stats.defense += item.DefenseBuff * usersItems.Robbing.FirstOrDefault(x => x.Id == item.Id).Count;
            }

            return stats;
        }

        public int GetInvWorth(Profile profile)
        {
            using var context = new RPGContext(_options);

            var usersItems = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            int invWorth = 0;
            List<int> idList = new List<int>();

            foreach (var item in usersItems.Robbing)
            {
                if (item.Count > 0)
                     idList.Add(item.Id);
            }

            var itemList = GetRobbingItems(idList);

            foreach (var item in itemList)
            {
                invWorth += item.Cost * usersItems.Robbing.First(x => x.Id == item.Id).Count;
                if (item.Id == 12) // check if its gold bag, if so, remove that from total
                    invWorth -= item.Cost;
            }

            return invWorth;
        }

        public async Task GiveUserItem(Profile profile, Profile botProfile, int itemId)
        {
            using var context = new RPGContext(_options);

            RobbingItems item = context.RobbingItems.SingleOrDefault(x => x.Id == itemId);

            if (profile.Gold < item.Cost)
                throw new InvalidOperationException("The user cannot afford to purchase this item.");

            if (profile.Level < item.LvlRequired)
                throw new InvalidOperationException("The user is not the correct level to purchase this item.");

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            if (itemsJson.Robbing.SingleOrDefault(x => x.Id == itemId).Count >= item.MaxAllowed)
                throw new InvalidOperationException("The user already owns the maximum number of items allowed.");

            string newItemsJson;

            if (itemsJson.Robbing.Exists(x => x.Id == itemId))
            {
                itemsJson.Robbing.SingleOrDefault(x => x.Id == itemId).Count++;

                newItemsJson = JsonConvert.SerializeObject(itemsJson);
            }
            else
            {
                Robbing newRobbing = new Robbing { Id = itemId, Count = 1 };

                itemsJson.Robbing.Add(newRobbing);

                newItemsJson = JsonConvert.SerializeObject(itemsJson);
            }

            profile.ItemJson = newItemsJson;
            profile.Gold -= item.Cost;

            context.Profiles.Update(profile);

            botProfile.Gold = +item.Cost;

            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task GiveUserItem(Profile profile, Profile botProfile, string itemName)
        {
            using var context = new RPGContext(_options);

            RobbingItems item = context.RobbingItems.First(x => x.Name.ToLower() == itemName.ToLower());

            if (profile.Gold < item.Cost)
                throw new Exception("User cannot afford the item.");

            if (profile.Level < item.LvlRequired)
                throw new Exception("User is not the right level");

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            string newItemsJson;

            if (itemsJson.Robbing.Exists(x => x.Id == item.Id))
            {
                if (itemsJson.Robbing.SingleOrDefault(x => x.Id == item.Id).Count >= item.MaxAllowed)
                    throw new Exception("user already owns max amount of items.");


                itemsJson.Robbing.SingleOrDefault(x => x.Id == item.Id).Count++;

                newItemsJson = JsonConvert.SerializeObject(itemsJson);
            }
            else
            {
                Robbing newRobbing = new Robbing { Id = item.Id, Count = 1 };

                itemsJson.Robbing.Add(newRobbing);

                newItemsJson = JsonConvert.SerializeObject(itemsJson);
            }



            profile.ItemJson = newItemsJson;
            profile.Gold -= item.Cost;

            context.Profiles.Update(profile);

            botProfile.Gold = +item.Cost;

            context.Profiles.Update(botProfile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveUserItemById(Profile profile, int itemId, bool refundGold)
        {
            using var context = new RPGContext(_options);

            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            RobbingItems item = context.RobbingItems.SingleOrDefault(x => x.Id == itemId);

            itemsJson.Robbing[itemId].Count--;

            if (refundGold)
            {
                profile.Gold =+ item.Cost;
            }

            string newItemsJson = JsonConvert.SerializeObject(itemsJson);

            profile.ItemJson = newItemsJson;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task CreateNewItem(string itemName, string itemDescription, int itemCost, int maxAllowed, int levelRequired, decimal defenseBuff, decimal attackBuff, bool allowHeist = false)
        {
            using var context = new RPGContext(_options);

            var existingItems = GetAllRobbingItems();

            if (existingItems.Exists(x => x.Name.ToLower() == itemName.ToLower()))
                throw new Exception("An item with that name already exists.");

            if (itemCost < 0)
                throw new Exception("You cannot have a negative value for the item cost.");

            if (maxAllowed < 1)
                throw new Exception("The minimum max allowed is 1.");

            if (defenseBuff > 0.5M)
                throw new Exception("The defense buff is really high, please use a lower value.");

            if (defenseBuff < -0.5M)
                throw new Exception("The defense buff is really low, please use a higher value.");

            if (attackBuff > 0.5M)
                throw new Exception("The attack buff is really high, please use a lower value.");

            if (attackBuff < -0.5M)
                throw new Exception("The attack buff is really low, please use a higher value.");

            var item = new RobbingItems
            {
                Name = itemName,
                Cost = itemCost,
                Description = itemDescription,
                LvlRequired = levelRequired,
                AllowHeist = allowHeist,
                MaxAllowed = maxAllowed,
                AttackBuff = attackBuff,
                DefenseBuff = defenseBuff
            };

            context.Add(item);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public decimal GetCarryAmount(Profile profile)
        {
            using var context = new RPGContext(_options);
            ItemsJson itemsJson = JsonConvert.DeserializeObject<ItemsJson>(profile.ItemJson);

            decimal carryAmount = .05M * Convert.ToDecimal(itemsJson.Robbing.First(x => x.Id == 12).Count);

            return carryAmount;
        }

        public void ModifyItem(int itemId, RobbingItems modifiedItem)
        {
            throw new NotImplementedException("This is not yet implemented.");
        }

        /// <summary>
        /// The strength of the users attack and defense, based on the items they own.
        /// </summary>
        public class BuffStats
        {
            /// <summary>
            /// The attacking strength of the user.
            /// </summary>
            public decimal attack { get; set; }

            /// <summary>
            /// The defending strength of the user.
            /// </summary>
            public decimal defense { get; set; }
        }
    }
}
