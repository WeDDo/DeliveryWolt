using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Controllers;

namespace DeliveryWolt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PackageController controller = new PackageController();
            
            return View();
        }

        public ActionResult OpenPackageList()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}