using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    [Table("Purchases")]
    public class Purchase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public float TotalPrice { get; set; }
        [Required]
        public String Type { get; set; }
        [Required]
        public DateTime Moment { get; set; }
        [Required]
        public User Customer { get; set; }
    }
}