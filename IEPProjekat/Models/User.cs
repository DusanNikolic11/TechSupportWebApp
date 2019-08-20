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
        [Key]
        private int Id { get; set; }
        [Required]
        private String Name { get; set; }
        [Required]
        private String LastName { get; set; }
        [Required]
        private String Mail { get; set; }
        [Required]
        private String Password { get; set; }
        [Required]
        private int Tokens { get; set; }
        [Required]
        private String Status { get; set; }
        [Required]
        private String Role { get; set; }
        public virtual ICollection<Question> Questions {get;set;}
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Channel> Channels { get; set; }
    }
}