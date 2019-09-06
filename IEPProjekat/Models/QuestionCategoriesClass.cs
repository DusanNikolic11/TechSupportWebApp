using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    public class QuestionCategoriesClass
    {
        public List<Question> questions { get; set; }
        public List<String> categories { get; set; }
        public String selected { get; set; }
        public QuestionPager qp { get; set; }
    }
}