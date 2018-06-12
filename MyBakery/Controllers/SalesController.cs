using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBakery.Models;

namespace MyBakery.Controllers
{
    public class SalesController : Controller
    {
        BakeryEntities db = new BakeryEntities();
        // GET: Sales
        public ActionResult Index()
        {
            Message m = new Message();
            if (Session["PersonKey"] == null)
            {
                m.text = "Sorry, you need to log in get some bakery.";
                Response.AddHeader("Refresh", "3;url=login");
                return View("Result", m);
            }
            ViewBag.Employees = new SelectList(db.Employees, "EmployeeKey", "EmployeeTitle");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include ="SaleDate, CustomerKey, EmployeeKey")] Sale sale)
        {
            try
            {
                sale.CustomerKey = (int)Session["PersonKey"];
                sale.SaleDate = DateTime.Now;
                db.Sales.Add(sale);
                db.SaveChanges();
                Session["SaleKey"] = sale.SaleKey;
            } catch (Exception e) {}
            return RedirectToAction("Details");
        }

        //GET method
        public ActionResult Details()
        {
            Message m = new Message();
            if (Session["PersonKey"] == null)
            {
                m.text = "Sorry, you need to log in get some bakery.";
                Response.AddHeader("Refresh", "3;url=login");
                return View("Result", m);
            }
            ViewBag.Products = new SelectList(db.Products, "ProductKey", "ProductName");
            return View();
        }

        [HttpPost]
        public ActionResult Details([Bind(Include = "SaleKey, ProductKey, SaleDetailPriceCharged, " +
            "SaleDetailQuantity, SaleDetailDiscount, SaleDetailSaleTaxPercent, SaleDetailEatInTax")]SaleDetail sd)
        {
            Message m = new Message();
            try
            {
                sd.SaleDetailDiscount = 0.1m;
                sd.SaleDetailSaleTaxPercent = 0.1m;
                sd.SaleDetailEatInTax = 0.1m;
                sd.SaleKey = (int)Session["SaleKey"];
                int productKey = (int)sd.ProductKey;
                int quantity = sd.SaleDetailQuantity;
                decimal price = (decimal)(from r in db.Products where r.ProductKey == productKey select r.ProductPrice).FirstOrDefault();
                sd.SaleDetailPriceCharged = price;
                String productName = (from r in db.Products where r.ProductKey == productKey select r.ProductName).FirstOrDefault();
                decimal total = sd.SaleDetailPriceCharged * quantity * 0.9m * 1.2m;
                //how to show this price on view page.
                db.SaleDetails.Add(sd);
                db.SaveChanges();

                String receipt = String.Format("Bakery: {0}, quantity: {1}, total amount: {2}." + Environment.NewLine, productName, quantity, total);
                Session["Order"] += receipt;
                decimal totalAll = 0m;
                if(Session["total"] != null)
                {
                    totalAll = (decimal)Session["total"];
                }
                totalAll += total;
                Session["total"] = totalAll;
                m.text = String.Format("You bought {0} {1} with price: {2}.", quantity, productName, price);
            } catch (Exception e)
            {
                m.text = e.Message;
            }
            return View("Result", m);
        }

        public ActionResult Result(Message m)
        {
            return View(m);
        }

        public ActionResult Receipt()
        {
            return View();
        }
    }
}