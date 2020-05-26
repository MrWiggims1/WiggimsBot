using System;
using System.Collections.Generic;
using System.Text;

namespace WigsBot.DAL.Models.Items
{
    public class RobbingItems : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int LvlRequired { get; set; }
        public int MaxAllowed { get; set; }
        public decimal DefenseBuff { get; set; }
        public decimal AttackBuff { get; set; }
        public bool AllowHeist { get; set; }
    }
}
