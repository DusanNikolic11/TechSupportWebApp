using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models {
    [Table("Channel")]
    public class Channel {
        [Required]
        public int Id { get; set; }
        public virtual User UserOpener { get; set; }
        public virtual ICollection<User> Agents { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public DateTime Moment { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int NumberOfAgents { get; set; }
    }
}