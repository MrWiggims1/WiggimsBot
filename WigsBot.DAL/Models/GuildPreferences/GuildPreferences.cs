﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WigsBot.DAL.Models.GuildPreferences
{
    public class GuildPreferences : Entity
    {
        public ulong GuildId { get; set; }

        public int XpPerMessage { get; set; }
        public bool SpellingEnabled { get; set; }
        public int ErrorListLength { get; set; }

        public string AssignableRoleJson { get; set; }

        public ulong AdminRole { get; set; }
        public ulong AutoRole { get; set; }

        public bool GuildEventNotification { get; set; }
        public ulong AdminNotificationChannel { get; set; }

        public bool PunishAtEveryone { get; set; }
    }
}