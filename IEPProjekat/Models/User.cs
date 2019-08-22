using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models {
    [Table("Users")]
    public class User {
        [Required]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String LastName { get; set; }
        [Required]
        public String Mail { get; set; }
        [Required]
        public String Password { get; set; }
        [Required]
        public int Tokens { get; set; }
        [Required]
        public String Status { get; set; }
        [Required]
        public String Role { get; set; }
        public virtual ICollection<Question> Questions {get;set;}
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Channel> Channels { get; set; }
    }
}