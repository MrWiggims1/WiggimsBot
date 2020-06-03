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
    public class TimeoutJson
    {
        public List<ulong> RoleId { get; set; }
    }
}
