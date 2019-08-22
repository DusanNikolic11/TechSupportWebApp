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
            ViewBag.picture = db.questions.Find(3).Picture;
            return View(db.users.ToList());
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
            DateTime dt = DateTime.Now;
            Question q = new Question { Title = title, Text = questiontext, Picture = Request.Files[0].FileName, Category = category, Status = 1, Author = (User)Session["user"], CreationTime = dt };
            db.questions.Add(q);
            db.SaveChanges();
            return View("Index");
        }

    }
}
