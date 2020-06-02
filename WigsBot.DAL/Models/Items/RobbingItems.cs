using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WigsBot.DAL.Models.Items
{
    public class RobbingItems : Entity
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        [Required]
        public int Cost { get; set; }

        
        public int LvlRequired { get; set; }

        [Required]
        public int MaxAllowed { get; set; }


        public decimal DefenseBuff { get; set; }
        public decimal AttackBuff { get; set; }
        public bool AllowHeist { get; set; }
    }
}
