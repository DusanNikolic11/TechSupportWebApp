namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Cryptography;

    internal sealed class Configuration : DbMigrationsConfiguration<IEPProjekat.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IEPProjekat.AppContext context)
        {
            // DBCC CHECKIDENT ('[TestTable]', RESEED,0) - Vraca primary key na vrednost 0
            HashAlgorithm algorithm = SHA256.Create();
            byte[] hashed = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin123"));
            String hashedPassword = BitConverter.ToString(hashed).Replace("-", String.Empty);
            Models.User user = new Models.User { Id=0, Name = "Administrator", LastName = "Administratorovic", Mail = "administrator123@gmail.com", Password = hashedPassword, Status = "Active", Role = "Admin" };
            context.users.Add(user);
            context.SaveChanges();
        }
    }
}
