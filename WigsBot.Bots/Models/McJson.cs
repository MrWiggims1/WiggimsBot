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
    public class jsonMC
    {
        public bool online { get; set; }
        public motd motd { get; set; }
        public players players { get; set; }
        public string hostname { get; set; }
        public string software { get; set; }
        public plugins plugins { get; set; }
    }

    public class motd
    {
        public List<string> clean { get; set; }
    }

    public class players
    {
        public int online { get; set; }
        public int max { get; set; }
        public List<string> list { get; set; }
    }

    public class plugins
    {
        public List<string> raw { get; set; }
    }
}
