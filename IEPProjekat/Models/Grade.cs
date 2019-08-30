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
        public int UserId { get; set; }
        [Index("IX_UserReplyUQ", 2, IsUnique = true)]
        public int ReplyId { get; set; }
        [Required]
        public int Value { get; set; }
        public User User;
        public Reply Reply;
    }
}