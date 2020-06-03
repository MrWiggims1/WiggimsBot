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
    public class UplaySearchJson
    {
        public bool foundmatch { get; set; }
        public Dictionary<String, users> players { get; set; }
    }

    public class users
    {
        public profile profile { get; set; }
        public userRanked ranked { get; set; }
        public R6Stats stats { get; set; }
    }

    public class profile
    {
        public string p_name { get; set; }
        public string p_user { get; set; }
    }

    public class userRanked
    {
        public string kd { get; set; }
        //public int mmr { get; set; }
        //public int rank { get; set; }
    }

    public class R6Stats
    {
        public int level { get; set; }
    }

    public class JsonPlayerStats
    {
        public bool found { get; set; }
        public player player { get; set; }
        public stats Stats { get; set; }
        public ranked ranked { get; set; }
        public Dictionary<string, siegeOp> operators { get; set; }
        public Dictionary<int, season> seasons { get; set; }
    }

    public class player
    {
        public string p_id { get; set; }
        public string p_user { get; set; }
        public string p_platform { get; set; }
        public string p_name { get; set; }
    }

    public class stats
    {
        public int level { get; set; }
        public int generalpvp_headshot { get; set; }
        public int generalpvp_kills { get; set; }
        public int generalpvp_death { get; set; }
        public string generalpvp_kd { get; set; }
        public string generalpvp_hsrate { get; set; }
        public int generalpvp_killassists { get; set; }
        public int generalpvp_meleekills { get; set; }
        public int generalpvp_revive { get; set; }
        public int generalpvp_penetrationkills { get; set; }
        public int generalpvp_matchwon { get; set; }
        public int generalpvp_matchlost { get; set; }
        public string generalpvp_wl { get; set; }
        public int generalpvp_timeplayed { get; set; }

        public int casualpvp_kills { get; set; }
        public int casualpvp_death { get; set; }
        public string casualpvp_kd { get; set; }
        public int casualpvp_matchwon { get; set; }
        public int casualpvp_matchlost { get; set; }
        public string casualpvp_wl { get; set; }
        public int casualpvp_timeplayed { get; set; }

        public int rankedpvp_kills { get; set; }
        public int rankedpvp_death { get; set; }
        public string rankedpvp_kd { get; set; }
        public int rankedpvp_matchwon { get; set; }
        public int rankedpvp_matchlost { get; set; }
        public string rankedpvp_wl { get; set; }
        public int rankedpvp_timeplayed { get; set; }

        public int plantbombpvp_matchwon { get; set; }
        public int plantbombpvp_matchlost { get; set; }
        public string plantbombpvp_wl { get; set; }
        public int secureareapvp_matchwon { get; set; }
        public int secureareapvp_matchlost { get; set; }
        public string secureareapvp_wl { get; set; }
        public int rescuehostagepvp_matchwon { get; set; }
        public int rescuehostagepvp_matchlost { get; set; }
        public string rescuehostagepvp_wl { get; set; }
    }

    public class ranked
    {
        // rando stats
        public int allkills { get; set; }
        public int alldeaths { get; set; }
        public int allwins { get; set; }
        public int alllosses { get; set; }
        public int allabandons { get; set; }
        public string allkd { get; set; }
        public string allwl { get; set; }
        public int killpermatch { get; set; }
        public int deathspermatch { get; set; }

        // top region stats
        public int mmr { get; set; }
        public int maxmmr { get; set; }
        public string kd { get; set; }
        public int rank { get; set; }
        public string rankname { get; set; }
        public int maxrank { get; set; }
        public string maxrankname { get; set; }
        public int champ { get; set; }
        public string topregion { get; set; }

        // Asia ranked stats
        public int AS_kills { get; set; }
        public int AS_deaths { get; set; }
        public int AS_wins { get; set; }
        public int AS_losses { get; set; }
        public int AS_abandons { get; set; }
        public int AS_mmr { get; set; }
        public int AS_maxmmr { get; set; }
        public int AS_champ { get; set; }
        public int AS_mmrchange { get; set; }
        public int AS_actualmmr { get; set; }
        public int AS_matches { get; set; }
        public string AS_wl { get; set; }
        public string AS_kd { get; set; }
        public int AS_rank { get; set; }
        public string AS_rankname { get; set; }
        public int AS_maxrank { get; set; }
        public string AS_maxrankname { get; set; }
        public int AS_killspermatch { get; set; }
        public int AS_deathspermatch { get; set; }

        // NA ranked stats
        public int NA_kills { get; set; }
        public int NA_deaths { get; set; }
        public int NA_wins { get; set; }
        public int NA_losses { get; set; }
        public int NA_abandons { get; set; }
        public int NA_mmr { get; set; }
        public int NA_maxmmr { get; set; }
        public int NA_champ { get; set; }
        public int NA_mmrchange { get; set; }
        public int NA_actualmmr { get; set; }
        public int NA_matches { get; set; }
        public string NA_wl { get; set; }
        public string NA_kd { get; set; }
        public int NA_rank { get; set; }
        public string NA_rankname { get; set; }
        public int NA_maxrank { get; set; }
        public string NA_maxrankname { get; set; }
        public int NA_killspermatch { get; set; }
        public int NA_deathspermatch { get; set; }

        // EU ranked stats
        public int EU_kills { get; set; }
        public int EU_deaths { get; set; }
        public int EU_wins { get; set; }
        public int EU_losses { get; set; }
        public int EU_abandons { get; set; }
        public int EU_mmr { get; set; }
        public int EU_maxmmr { get; set; }
        public int EU_champ { get; set; }
        public int EU_mmrchange { get; set; }
        public int EU_actualmmr { get; set; }
        public int EU_matches { get; set; }
        public string EU_wl { get; set; }
        public string EU_kd { get; set; }
        public int EU_rank { get; set; }
        public string EU_rankname { get; set; }
        public int EU_maxrank { get; set; }
        public string EU_maxrankname { get; set; }
        public int EU_killspermatch { get; set; }
        public int EU_deathspermatch { get; set; }
    }

    public class siegeOp
    {
        public string id { get; set; }
        public string type { get; set; }
        public overall overall { get; set; }
        public seasonal seasonal { get; set; }
    }

    public class overall
    {
        public int wins { get; set; }
        public int losses { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
        public int timeplayed { get; set; }
        public string kd { get; set; }
        public int winrate { get; set; }
    }

    public class seasonal
    {
        public int wins { get; set; }
        public int losses { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
        public int timeplayed { get; set; }
        public string kd { get; set; }
        public int winrate { get; set; }
    }

    public class season
    {
        public string seasonname { get; set; }
        public int AS_mmr { get; set; }
        public int AS_wins { get; set; }
        public int AS_losses { get; set; }
        public int AS_abandons { get; set; }
        public int AS_kills { get; set; }
        public int AS_deaths { get; set; }
        public string maxrankname { get; set; }
    }
}
