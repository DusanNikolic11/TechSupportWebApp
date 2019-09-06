using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    [Table("categories")]
    public class QuestionCategories
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public String Category { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}