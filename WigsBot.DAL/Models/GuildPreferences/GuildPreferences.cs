using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WigsBot.DAL.Models.GuildPreferences
{
    public class GuildPreferences : Entity
    {
        [Required]
        public ulong GuildId { get; set; }

        public int XpPerMessage { get; set; }
        public bool SpellingEnabled { get; set; }
        public int ErrorListLength { get; set; }
        public bool IsGoldEnabled { get; set; }
        public int GoldPerLevelUp { get; set; }

        [MaxLength(2000)]
        public string AssignableRoleJson { get; set; }
        public ulong TimeoutRoleId { get; set; }
        public ulong TimeoutTextChannelId { get; set; }
        public ulong AutoRole { get; set; }
        public ulong StatChannelCatergoryId { get; set; }

        public int TotalCommandsExecuted { get; set; }

        public List<StatChannel> StatChannels { get; set; } = new List<StatChannel>();
    }
}
