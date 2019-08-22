using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models {
    [Table("Grade")]
    public class Grade {
        [Required]
        public int Id { get; set; }
        [Index("IX_UserReplyUQ", 1, IsUnique=true)]
        public User User { get; set; }
        [Index("IX_UserReplyUQ", 2, IsUnique = true)]
        public Reply Reply { get; set; }
        [Required]
        public int Value { get; set; }
    }
}