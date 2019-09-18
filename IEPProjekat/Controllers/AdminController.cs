using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEPProjekat;
using IEPProjekat.Models;

namespace IEPProjekat.Controllers
{
    public class AdminController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Admin
        public ActionResult Index(int? page)
        {
            List<Question> questions = (List<Question>)TempData.Peek("list");
            String categorySelected = (String)TempData.Peek("category");
            if (categorySelected == "All categories")
            {
                TempData.Remove("list");
            }
            if (questions == null)
                questions = db.questions.ToList();
            questions = questions.ToList().FindAll(x => x.MyChannel == null);
            QuestionCategoriesClass qc = new QuestionCategoriesClass();
            qc.questions = questions;
            List<String> categories = new List<String>();
            categories.Add("All categories");
            foreach (QuestionCategories category in db.categories.ToList())
            {
                categories.Add(category.Category);
            }
            categories.Add("My questions");
            qc.categories = categories;
            if (categorySelected != null)
            {
                qc.selected = categorySelected;
            }
            else
            {
                qc.selected = "All categories";
            }
            Pager pager = new Pager(questions.ToList().Count, page);
            QuestionPager questpager = new QuestionPager
            {
                Items = questions.ToList().Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
            qc.qp = questpager;
            ViewBag.Questions = qc;
            return View();
        }

        public ActionResult openSetupPage()
        {
            ViewBag.stats = db.setup.ToList().First();
            ViewBag.categories = db.categories.ToList();
            return View("SetupPage");
        }

        public ActionResult openQuestion(int index)
        {
            Question question = db.questions.ToList().Find(x => x.Id == index);
            List<Reply> replies = null;
            if (question.Replies != null)
            {
                replies = question.Replies.ToList();
            }
            QuestionAnswersClass q = new QuestionAnswersClass();
            q.filters = new List<String>();
            q.filters.Add("Least recent"); q.filters.Add("Most recent"); q.filters.Add("Best rated"); q.filters.Add("Worst rated");
            q.selectedFilter = "Least recent";
            q.question = question;
            q.allReplies = replies;
            ViewBag.returnVal = q;
            return View("questionThread");
        }

        [HttpPost]
        public void removeCategory(String dataValue)
        {
            QuestionCategories qc = db.categories.ToList().Find(x => x.Category == dataValue);
            foreach (var question in db.questions.ToList())
            {
                if (question.Category == qc)
                {
                    foreach (var replies in db.replies.ToList())
                    {
                        if (replies.ReplyToWhichQuestion==question)
                        {
                            foreach (var grade in db.grades.ToList())
                            {
                                if (grade.ReplyId==replies.Id)
                                {
                                    db.grades.Remove(grade);
                                }
                            }
                            db.replies.Remove(replies);
                        }
                    }
                    db.questions.Remove(question);
                }
            }
            db.categories.Remove(qc);
            db.SaveChanges();
        }

        public void addCategory(String dataValue)
        {
            QuestionCategories qc = new QuestionCategories { Category = dataValue };
            db.categories.Add(qc);
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult changeSetup(float ps, float pg, float pp, int asi, int ag, int ap, int cas, int cag, int cap)
        {
            Setup setup = db.setup.First();
            setup.PriceSilver = ps;
            setup.PriceGold = pg;
            setup.PricePlat = pp;
            setup.AmountSilver = asi;
            setup.AmountGold = ag;
            setup.AmountPlat = ap;
            setup.ChannelAmountSilver = cas;
            setup.ChannelAmountGold = cag;
            setup.ChannelAmountPlat = cap;
            db.SaveChanges();
            return RedirectToAction("openSetupPage");
        }

        public ActionResult goToMyProfile()
        {
            User u = (User)Session["user"];
            ViewBag.User = u;
            return View("../Shared/profilePage");
        }
        public ActionResult logout()
        {
            Session.Remove("user");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult searchByWord(String text)
        {
            List<Question> questionss;
            if (TempData["list"] == null || (TempData["category"] == null))
            {
                questionss = db.questions.ToList().FindAll(x => x.Title.Contains(text) == true);
            }
            else
            {
                questionss = ((List<Question>)TempData.Peek("list")).FindAll(x => x.Title.Contains(text) == true);
            }
            TempData["list"] = questionss;
            return RedirectToAction("Index");
        }

        public ActionResult filterCategory(String category)
        {
            List<Question> questions = null;
            if (category == "My questions")
            {
                questions = db.questions.ToList().FindAll(x => x.Author.Name.Equals(((User)Session["user"]).Name) && x.Author.LastName.Equals(((User)Session["user"]).LastName));
                TempData["list"] = questions;
            }
            else if (category != "All categories")
            {
                questions = db.questions.ToList().FindAll(x => x.Category.Category.Equals(category));
            }
            TempData["list"] = questions;
            TempData["category"] = category;
            return RedirectToAction("Index");
        }

        public ActionResult filterReplies(String category, int question)
        {
            Question questionn = db.questions.ToList().Find(x => x.Id == question);
            List<Reply> replies = db.replies.ToList().FindAll(x => x.ReplyToWhichQuestion.Id == questionn.Id);
            QuestionAnswersClass q = new QuestionAnswersClass();
            q.filters = new List<String>();
            q.filters.Add("Least recent"); q.filters.Add("Most recent"); q.filters.Add("Best rated"); q.filters.Add("Worst rated");
            switch (category)
            {
                case "Least recent": { replies.Sort(new DateComparer()); q.selectedFilter = "Least recent"; } break;
                case "Most recent": { replies.Sort(new DateComparer()); replies.Reverse(); q.selectedFilter = "Most recent"; } break;
                case "Best rated":
                    replies.Sort(delegate (Reply x, Reply y)
                    {
                        q.selectedFilter = "Best rated";
                        if (x.MinusGrades > y.MinusGrades)
                            return 1;
                        else if (y.MinusGrades > x.MinusGrades)
                            return -1;
                        else
                            return 0;
                    }); break;

                case "Worst rated":
                    replies.Sort(delegate (Reply x, Reply y)
                    {
                        q.selectedFilter = "Worst rated";
                        if (x.PlusGrades > y.PlusGrades)
                            return 1;
                        else if (y.PlusGrades > x.PlusGrades)
                            return -1;
                        else
                            return 0;
                    }); break;
            }
            q.question = questionn;
            q.allReplies = replies;
            ViewBag.returnVal = q;
            return View("questionThread");
        }

        public ActionResult lockQuestion(int questionId)
        {
            Question q = db.questions.ToList().Find(x => x.Id == questionId);
            q.LastLockTime = DateTime.Now;
            q.Status = 1;
            db.SaveChanges();
            return RedirectToAction("openQuestion", new { index = questionId });
        }

        public ActionResult deleteQuestion(int questionId)
        {
            Question q = db.questions.ToList().Find(x => x.Id == questionId);
            foreach (var reply in db.replies.ToList())
            {
                if (reply.ReplyToWhichQuestion == q)
                {
                    db.replies.Remove(reply);
                }
            }
            db.questions.Remove(q);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult openAllUsers()
        {
            ViewBag.users = db.users.ToList();
            return View("allUsers");
        }

        [HttpPost]
        public ActionResult changeRole(String userId, String newR)
        {
            int id = int.Parse(userId);
            User u = db.users.ToList().Find(x => x.Id == id);
            u.Role = newR;
            db.SaveChanges();
            return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult changeStatus(String userId, String newR)
        {
            int id = int.Parse(userId);
            User u = db.users.ToList().Find(x => x.Id == id);
            u.Status = newR;
            db.SaveChanges();
            return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult changeEverything(String userId, String newName, String newEmail, String newTokens)
        {
            int id = int.Parse(userId);
            int tokens = int.Parse(newTokens);
            User u = db.users.ToList().Find(x => x.Id == id);
            u.Name = newName.Split(' ')[0];
            u.LastName = newName.Split(' ')[1];
            u.Mail = newEmail;
            u.Tokens = tokens;
            db.SaveChanges();
            return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
        }
    }
}
