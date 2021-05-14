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
            model.viewDel = "ViewDelivery";
            return View("DeliveryListPage", model);
        }



        //-------------------------------------------------------------------------------------

        public List<Delivery> getDeliveries(int deliveryman_id)
        {
            DataTable table = new DataTable();
            List<Delivery> delivery = new List<Delivery>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM delivery WHERE deliveryman_id={0} ORDER BY id DESC LIMIT 1", deliveryman_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;

            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            foreach (DataRow row in table.Rows)
            {
                delivery.Add(new Delivery((int)row[0], (double)row[1], (double)row[2], (bool)row[3], (int)row[4]));
            }
            databaseConnection.Close();
            return delivery;
        }



        //-------------------------------------------------------------------------------------

        public ActionResult removePackage(int id)
        {
            PackageController controller = new PackageController();
            controller.updateStatus(id, "available", null);

            return openDeliveryView();
        }


        public ActionResult changeState(int id, string dropdown)
        {
            PackageController packageController = new PackageController();
            packageController.changeState(id, dropdown);
            return openDeliveryView();
        }


        //-------------------------------------------------------------------------------------

        [ActionName("ViewPersonalDeliveryList")]
        public ActionResult openPersonalDeliveryView()
        {
            List<Delivery> deliveries = getDeliveries(1);
            PackageController package = new PackageController();

            List<Package> packages = package.getPackages(deliveries[0].Id);


            //drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return openPersonalDeliveryListPage(deliveries, packages);
        }

        public ActionResult openPersonalDeliveryListPage(List<Delivery> deliveries, List<Package> packages)
        {

            dynamic model = new ExpandoObject();
            model.delivery = deliveries;
            model.packages = packages;
            model.viewDel = "ViewPersonalDeliveryList";
            return View("DeliveryListPage", model);
        }


        //-------------------------------------------------------------------------------------
        public ActionResult addPackageToList(int id, int order)
        {
            int idworker = 1;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `reserved_by`= '{0}',`status`= 'reserved', order_by = {3} WHERE `Id`= '{1}' AND status = '{2}'", idworker, id,"available", order+1);
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
            List<Delivery> deliveries = getDeliveries(1);
            PackageController packageController = new PackageController();
            packageController.getPackages(1);
            PackageController package = new PackageController();

            List<Package> packages = package.getPackages(deliveries[0].Id);
            // drawDeliveryRoute method
            //displayDeliveryPage(deliveries);

            return openPersonalDeliveryListPage(deliveries, packages);
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

        public ActionResult clearDeliveryList(int id, int order, int res_by)
        {
            clearDelivery(id, order, res_by);
            return openManualList();
        }

        public void clearDelivery(int id, int order, int res_by)
        {
            int idworker = 1;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `order_by` = `order_by`-1 WHERE `order_by` > '{1}' AND `reserved_by`= '{2}'; UPDATE `package` SET `reserved_by`= '0',`status`= 'available' WHERE `Id`= '{0}'", id, order, res_by);
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
        public ActionResult movePackage(int wh, int id, int res_by, int order)
        {
            changePackageOrder(wh, id, res_by, order);
            return openManualList();
        }

        public void changePackageOrder(int wh, int id, int res_by, int order)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            int order_new = order + wh;


            string query = String.Format("UPDATE `package` SET `order_by` = '{0}' WHERE `order_by` = '{1}' AND `reserved_by`= '{2}'; UPDATE `package` SET `order_by` = '{1}' WHERE `Id`= '{3}'", order, order_new, res_by, id);
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
        [ActionName("SaveManualDeliveryList")]
        public void saveDeliveriesLIst()
        {

        }

    }
}