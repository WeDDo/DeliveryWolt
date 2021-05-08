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
        public ActionResult DeliveryManPage()
        {
            return View("DeliveryManPage");
        }

        //-------------------------------------------------------------------------------------
        //needs work

        [ActionName("ViewDelivery")]
        public ActionResult openDeliveryView()
        {
            List<Delivery> deliveries = getDeliveries();
            PackageController packageController = new PackageController();
            packageController.getPackages();

            // drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return displayDeliveryPage(deliveries);
        }

        public List<Delivery> getDeliveries()
        {
            List<Delivery> deliveries = new List<Delivery>() {
                new Delivery(0, 10, 10, true, new List<Package>{ new Package(0, "11x11x11", 10, new DateTime(2011,11,11), "AA", "AAA", 12.5, false, 7 )}),
                new Delivery(1, 10, 10, true, new List<Package>{ new Package(0, "11x11x11", 10, new DateTime(2011,11,11), "AA", "AAA", 12.5, false, 7 )}),
                new Delivery(2, 10, 10, true, new List<Package>{ new Package(0, "11x11x11", 10, new DateTime(2011,11,11), "AA", "AAA", 12.5, false, 7 )})
            };

            return deliveries;
        }

        public ActionResult displayDeliveryPage(List<Delivery> deliveries)
        {
            return View("DeliveryListPage", deliveries[0]);
        }

        //-------------------------------------------------------------------------------------

        [ActionName("ChangePackageState")]
        public void changeState()
        {
            PackageController packageController = new PackageController();
            packageController.changeState();
        }

        //-------------------------------------------------------------------------------------
        [ActionName("RemovePackageFromDelivery")]
        public void removePackage()
        {
            PackageController packageController = new PackageController();
            packageController.removePackage();
        }

        //-------------------------------------------------------------------------------------

        [ActionName("ViewPersonalDeliveryList")]
        public ActionResult openPersonalDeliveryView()
        {
            List<Delivery> deliveries = getDeliveries();
            PackageController packageController = new PackageController();
            packageController.getPackages();

            // drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return openPersonalDeliveryListPage(deliveries);
        }

        public ActionResult openPersonalDeliveryListPage(List<Delivery> deliveries)
        {
            return View("DeliveryListPage", deliveries[0]);
        }


        //-------------------------------------------------------------------------------------
        [ActionName("CreateNewDelivery")]
        public ActionResult openCreateNewDelivery()
        {
            List<Delivery> deliveries = getDeliveries();
            PackageController packageController = new PackageController();
            packageController.getPackages();

            // drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return openPersonalDeliveryListPage(deliveries);
        }

        public void getRegionsPackageAmount()
        {
            PackageController packageController = new PackageController();
            int package_amount = packageController.getRegionsPackageAmount();
            if (package_amount == 0)
            {
                //displayNoPackagesNotification()
            }
            else
            {
                //GL HF
            }

        }


        //-------------------------------------------------------------------------------------
        [ActionName("RemoveSomeDeliveries")]
        public void showdeliverymarkingform()
        {

        }


        //-------------------------------------------------------------------------------------
        [ActionName("CreateManualDeliveryList")]
        public ActionResult openManualList()
        {
            List<Delivery> deliveries = getDeliveries();
            PackageController packageController = new PackageController();
            packageController.getPackages();
            // reikia ideti kad grazintu abu kaip atskirus listus
            viewAvaibalePackageListinCity();
            return showPackagesInfo(deliveries);
        }

        public void viewAvaibalePackageListinCity()
        {
            PackageController packageController = new PackageController();
            packageController.getAvailablePackages();

        }


        public ActionResult showPackagesInfo(List<Delivery> deliveries)
        {
            return View("ManualDeliveryPage", deliveries[0]);
        }

        //-------------------------------------------------------------------------------------
        [ActionName("AddPackageToPersonalDeliveryList")]
        public void addPackageToList()
        { 

        }

        //-------------------------------------------------------------------------------------
        [ActionName("RemovePackagesFromManualDeliveryList")]
        public void clearDeliveryList()
        {

        }

        //-------------------------------------------------------------------------------------
        [ActionName("ChangePackageOrder")]
        public void movePackage()
        {

        }

        //-------------------------------------------------------------------------------------
        [ActionName("SaveManualDeliveryList")]
        public void saveDeliveriesLIst()
        {

        }

    }
}