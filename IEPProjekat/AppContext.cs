using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IEPProjekat
{
    public class AppContext : DbContext
    {
        public DbSet<Models.User> users { get; set; }
        public DbSet<Models.Reply> replies { get; set; }
        public DbSet<Models.Question> questions { get; set; }
        public DbSet<Models.Grade> grades { get; set; }
        public DbSet<Models.Channel> channels { get; set; }

        public AppContext() : base("IEP2019")
        {
        }

    }
}