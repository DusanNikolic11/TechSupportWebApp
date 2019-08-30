using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models {
    [Table("Reply")]
    public class Reply {
        [Required]
        public int Id { get; set; }
        [Required]
        public String Text { get; set; }
        public virtual Question ReplyToWhichQuestion { get; set; }
        public virtual User ReplyAuthor { get; set; }
        [Required]
        public DateTime Moment { get; set; }
        public virtual Reply ReplyToWhichReply { get; set; }
        [Required]
        public int PlusGrades { get; set; }
        [Required]
        public int MinusGrades { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual Channel MyChannel { get; set; }
    }
}