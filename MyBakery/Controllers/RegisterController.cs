using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBakery.Models;

namespace MyBakery.Controllers
{
    public class RegisterController : Controller
    {
        BakeryEntities db = new BakeryEntities();
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "lastName, firstName, email, phone, password")]PersonRegistered pr)
        {
            int result = db.usp_newPerson(pr.lastName, pr.firstName, pr.email, pr.phone, pr.password);
            Message m = new Message();
            if(result == -1)
            {
                m.text = "Sorry, something went wrong. Please register for one more time.";
                Response.AddHeader("Refresh", "3;url=register");
            } else
            {
                m.text = "Thank you for your registration. Welcome to our store. Redirecting to log in page...";
                Response.AddHeader("Refresh", "3;url=login");
            }
            return View("Result", m);
        }

        public ActionResult Result(Message message)
        {
            return View(message);
        }
    }
}