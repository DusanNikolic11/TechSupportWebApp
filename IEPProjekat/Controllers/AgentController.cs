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
    public class AgentController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Agent
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

       public ActionResult goToMyProfile()
        {
            ClientController controller = DependencyResolver.Current.GetService<ClientController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
            return controller.goToMyProfile();
        }
    }
}
