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
using System.Security.Policy;

namespace WigsBot.Bot.Models
{
    public class TimeoutJson
    {
        public CommandCaller CommandCaller { get; set; }
        public Victim Victim { get; set; }
        public bool HasBeenRestored { get; set; }
        public List<ulong> RoleId { get; set; }
    }

    public class Victim
    {
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
    }

    public class CommandCaller
    {
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
    }
}
