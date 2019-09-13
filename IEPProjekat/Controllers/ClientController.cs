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
            db.questions.Remove(q);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult openBuyTokensPage()
        {
            return View("BuyTokens");
        }

        public ActionResult buyTokens(int id)
        {
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
                        return RedirectToAction("info", "client", new { message = "unsuccessful" });
                    }
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return RedirectToAction("info", "client", new { message = "unsuccessful"});
            }
            //on successful payment, show success page to user. 
            /*
            SysDef sysdef = db.definitions.Find(1);
            Orders order = new Orders();
            User user = (User)Session["user"];
            int id = (int)Session["orderNumber"];
            user = db.users.Find(user.Id);
            order.user = user;
            order.userId = user.Id;
            order.moment = DateTime.Now;
            if (id == 1)
            {
                order.amount = sysdef.silverAmount;
                order.price = sysdef.silverPrice;
                user.tokens += order.amount;
                order.type = Orders.orderType.SILVER;
            }
            if (id == 2)
            {
                order.amount = sysdef.goldAmount;
                order.price = sysdef.goldPrice;
                user.tokens += order.amount;
                order.type = Orders.orderType.GOLD;
            }
            if (id == 3)
            {
                order.amount = sysdef.platinumAmount;
                order.price = sysdef.platinumPrice;
                user.tokens += order.amount;
                order.type = Orders.orderType.PLATINUM;
            }
            db.orders.Add(order);
            db.SaveChanges();
            */
            return RedirectToAction("info", "client", new { message = "successful"});
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
            var name = "kupovina";
            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            //SysDef sysdef = db.definitions.Find(1);
            /*string name = "";
            int id = (int)Session["orderNumber"];
            if (id == 1)
            {
                name = "Silver Package";
            }
            if (id == 2)
            {
                name = "Gold Package";
            }
            if (id == 3)
            {
                name = "Platinum Package";
            }*/
            //Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = name,
                currency = "USD",
                price = "0.4",
                quantity = "1",
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
                tax = "0.1",
                shipping = "0.1",
                subtotal = "0.4",
                shipping_discount = "-0.1"
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = "0.5", // Total must be equal to sum of tax, shipping and subtotal.
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
    }

    
}
