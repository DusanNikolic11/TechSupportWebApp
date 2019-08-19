using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IEPProjekat
{
    public class AppContext : DbContext
    {
        public AppContext() : base("IEP2019")
        {
        }

    }
}