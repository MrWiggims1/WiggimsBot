using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WigsBot.Core.Services.GuildPreferenceServices;
using WigsBot.DAL;
using WigsBot.DAL.Models.GuildPreferences;
using static WigsBot.Core.Services.GuildPreferencesServices.AssignableRoleService;

namespace WigsBot.Core.Services.GuildPreferencesServices
{
    public interface IAssignableRoleService
    {
        /// <summary>
        /// Adds a role to a guilds assignable roles list.
        /// </summary>
        /// <param name="guildId">The id of the discord guild.</param>
        /// <param name="roleId">The id of the discord guild.</param>
        /// <param name="emojiId">The Id of the emoji</param>
        /// <returns></returns>
        public Task AddRoleToAssignableRoles(ulong guildId, ulong roleId, ulong emojiId);

        /// <summary>
        /// Removes a role from the guilds assignable roles list.
        /// </summary>
        /// <param name="guildId">The id of the discord guild.</param>
        /// <param name="roleId">The id of the discord role.</param>
        /// <returns></returns>
        public Task RemoveRoleFromAssignableRoles(ulong guildId, ulong roleId);
    }

    public class AssignableRoleService : IAssignableRoleService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IGuildPreferences _guildPreferences;

        public AssignableRoleService(DbContextOptions<RPGContext> options, IGuildPreferences guildPreferences)
        {
            _options = options;
            _guildPreferences = guildPreferences;
        }

        public async Task AddRoleToAssignableRoles(ulong guildId, ulong roleId, ulong emojiId)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(guildId).ConfigureAwait(false);

            AssignableRoleJson roleArray = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

            Roles roleToAdd = new Roles { RoleId = roleId, EmojiId = emojiId };

            roleArray.Roles.Add(roleToAdd);

            string newRoleArray = JsonConvert.SerializeObject(roleArray);

            guildPreferences.AssignableRoleJson = newRoleArray;

            context.GuildPreferences.Update(guildPreferences);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveRoleFromAssignableRoles(ulong guildId, ulong roleId)
        {
            using var context = new RPGContext(_options);

            GuildPreferences guildPreferences = await _guildPreferences.GetOrCreateGuildPreferences(guildId).ConfigureAwait(false);

            AssignableRoleJson roleArray = JsonConvert.DeserializeObject<AssignableRoleJson>(guildPreferences.AssignableRoleJson);

            int roleIndex = roleArray.Roles.FindIndex(x => x.RoleId == roleId);

            roleArray.Roles.RemoveAt(roleIndex);

            string newRoleArray = JsonConvert.SerializeObject(roleArray);

            guildPreferences.AssignableRoleJson = newRoleArray;

            context.GuildPreferences.Update(guildPreferences);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }




        /// <summary>
        /// A list of roles that are assignable within this guild.
        /// </summary>
        public class AssignableRoleJson
        {
            public List<Roles> Roles { get; set; }
        }

        /// <summary>
        /// The role assignable withing this guild.
        /// </summary>
        public class Roles
        {
            /// <summary>
            /// The Id of the role.
            /// </summary>
            public ulong RoleId { get; set; }

            /// <summary>
            /// The id of the emoji assigned to the role.
            /// </summary>
            public ulong EmojiId { get; set; }
        }
    }
}
