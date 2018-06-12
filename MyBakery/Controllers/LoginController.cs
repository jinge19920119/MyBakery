using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBakery.Models;

namespace MyBakery.Controllers
{
    public class LoginController : Controller
    {
        BakeryEntities db = new BakeryEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "email, password")] PersonLogin pl)
        {
            int res = db.usp_Login(pl.email, pl.password);
            int personKey = 0;
            Message message = new Message();
            bool isEmployee = false;
            if(res == -1)
            {
                message.text = "Sorry, your email/password is incorrect. Please try it one more time.";
                Response.AddHeader("Refresh", "3;url=login");
            } else
            {
                var personItem = (from r in db.People where r.PersonEmail.Equals(pl.email) select r).FirstOrDefault();
                personKey = (int)personItem.PersonKey;
                var firstName = personItem.PersonFirstName;
                var emKey = (from e in db.Employees where e.PersonKey == personKey select e.EmployeeKey).FirstOrDefault();
                isEmployee = ((int)emKey)==personKey;
                //TODO: Not try with an employee account.

                if(isEmployee)
                {
                    message.text = String.Format("Welcome employee {0}, you can go to check the menu.", firstName);
                    //Response.AddHeader("Refresh", "3;url=product");
                } else
                {
                    message.text = String.Format("Welcome customer {0}, get some delicous bakery with good price.", firstName);
                    Response.AddHeader("Refresh", "3;url=sales");
                }
                Session["PersonKey"] = personKey;
                Session["isEmployee"] = isEmployee;
            }
            return View("Result", message);
        }

        public ActionResult Result(Message m)
        {
            return View(m);
        }
    }
}