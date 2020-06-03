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
    public class JokeAPIJson
    {
        public string type { get; set; }
        public string joke { get; set; }
        public string setup { get; set; }
        public string delivery { get; set; }
        public bool error { get; set; }
        public List<string> causedBy { get; set; }
    }
}
