using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Models;

namespace DeliveryWolt.Controllers
{
    public class DeliveryController : Controller
    {
        List<Delivery> deliveries = new List<Delivery>() {
                new Delivery(0, 10, 10, true, new List<Package>{ new Package(0, "11x11x11", 10, new DateTime(2011,11,11), "AA", "AAA", 12.5, false, 7 )})
            };

        // GET: Delivery
        public ActionResult DeliveryManPage()
        {
            return View();
        }

        [ActionName("DeliveryView")]
        public ActionResult openDeliveryView()
        {
            Delivery delivery = getDeliveries()[0];

            return displayDeliveryPage(delivery);
        }

        public List<Delivery> getDeliveries()
        {
            return deliveries;
        }

        public ActionResult displayDeliveryPage(Delivery delivery)
        {
            return View("DeliveryView", delivery);
        }
    }
}