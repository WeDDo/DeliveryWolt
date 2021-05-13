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
            Delivery delivery = getLastDelivery(1);
            PackageController package = new PackageController();

            List<Package> packages = package.getPackages(delivery.Id);

            // Draw delivey route method

            return displayDeliveryPage(delivery, packages);
        }
        public Delivery getLastDelivery(int deliveryman_id)
        {
            DataTable table = new DataTable();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM delivery WHERE deliveryman_id={0} ORDER BY id DESC LIMIT 1", deliveryman_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                Delivery delivery = new Delivery((int)row[0], (double)row[1], (double)row[2], (bool)row[3], (int)row[4]);
                databaseConnection.Close();
                return delivery;
            }
            else
            {
                //DataRow row = table.Rows[0];
                //Delivery delivery = new Delivery((int)row[0], (double)row[1], (double)row[2], (bool)row[3], (int)row[4]);
                //databaseConnection.Close();
                return null;

            }

        }

        public ActionResult displayDeliveryPage(Delivery delivery, List<Package> packages)
        {
            dynamic model = new ExpandoObject();
            List<Delivery> del = new List<Delivery>();
            del.Add(delivery);
            model.delivery = del;
            model.packages = packages;
            return View("DeliveryListPage", model);
        }

        //-------------------------------------------------------------------------------------

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
                deliveries.Add(new Delivery((int)row[0], (double)row[1], (double)row[2], (bool)row[3], (int)row[0]));
            }
            databaseConnection.Close();
            return deliveries;
        }



        //-------------------------------------------------------------------------------------
        public ActionResult changeState(int id, string dropdown)
        {
            PackageController packageController = new PackageController();
            packageController.changeState(id, dropdown);
            return openDeliveryView();
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
            packageController.getPackages(1);

            // drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return openPersonalDeliveryListPage(deliveries);
        }

        public ActionResult openPersonalDeliveryListPage(List<Delivery> deliveries)
        {
            return View("DeliveryListPage", deliveries[0]);
        }


        //-------------------------------------------------------------------------------------
        public ActionResult addPackageToList(int id)
        {
            int idworker = 1;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `reserved_by`= '{0}',`status`= 'reserved' WHERE `Id`= '{1}' AND status = '{2}'", idworker, id,"available");
            System.Diagnostics.Debug.WriteLine(query);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();

                // Succesfully updated

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Ops, maybe the id doesn't exists ?
            }
            return openManualList();
        }
        //------------------------------------------------------------
        [ActionName("CreateNewDelivery")]
        public ActionResult openCreateNewDelivery()
        {
            List<Delivery> deliveries = getDeliveries();
            PackageController packageController = new PackageController();
            packageController.getPackages(1);

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
            int id = 1;

            List<Package> packagesincity = new List<Package>();
            List<Package> personal = new List<Package>();
            PackageController packageController = new PackageController();
            personal = packageController.getPackages2(id);

            
            // reikia ideti kad grazintu abu kaip atskirus listus
            packagesincity = viewAvaibalePackageListinCity();
            return showPackagesInfo(packagesincity,personal);
        }

        public List<Package> viewAvaibalePackageListinCity()
        {
            PackageController packageController = new PackageController();
            List<Package> packages = new List<Package>();
            packages = packageController.getAvailablePackages("Kaunas");
            return packages;
        }


        public ActionResult showPackagesInfo(List<Package> packages,List<Package> personal)
        {
            dynamic model = new ExpandoObject();
            model.packages = packages;
            model.personalpackages = personal;
            return View("ManualDeliveryPage", model);
        }

        //-------------------------------------------------------------------------------------
        [ActionName("AddPackageToPersonalDeliveryList")]
        public void addPackageToList()
        { 

        }

        //-------------------------------------------------------------------------------------

        public ActionResult clearDeliveryList(int id)
        {
            clearDelivery(id);
            return openManualList();
        }

        public void clearDelivery(int id)
        {
            int idworker = 1;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `reserved_by`= '0',`status`= 'available' WHERE `Id`= '{0}'", id);
            System.Diagnostics.Debug.WriteLine(query);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();

                // Succesfully updated

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Ops, maybe the id doesn't exists ?
            }
         
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