﻿using System;
using System.Collections.Generic;
using System.Text;
//using WigsBot.DAL.Models.Items;

namespace WigsBot.DAL.Models.Profiles
{
    /// <summary>
    /// The information held in the database for the member.
    /// </summary>
    public class Profile : Entity
    {
        /// <summary>
        /// The Members Id.
        /// </summary>
        public ulong DiscordId { get; set; }

        /// <summary>
        /// The Id of the guild.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The xp the user has, this is also how many messages the user has sent if xp per message is set to 1.
        /// </summary>
        public int Xp { get; set; }

        /// <summary>
        /// The amount of gold a member has.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// How many times a user has been "got" by a bot.
        /// </summary>
        public int Gots { get; set; }

        /// <summary>
        /// The milestone for gots.
        /// </summary>
        public int GotLevel => Gots / 20;

        /// <summary>
        /// How many times a user has rejoined the server (only actictive for someusers).
        /// </summary>
        public int LeaveCount { get; set; }

        /// <summary>
        /// The amount of words the member has entered incorrectly.
        /// </summary>
        public int SpellErrorCount { get; set; }

        /// <summary>
        /// The amount of words the member has entered correctly.
        /// </summary>
        public int SpellCorrectCount { get; set; }

        /// <summary>
        /// A list of words spelt incorrectly, seperated by commas.
        /// </summary>
        public string SpellErrorList {get; set;}

        /// <summary>
        /// The amount of words the members has entered that show up on the "bogan words list".
        /// </summary>
        public int BoganCount { get; set; }
        
        /// <summary>
        /// The members Beatsaber Id, this will usually be the steam id.
        /// </summary>
        public long BeatSaberId { get; set; }

        /// <summary>
        /// The members Steam Id
        /// </summary>
        public long SteamId { get; set; }

        /// <summary>
        /// The members uplay Id
        /// </summary>
        public String UplayUsername { get; set; }

        /// <summary>
        /// The members Battle Net Id.
        /// </summary>
        public String BattleNetUsername { get; set; }

        /// <summary>
        /// The members Call Of Duty Id.
        /// </summary>
        public String CODMWUsername { get; set; }

        /// <summary>
        /// If the user should recive notifications on events (I gave this a dumb name sorry).
        /// </summary>
        public bool QuietMode { get; set; }

        /// <summary>
        /// Should the user be able to be mimicked with the mimic commands.
        /// </summary>
        public bool IsMimicable { get; set; }

        /// <summary>
        /// The json string containing the users todo lists, and tasks.
        /// </summary>
        public string ToDoJson { get; set; }

        /// <summary>
        /// The json string containing the members items.
        /// </summary>
        public string ItemJson { get; set; }

        /// <summary>
        /// The last date a member used the daily command.
        /// </summary>
        public DateTime DailyCooldown { get; set; }

        /// <summary>
        /// The last time the member robbed someone.
        /// </summary>
        public DateTime RobbingCooldown { get; set; }





        /// <summary>
        /// How many times has the member failed to rob someone. (since 27/5/2020)
        /// </summary>
        public int RobbingAttackLost { get; set; }

        /// <summary>
        /// How many times a member has robbed successfully (since 27/5/2020)
        /// </summary>
        public int RobbingAttackWon { get; set; }

        /// <summary>
        /// How many times has this member been robbed but defended them self. (since 27/5/2020)
        /// </summary>
        public int RobbingDefendWon { get; set; }

        /// <summary>
        /// How many times has this member been robbed. (since 27/5/2020)
        /// </summary>
        public int RobbingDefendLost { get; set; }

        /// <summary>
        /// How much gold has this member stolen from other members. (since 27/5/2020)
        /// </summary>
        public int GoldStolen { get; set; }

        /// <summary>
        /// How much gold has this member lost from other members robbing them. (since 27/5/2020)
        /// </summary>
        public int GoldLostFromTheft { get; set; }

        /// <summary>
        /// How much gold has this member lost from being fined. (since 27/5/2020)
        /// </summary>
        public int GoldLostFines { get; set; }

        /// <summary>
        /// How much gold has this member received from fines of others. (since 27/5/2020)
        /// </summary>
        public int GoldGainedFines { get; set; }

        /// <summary>
        /// How many times a member has used the daily command. (since 27/5/2020)
        /// </summary>
        public int DailiesCollected { get; set; }

        /// <summary>
        /// How much has this member Earned from dailies. (since 27/5/2020)
        /// </summary>
        public int TotalDailyEarnings { get; set; }

        /// <summary>
        /// How many times has the member been mimicked. (since 27/5/2020)
        /// </summary>
        public int TimesMimicked { get; set; }

        /// <summary>
        /// How many times has the member played roulette and succeeded. (since 27/5/2020)
        /// </summary>
        public int RouletteSuccesses { get; set; }

        /// <summary>
        /// How many times has the member played roulette and failed. (since 27/5/2020)
        /// </summary>
        public int RouletteFails { get; set; }

        /// <summary>
        /// How much money has the member won from roulette in total (includes losses). (since 27/5/20)
        /// </summary>
        public int TotalRouletteEarnings { get; set; }





        

        /// <summary>
        /// The level of the member, based on the members Xp.
        /// </summary>
        public int Level => Convert.ToInt32(Math.Pow(Convert.ToDouble(Xp), .75) / 30);

        /// <summary>
        /// The spelling accuarcy of the member.
        /// </summary>
        public double SpellAcc => Math.Round(Convert.ToDouble(SpellCorrectCount) / (Convert.ToDouble(SpellCorrectCount) + Convert.ToDouble(SpellErrorCount)) * 100, 2);

        /// <summary>
        /// The percentage of words that the member uses that a counted as "bogan words".
        /// </summary>
        public double BoganPercent => Math.Round(Convert.ToDouble(BoganCount) / (Convert.ToDouble(SpellCorrectCount) + Convert.ToDouble(SpellErrorCount)) * 100, 2);

        /// <summary>
        /// A measure of how "bogan" a member is.
        /// </summary>
        public double Boganometer => Math.Round(100 / (1 + Math.Pow(Math.E, .08 * (SpellAcc - BoganPercent - 50))), 2 );

        /// <summary>
        /// The ratio between amount of times a member has been got and messages sent.
        /// </summary>
        public double gotWordRatio => Math.Round((Convert.ToDouble(Gots))/((Convert.ToDouble(Xp))) * 100, 5);

        /// <summary>
        /// How often does this member succeed at robbing.
        /// </summary>
        public decimal RobAttackSuccessRate => Math.Round(Convert.ToDecimal(RobbingAttackWon) / (Convert.ToDecimal(RobbingAttackWon) + Convert.ToDecimal(RobbingAttackLost)) * 100, 2);

        /// <summary>
        /// How often does this member succeed at defending a robbing.
        /// </summary>
        public decimal RobDefendSuccessRate => Math.Round(Convert.ToDecimal(RobbingDefendWon) / (Convert.ToDecimal(RobbingDefendWon) + Convert.ToDecimal(RobbingDefendLost)) * 100, 2);

        /// <summary>
        /// How often does this member succeed at Roulette.
        /// </summary>
        public decimal RouletteSuccessRate => Math.Round(Convert.ToDecimal(RouletteSuccesses) / (Convert.ToDecimal(RouletteSuccesses) + Convert.ToDecimal(RouletteFails)) * 100, 2);
    }
}
