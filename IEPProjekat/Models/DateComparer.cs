using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEPProjekat.Models
{
    public class DateComparer : IComparer<Reply>
    {
        public int Compare(Reply x, Reply y)
        {
            if (x.Moment.CompareTo(y.Moment) == 1)
                return 1;
            else if (x.Moment.CompareTo(y.Moment) == 0)
                return 0;
            else return -1;
        }
    }
}