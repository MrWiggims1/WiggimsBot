using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WigsBot.Bot
{
    public struct ConfigJson
    { 
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string[] Prefix { get; private set; }

        [JsonProperty("steamapitoken")]
        public string SteamApiKey { get; private set; }

        [JsonProperty("r6tabtoken")]
        public string r6tabtoken { get; private set; }

        [JsonProperty("dbconnectionstring")]
        public string dbconnectionstring { get; private set; }

        [JsonProperty("goldperlevelup")]
        public int goldperlevelup { get; private set; }

        [JsonProperty("youtubeApiKey")]
        public string youtubeApiKey { get; private set; }
    }
}
