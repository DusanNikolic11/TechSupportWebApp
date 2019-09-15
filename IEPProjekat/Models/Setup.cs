using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    [Table("Setup")]
    public class Setup
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public float PriceSilver { get; set; }
        [Required]
        public float PriceGold { get; set; }
        [Required]
        public float PricePlat { get; set; }
        [Required]
        public int AmountSilver { get; set; }
        [Required]
        public int AmountGold { get; set; }
        [Required]
        public int AmountPlat { get; set; }
        [Required]
        public int ChannelAmountSilver { get; set; }
        [Required]
        public int ChannelAmountGold { get; set; }
        [Required]
        public int ChannelAmountPlat { get; set; }
    }
}