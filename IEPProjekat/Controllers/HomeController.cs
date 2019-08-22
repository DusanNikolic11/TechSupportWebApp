using IEPProjekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace IEPProjekat.Controllers
{
    public class HomeController : Controller
    {
        private static AppContext db = new AppContext();

        public static AppContext getContext()
        {
            return db;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult goToRegisterPage()
        {
            return View("RegisterPage");
        }
        [HttpPost]
        public ActionResult registerMe(String first_name, String last_name, String email, String password, String password_confirmation)
        {
            User user = db.users.SingleOrDefault(s => s.Mail == email);
            if (user == null)
            {
                if (password==password_confirmation)
                {
                    String hashedPassword = hashPassword(password);
                    User newUser = new User { Name = first_name, LastName = last_name, Mail = email, Password = hashedPassword, Role="Client", Status="Active" };
                    db.users.Add(newUser);
                    db.SaveChanges();
                    return View("Login");
                }
            }
            return View("Home", "Index");
        }

        private String hashPassword(String password)
        {
            HashAlgorithm algorithm = SHA256.Create();
            byte[] hashed = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashed).Replace("-", String.Empty);
        }

        public ActionResult goToLoginPage()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(String email, String password)
        {
            String hashedPassword = hashPassword(password);
            User user = db.users.SingleOrDefault(s => s.Mail == email && s.Password==hashedPassword && s.Status == "Active");
            if (user!=null)
            {
                Session["user"] = user;
                return RedirectToAction("Index", "Client");
            }
            return View("Login");
        }
    }
}