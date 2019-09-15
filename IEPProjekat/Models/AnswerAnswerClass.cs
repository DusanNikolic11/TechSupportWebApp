using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    public class AnswerAnswerClass
    {
        public Reply reply {get;set;}
        public int offset { get; set; }
        public List<Reply> allReplies { get; set; }
    }
}