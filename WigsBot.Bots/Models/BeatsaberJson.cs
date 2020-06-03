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
    public class jsonBeatSaber
    {
        public playerInfo playerInfo { get; set; }
        public scoreStats scoreStats { get; set; }
    }

    public class playerInfo
    {
        public long playerid { get; set; }
        public double pp { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string avatar { get; set; }
        public int rank { get; set; }
        public int countryRank { get; set; }
    }

    public class scoreStats
    {
        public int totalScore { get; set; }
        public int totalRankedScore { get; set; }
        public double averageRankedAccuracy { get; set; }
        public int totalPlayCount { get; set; }
        public int rankedPlayCount { get; set; }
    }

    public class jsonScores
    {
        public List<scores> scores { get; set; }
    }

    public class scores
    {
        public int score { get; set; }
        public double pp { get; set; }
        public string name { get; set; }
        public string songAuthorName { get; set; }
        public string levelAuthorName { get; set; }
        public string diff { get; set; }
        public string rank { get; set; }
    }
}
