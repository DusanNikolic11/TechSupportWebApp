using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models {
    [Table("Question")]
    public class Question {
        [Required]
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Text { get; set; }
        public String Picture { get; set; }
        [Required]
        public String Category { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public virtual User Author { get; set; }
        [Required]
        public DateTime CreationTime { get; set; }
        public DateTime? LastLockTime { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual Channel MyChannel { get; set; }
    }
}