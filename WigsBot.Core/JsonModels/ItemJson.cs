
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WigsBot.Bots.JsonModels
{
    public class ItemsJson
    {
        public List<Robbing> Robbing { get; set; }
    }

    public class Robbing
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
}
