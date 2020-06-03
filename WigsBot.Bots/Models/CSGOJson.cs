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

namespace WigsBot.Bot.Models
{
    public class jsonCSGO
    {
        public playerstats playerstats { get; set; }
    }

    public class playerstats
    {
        public long steamID { get; set; }
        public string gameName { get; set; }
        public List<CSGOstats> stats { get; set; }
        public List<achievements> achievements { get; set; }
    }


    public class CSGOstats
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class achievements
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class jsonSteam
    {
        public response response { get; set; }
    }

    public class response
    {
        public List<McPlayers> players { get; set; }
    }

    public class McPlayers
    {
        public string personaname { get; set; }
        public string avatarmedium { get; set; }
        public string profileurl { get; set; }
    }
}
