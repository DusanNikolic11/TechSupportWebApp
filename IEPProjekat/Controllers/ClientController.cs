using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IEPProjekat;
using IEPProjekat.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IEPProjekat.Controllers
{
    public class ClientController : Controller
    {
        private AppContext db = IEPProjekat.Controllers.HomeController.getContext();

        // GET: Client
        public ActionResult Index()
        {
            List<Question> questions = (List<Question>)TempData["list"];
            TempData.Remove("list");
            if (questions==null)
                questions = db.questions.ToList();
            ViewBag.questions = questions;
            return View();
        }

        public ActionResult goToMyProfile()
        {
            User u = (User)Session["user"];
            ViewBag.User = u;
            return View("../Shared/profilePage");
        }

        public ActionResult goToAskQuestion()
        {
            return View("CreateQuestion");
        }

        [HttpPost]
        public ActionResult submitQuestion(String title, String category, String questiontext)
        {
            String fileLocation="";
            String picname = "";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    fileLocation = Path.Combine(
                        Server.MapPath("~/"), fileName);
                    file.SaveAs(fileLocation);
                }
            }
            if (fileLocation == "")
                picname = "defaultPic.jpg";
            else
                picname = Request.Files[0].FileName;
            DateTime dt = DateTime.Now;
            Question q = new Question { Title = title, Text = questiontext, Picture = picname, Category = category, Status = 1, Author = (User)Session["user"], CreationTime = dt };
            db.questions.Add(q);
            db.SaveChanges();
            return Redirect("Index");
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
        public ActionResult giveReply(String text, int questionId)
        {
            DateTime dt = DateTime.Now;
            Question question = db.questions.ToList().Find(x => x.Id == questionId);
            Reply reply = new Reply() { Text = text, ReplyToWhichQuestion = question, ReplyAuthor = (User)Session["user"], Moment = dt, ReplyToWhichReply = null, PlusGrades = 0, MinusGrades = 0, MyChannel = null };
            db.replies.Add(reply);
            db.SaveChanges();
            return RedirectToAction("openQuestion", new { index = questionId });
        }

        public ActionResult filterCategory(String category)
        {
            List<Question> questions = db.questions.ToList().FindAll(x => x.Category.Equals(category));
            TempData["list"] = questions;
            return RedirectToAction("Index");
        }

        public void rateReply(int replyId, int value)
        {
            Reply reply = db.replies.ToList().Find(x => x.Id == replyId);
            if (value==1)
            {
                reply.PlusGrades++;
            }
            else
            {
                reply.MinusGrades++;
            }
            Grade g = new Grade() {ReplyId=reply.Id, UserId=((User)Session["user"]).Id, Reply = reply, User = (User)Session["user"], Value = value };
            reply.Grades.Add(g);
            ((User)Session["user"]).Grades.Add(g);
            db.grades.Add(g);
            db.SaveChanges();
        }

    }
}
