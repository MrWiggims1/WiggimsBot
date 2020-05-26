using System;
using System.Collections.Generic;
using System.Text;

namespace WigsBot.DAL.Models.Items
{
    public class Fish : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Emoji { get; set; }
        public int Worth { get; set; }
        public int Rarity { get; set; }
    }
}
