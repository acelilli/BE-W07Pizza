using BE_W07Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE_W07Pizza.Controllers
{
    public class HomeController : Controller
    {
        readonly ModelDBContext db = new ModelDBContext();
        public ActionResult Index()
        {

            return View(db.Articoli.ToList());
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
    }
}