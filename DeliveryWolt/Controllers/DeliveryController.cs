using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
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
            //getRegionsPackageAmount("Kaunas");   //FOR TESTING MAIN ALGOSTART ITS A BROKEN 
            Delivery delivery = getLastDelivery(1);
            PackageController package = new PackageController();
            List<Package> packages = new List<Package>();

            if (delivery != null)
            {
                List<Package> temp = package.getPackages(delivery.Id);
                if (temp != null)
                {
                    packages = temp;

                }
            }
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
            string query = String.Format("SELECT * FROM delivery WHERE deliveryman_id={0} ORDER BY id", deliveryman_id);
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

            List<Package> packages = new List<Package>();
            foreach (Delivery del in deliveries)
            {
                List<Package> pack = package.getPackages(del.Id);
                if (pack != null)
                {
                    foreach (Package pk in pack)
                    {
                        packages.Add(pk);
                    }
                }
            }

            

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
            sqlcheck(id, order);
            return openManualList();
        }

        public void sqlcheck(int id,int order)
        {
            int idworker = 1;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `reserved_by`= '{0}',`status`= 'reserved', order_by = {3} WHERE `Id`= '{1}' AND status = '{2}'", idworker, id, "available", order + 1);
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

        public void getRegionsPackageAmount(string[] regions)
        {
            PackageController packageController = new PackageController();
            List<string> packageCoordinates = new List<string>();

            int package_amount = packageController.getRegionsPackageAmount(regions);
            if (package_amount == 0)
            {
                //displayNoPackagesNotification()
                System.Diagnostics.Debug.WriteLine("EMTPY");
            }
            else
            {
                Delivery delivery = new Delivery();
                delivery.TotalDistance = 0;

                List<Package> deliveryPackages = new List<Package>();
                List<Package> regionPackageList = new List<Package>();
                //GL HF
                for (int i = 0; i < regions.Count(); i++)        //AVAILABLE PACKAGES IN SELECTED REGIONS
                {
                    regionPackageList.AddRange(packageController.getAvailablePackages(regions[i]));
                }
                for(int i = 0; i < regionPackageList.Count; i++) //GEOLOCATION 
                {
                    string address = regionPackageList[i].City;
                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), "AIzaSyD0fJTwlRylJMp5EdC-gfdAfLgI8G9BaXk");
                    System.Diagnostics.Debug.WriteLine(requestUri);
                    WebRequest request = WebRequest.Create(requestUri);
                    WebResponse response = request.GetResponse();
                    XDocument xdoc = XDocument.Load(response.GetResponseStream());

                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement locationElement = result.Element("geometry").Element("location");
                    XElement lat = locationElement.Element("lat");
                    XElement lng = locationElement.Element("lng");

                    string coordinates = lat.Value + "," + lng.Value;
                    packageCoordinates.Add(coordinates);

                    System.Diagnostics.Debug.WriteLine("NOT EMPTY");
                }
                for (int i = 0; i < packageCoordinates.Count && deliveryPackages.Count <= 5; i++) //ADD TO DELIVERY LIST UNTIL SET AMOUNT
                {
                    //System.Diagnostics.Debug.WriteLine(packageCoordinates[i]);
                    deliveryPackages.Add(regionPackageList[i]);
                }

                string origin = deliveryPackages[0].City.ToString(); //CHANGE CITY TO ADDRESS AND CITY WHEN FULLY WORKING
                string destinations = "";
                for (int i = 1; i < deliveryPackages.Count; i++) //PAKEISTI ATGAL i = 1, kad pirmas butu origin for testing padariau sitaip dabar
                {
                    destinations += deliveryPackages[i].City.ToString() + "|";
                }
                System.Diagnostics.Debug.WriteLine("ORIGIN: " + origin);
                System.Diagnostics.Debug.WriteLine("DESTINATIONS: " + destinations);
                string requestUri1 = string.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric&origins={1}&destinations={0}&key=AIzaSyD0fJTwlRylJMp5EdC-gfdAfLgI8G9BaXk", Uri.EscapeDataString(destinations), Uri.EscapeDataString(origin));
                System.Diagnostics.Debug.WriteLine(requestUri1);
                WebRequest request1 = WebRequest.Create(requestUri1);
                WebResponse response1 = request1.GetResponse();
                XDocument xdoc1 = XDocument.Load(response1.GetResponseStream());
                XElement result1 = xdoc1.Element("DistanceMatrixResponse").Element("row");

                List<string> distances = new List<string>();
                foreach (var item in result1.Elements("element"))
                {
                    XElement distanceElement = item.Element("distance");
                    if (distanceElement != null)
                    {
                        distances.Add(distanceElement.Element("value").Value.ToString());
                        System.Diagnostics.Debug.WriteLine("ADDED");
                    }
                }
                System.Diagnostics.Debug.WriteLine(distances.Count);
                System.Diagnostics.Debug.WriteLine(distances[0] + " " + distances[1]);
                //INSERT NEW DELIVERY AND UPDATE PACKAGE DELIVERY_ID IN SQL AND CODE
                for (int i = 1; i < deliveryPackages.Count; i++)
                {
                    delivery.Cost += deliveryPackages[i].CostModifier;
                    delivery.Deliveryman_id = 1;
                }
                for (int j = 0; j < distances.Count; j++)
                {
                    delivery.TotalDistance += double.Parse(distances[j]);
                }
                //INSERTING DELIVERY INTO DATABASE --->
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                string query = String.Format("INSERT INTO `delivery`(`cost`, `total_distance`, `display`, `deliveryman_id`) VALUES ('{0}','{1}','{2}','{3}');", delivery.Cost, delivery.TotalDistance, 1, delivery.Deliveryman_id);
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
                commandDatabase.CommandTimeout = 60;

                try
                {
                    databaseConnection.Open();
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    //Successful add
                    databaseConnection.Close();
                }
                catch (Exception ex)
                {
                    // Show any error message.
                }

                // SELECT * FROM Table ORDER BY ID DESC LIMIT 1 <-- select last created delivery from database to get id and pass it to package
                delivery = getLastDelivery(1);
                System.Diagnostics.Debug.WriteLine(delivery.Id);


                for (int i = 0; i < deliveryPackages.Count; i++) //UPDATE EACH PACKAGE STATE
                {
                    query = String.Format("UPDATE `package` SET `status`= '{0}', `delivery_id`= '{1}', `reserved_by`= '{2}' WHERE `Id`= '{3}'", deliveryPackages[i].Statuses[1], delivery.Id, 1, deliveryPackages[i].Id); //CHANGE PACKAGE STATE TO RESERVED

                    MySqlConnection databaseConnection1 = new MySqlConnection(connectionString);
                    MySqlCommand commandDatabase1 = new MySqlCommand(query, databaseConnection1);
                    commandDatabase.CommandTimeout = 60;
                    MySqlDataReader reader;

                    try
                    {
                        databaseConnection1.Open();
                        reader = commandDatabase1.ExecuteReader();

                        // Succesfully updated

                        databaseConnection1.Close();
                    }
                    catch (Exception ex)
                    {
                        // Ops, maybe the id doesn't exists ?
                    }
                }
                
            }
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
            return showPackagesInfo(packagesincity, personal);
        }

        public List<Package> viewAvaibalePackageListinCity()
        {
            PackageController packageController = new PackageController();
            List<Package> packages = new List<Package>();
            packages = packageController.getAvailablePackages("Kaunas");
            return packages;
        }


        public ActionResult showPackagesInfo(List<Package> packages, List<Package> personal)
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
        
        public ActionResult saveDeliveriesList(int del_id, double dist, double cost)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("INSERT INTO `delivery`(`cost`, `total_distance`, `display`, `deliveryman_id`) VALUES ('{0}','{1}','{2}','{3}');", cost, dist, 1, del_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                //Successful add

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.

            }

            DataTable table = new DataTable();
            query = String.Format("SELECT `id` FROM `delivery` WHERE `deliveryman_id` = '{0}' ORDER BY `id` DESC LIMIT 1", del_id);
            commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(commandDatabase);
            adapter.Fill(table);
            int id = 0;
            foreach (DataRow row in table.Rows)
            {
                id = (int)row[0];
            }
            databaseConnection.Close();
            query = "";

            PackageController packageController = new PackageController();
            List<Package> pack = packageController.getPackages2(del_id);

            foreach (Package package in pack)
            {
                query = query + String.Format("UPDATE `package` SET `delivery_id`='{0}', status=\"in_transit\" WHERE `Id` = '{1}';", id, package.Id);
            }


            commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                //Successful add

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.

            }


            return openManualList();
        }

        public ActionResult deleteDeliveries(IEnumerable<int> ids)
        {
            if (ids != null)
            {
                string s = "";
                if (ids != null)
                {
                    s = string.Join(",", ids);
                }

                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                string query = String.Format("UPDATE delivery SET display = 0 WHERE id in ({0})", s);
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                cmd.CommandTimeout = 60;
                databaseConnection.Open();
                cmd.ExecuteNonQuery();
                databaseConnection.Close();
            }

            return openManualList();
        }

        [ActionName("ViewMap")]
        public ActionResult viewDeliveryMap()
        {
            return View("MapView");
        }

        [ActionName("AddToDelivery")]
        public ActionResult addToDeliveryList()
        {
            string[] regions = new string[] { "Kaunas", "Vilnius" };
            getRegionsPackageAmount(regions);

            return openDeliveryView();
        }
    }
}