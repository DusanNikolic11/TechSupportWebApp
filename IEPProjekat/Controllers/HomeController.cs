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
            List<Question> questions = (List<Question>)TempData["list"];
            TempData.Remove("list");
            if (questions == null)
                questions = db.questions.ToList();
            ViewBag.questions = questions;
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

        public ActionResult openQuestion(int index)
        {
            Question question = db.questions.ToList().Find(x => x.Id == index);
            List<Reply> replies = question.Replies.ToList();
            QuestionAnswersClass q = new QuestionAnswersClass();
            q.question = question;
            q.allReplies = replies;
            ViewBag.returnVal = q;
            return View("questionThread");
        }

        [HttpGet]
        public ActionResult searchByWord(String text)
        {
            List<Question> questionss = db.questions.ToList().FindAll(x => x.Text.Contains(text));
            TempData["list"] = questionss;
            return RedirectToAction("Index");
        }

        public ActionResult filterReplies(String category, int question)
        {
            Question questionn = db.questions.ToList().Find(x => x.Id == question);
            List<Reply> replies = db.replies.ToList().FindAll(x => x.ReplyToWhichQuestion.Id == questionn.Id);
            switch (category)
            {
                case "0": replies.Sort(new DateComparer()); break;
                case "1": { replies.Sort(new DateComparer()); replies.Reverse(); } break;
                case "2":
                    replies.Sort(delegate (Reply x, Reply y)
          {
              if (x.PlusGrades > y.PlusGrades)
                  return 1;
              else if (y.PlusGrades > x.PlusGrades)
                  return -1;
              else
                  return 0;
          }); break;
                case "3":
                    replies.Sort(delegate (Reply x, Reply y)
              {
                  if (x.MinusGrades > y.MinusGrades)
                      return 1;
                  else if (y.MinusGrades > x.MinusGrades)
                      return -1;
                  else
                      return 0;
              }); break;
            }
            QuestionAnswersClass q = new QuestionAnswersClass();
            q.question = questionn;
            q.allReplies = replies;
            ViewBag.returnVal = q;
            return View("questionThread");
        }
    }
}