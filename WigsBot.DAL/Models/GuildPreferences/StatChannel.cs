using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WigsBot.Bot.Models;

namespace WigsBot.DAL.Models.GuildPreferences
{
    public class StatChannel : Entity
    {
        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        public StatOption StatOption { get; set; }

        [Required, MaxLength(12)]
        public string StatMessage { get; set; }
    }
}
