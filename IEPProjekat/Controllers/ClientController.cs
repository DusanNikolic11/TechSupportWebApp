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
using PayPal.Api;

namespace IEPProjekat.Controllers
{
    public class ClientController : Controller
    {
        private AppContext db = IEPProjekat.Controllers.HomeController.getContext();

        public ActionResult Index(int? page)
        {
            List<Question> questions = (List<Question>)TempData.Peek("list");
            String categorySelected = (String)TempData.Peek("category");
            if (categorySelected == "All categories")
            {
                TempData.Remove("list");
            }
            if (questions==null)
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

        public ActionResult clearSearch()
        {
            TempData.Remove("list");
            return RedirectToAction("Index");
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
            QuestionCategories databasecategory = db.categories.ToList().Find(x => x.Category == category);
            Question q = new Question { Title = title, Text = questiontext, Picture = picname, Category = databasecategory, Status = 1, Author = (User)Session["user"], CreationTime = dt };
            db.questions.Add(q);
            db.SaveChanges();
            return Redirect("Index");
        }

        public ActionResult openQuestion(int index)
        {
            Question question = db.questions.ToList().Find(x => x.Id == index);
            List<Reply> replies = null;
            if (question.Replies !=null) { 
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

        [HttpGet]
        public ActionResult giveReply(String text, int questionId)
        {
            DateTime dt = DateTime.Now;
            Question question = db.questions.ToList().Find(x => x.Id == questionId);
            Reply reply = new Reply() { Text = text, ReplyToWhichQuestion = question, ReplyAuthor = (User)Session["user"], Moment = dt, ReplyToWhichReply = null, PlusGrades = 0, MinusGrades = 0, MyChannel = null };
            reply.Grades = new List<Grade>();
            db.replies.Add(reply);
            db.SaveChanges();
            return RedirectToAction("openQuestion", new { index = questionId });
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

        [HttpPost]
        public ActionResult rateReply(String replyId, String value)
        {
            int intReplyId = int.Parse(replyId);
            int intValue = int.Parse(value);
            Reply reply = db.replies.ToList().Find(x => x.Id == intReplyId);
            User rater = (User)Session["user"];
            Grade gg = db.grades.ToList().Find(x => x.ReplyId == reply.Id && x.UserId == rater.Id);
            if (gg!=null)
            {
                return Json(new { success = false, responseText = "The attached file is not supported." }, JsonRequestBehavior.AllowGet);
            }
            if (intValue==1)
            {
                reply.PlusGrades++;
            }
            else
            {
                reply.MinusGrades++;
            }
            Grade g = new Grade() {ReplyId=reply.Id, UserId=((User)Session["user"]).Id, Reply = reply, User = (User)Session["user"], Value = intValue };
            ((User)Session["user"]).Grades.Add(g);
            reply.Grades.Add(g);
            db.grades.Add(g);
            db.SaveChanges();
            return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult lockQuestion(int questionId)
        {
            Question q = db.questions.ToList().Find(x => x.Id == questionId);
            q.Status = 0;
            db.SaveChanges();
            return RedirectToAction("openQuestion", new { index = questionId });
        }

        public ActionResult deleteQuestion(int questionId)
        {
            Question q = db.questions.ToList().Find(x => x.Id == questionId);
            foreach (var reply in db.replies.ToList())
            {
                if (reply.ReplyToWhichQuestion==q)
                {
                    db.replies.Remove(reply);
                }
            }
            db.questions.Remove(q);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult openBuyTokensPage(String message=null)
        {
            ViewBag.purchases = db.purchases.ToList().FindAll(x=>x.Customer.Id==((User)Session["user"]).Id);
            ViewBag.message = message;
            ViewBag.setup = db.setup.First();
            return View("BuyTokens");
        }

        [HttpPost]
        public ActionResult buyTokens(int id)
        {
            Session["tokenID"] = id;
            return RedirectToAction("PaymentWithPaypal", "Client");
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Client/PaymentWithPaypal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var concatenatedString = baseURI + "guid=" + guid;
                    var createdPayment = this.CreatePayment(apiContext, concatenatedString);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid 
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("openBuyTokensPage", "Client", new { message = "unsuccessful" });
                    }
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return RedirectToAction("openBuyTokensPage", "Client", new { message = "unsuccessful"});
            }
            //on successful payment, show success page to user. 
            Setup setup = db.setup.First();
            Purchase purchase = new Purchase();
            User user = (User)Session["user"];
            int id = (int)Session["tokenID"];
            purchase.Customer = user;
            purchase.Moment = DateTime.Now;
            if (id == 1)
            {
                purchase.Amount = setup.AmountSilver;
                purchase.TotalPrice = setup.PriceSilver *setup.AmountSilver;
                user.Tokens += purchase.Amount;
                purchase.Type = "Silver";
            }
            if (id == 2)
            {
                purchase.Amount = setup.AmountGold;
                purchase.TotalPrice = setup.PriceGold * setup.AmountGold;
                user.Tokens += purchase.Amount;
                purchase.Type = "Gold";
            }
            if (id == 3)
            {
                purchase.Amount = setup.AmountPlat;
                purchase.TotalPrice = setup.PricePlat * setup.AmountPlat;
                user.Tokens += purchase.Amount;
                purchase.Type = "Platinum";
            }
            db.purchases.Add(purchase);
            db.SaveChanges();
            return RedirectToAction("openBuyTokensPage", "Client", new { message = "successful"});
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }


        public Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            string price = "";
            string quan = "";
            float total = 0;
            Setup setup = db.setup.First();
            string name = "Token purchase - ";
            int id = (int)Session["tokenID"];
            if (id == 1)
            {
                name += "Silver Package";
                price = setup.PriceSilver.ToString();
                quan = setup.AmountSilver.ToString();
                total = (setup.PriceSilver * setup.AmountSilver);
            }
            if (id == 2)
            {
                name += "Gold Package";
                price = setup.PriceGold.ToString();
                quan = setup.AmountGold.ToString();
                total = (setup.PriceGold * setup.AmountGold);
            }
            if (id == 3)
            {
                name += "Platinum Package";
                price = setup.PricePlat.ToString();
                quan = setup.AmountPlat.ToString();
                total = (setup.PricePlat * setup.AmountPlat);
            }
            //Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = name,
                currency = "USD",
                price = price,
                quantity = quan,
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total.ToString(),
                shipping_discount = "0"
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = itemList
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }
        public ActionResult info(String message)
        {
            ViewBag.message = message;
            return View("info");
        }

        public ActionResult openChannelsPage()
        {
            return View("../Shared/ChannelWindow");
        }

        [HttpPost]
        public PartialViewResult replyToReply(String text, String wc, String wr)
        {
            Question q = db.questions.ToList().Find(x => x.Id == int.Parse(wc));
            Reply r = db.replies.ToList().Find(x => x.Id == int.Parse(wr));
            Reply newr = new Reply() { Text = text, ReplyToWhichQuestion = q, ReplyAuthor = (User)Session["user"], Moment = DateTime.Now, Grades = new List<Grade>(), ReplyToWhichReply = r, PlusGrades = 0, MinusGrades = 0 };
            q.Replies.Add(newr);
            db.replies.Add(newr);
            db.SaveChanges();
            AnswerAnswerClass aac = new AnswerAnswerClass { reply = newr, allReplies = new List<Reply>(), offset = 1 };
            aac.allReplies.Add(newr);
            /*if (r.ReplyToWhichReply==null)
            {
                aac.offset = 1;
            }
            else
            {
                while (r.ReplyToWhichReply != null)
                {
                    aac.offset++;
                    r = r.ReplyToWhichReply;
                }
            }*/
            PartialViewResult pvr = PartialView("OneReply", aac);
            return pvr;
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
    }
}
