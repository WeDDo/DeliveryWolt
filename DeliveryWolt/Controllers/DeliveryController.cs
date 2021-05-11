using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Models;
using MySql.Data.MySqlClient;

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
            // Draw delivey route method

            return displayDeliveryPage(deliveries);
        }


        public List<Delivery> getDeliveries()
        {
            DataTable table = new DataTable();
            List<Delivery> deliveries = new List<Delivery>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM delivery");
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;

            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            foreach (DataRow row in table.Rows)
            {
                deliveries.Add(new Delivery((int)row[0], (double)row[1], (double)row[2], (bool)row[3]));
            }
            databaseConnection.Close();
            return deliveries;
        }

        public ActionResult displayDeliveryPage(List<Delivery> deliveries)
        {
            dynamic model = new ExpandoObject();
            model.deliveries = deliveries;
            return View("DeliveryListPage", model);
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