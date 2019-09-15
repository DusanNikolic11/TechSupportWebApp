﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    public class QuestionAnswersClass
    {
        public Question question { get; set; }
        public List<Reply> allReplies { get; set; }

        public String selectedFilter { get; set; }

        public List<String> filters { get; set; }
    }
}