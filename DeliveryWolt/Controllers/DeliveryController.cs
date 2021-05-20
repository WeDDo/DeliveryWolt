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
using Newtonsoft.Json;

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

        public void sqlcheck(int id, int order)
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


            int package_amount = packageController.getRegionsPackageAmount(regions);
            if (package_amount == 0)
            {
                //displayNoPackagesNotification()
                System.Diagnostics.Debug.WriteLine("NO PACKAGES AVAILABLE...");
            }
            else
            {
                //GL HF

                List<Package> regionPackageList = getRegionsPackageList(regions, packageController); //AVAILABLE PACKAGES IN SELECTED REGIONS
                string warehouseAddress = "Taikos pr. 100C, Kaunas"; // MAKE GETWAREHOUSE TO GET WAREHOUSE BY ID
                string origin = warehouseAddress;
                List<Package> deliveryPackages = new List<Package>();

                //------------------------------------
                Delivery delivery = new Delivery();
                delivery.TotalDistance = 0;
                //------------------------------------

                int totalDistance = 0;
                for (int i = 0; i < regionPackageList.Count; i++)
                {
                    int shortestDistance = int.MaxValue;
                    Package shortestDistancePackage = new Package();
                    for (int j = 0; j < regionPackageList.Count; j++)
                    {
                        if (!deliveryPackages.Contains(regionPackageList[j]))
                        {
                            int distance = getDistanceToPackage(origin, regionPackageList[j].Address + " " + regionPackageList[j].City + "|");
                            if (distance < shortestDistance)
                            {
                                shortestDistance = distance;
                                shortestDistancePackage = regionPackageList[j];
                            }
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Origin BEFORE: " + origin);
                    totalDistance += shortestDistance;
                    System.Diagnostics.Debug.WriteLine("i: " + i + " " + totalDistance + "m");
                    deliveryPackages.Add(shortestDistancePackage);
                    System.Diagnostics.Debug.WriteLine("Delivery Packages Count = " + deliveryPackages.Count());
                    origin = shortestDistancePackage.Address + " " + shortestDistancePackage.City + "|";
                    System.Diagnostics.Debug.WriteLine("Origin AFTER: " + origin);
                }

                delivery.TotalDistance = totalDistance;
                for (int i = 0; i < deliveryPackages.Count; i++)
                {
                    delivery.Cost += deliveryPackages[i].CostModifier * 2;
                    delivery.Deliveryman_id = 1;
                }

                //Insert delivery into the database
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                string query = String.Format("INSERT INTO `delivery`(`cost`, `total_distance`, `display`, `deliveryman_id`) VALUES ('{0}','{1}','{2}','{3}');", delivery.Cost, delivery.TotalDistance, 1, delivery.Deliveryman_id);
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
                commandDatabase.CommandTimeout = 60;
                try
                {
                    databaseConnection.Open();
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    databaseConnection.Close();
                }
                catch (Exception ex) { }

                delivery = getLastDelivery(1);
                System.Diagnostics.Debug.WriteLine(delivery.Id);
                delivery.Packages.AddRange(deliveryPackages);

                //INSERT POINT INTO DATABASE
                string coordinates = getAddressCoordinates(warehouseAddress); //GET WAREHOUSE COORDINATES AND PUT THEM AS THE FIRST(0) POINT
                insertPoint(coordinates, 0, delivery.Id);
                for (int i = 0; i < deliveryPackages.Count; i++)
                {
                    coordinates = getPackageCoordinates(deliveryPackages[i]);
                    insertPoint(coordinates, i + 1, delivery.Id);
                }
                getPoints(delivery.Id);

                
                //Updating each packageState
                for (int i = 0; i < deliveryPackages.Count; i++) 
                {
                    query = String.Format("UPDATE `package` SET `status`= '{0}', `delivery_id`= '{1}', `reserved_by`= '{2}' WHERE `Id`= '{3}'", deliveryPackages[i].Statuses[0], delivery.Id, 1, deliveryPackages[i].Id); //CHANGE LATER TO RESERVED
                    MySqlConnection databaseConnection1 = new MySqlConnection(connectionString);
                    MySqlCommand commandDatabase1 = new MySqlCommand(query, databaseConnection1);
                    commandDatabase.CommandTimeout = 60;
                    MySqlDataReader reader;
                    try
                    {
                        databaseConnection1.Open();
                        reader = commandDatabase1.ExecuteReader();
                        databaseConnection1.Close();
                    }
                    catch (Exception ex) { }
                }
                
            }
        }

        public void insertPoint(string coordinates, int queue_nr, int delivery_id)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("INSERT INTO `point`(`coordinates`, `queue_nr`, `delivery_id`) VALUES ('{0}','{1}','{2}');", coordinates, queue_nr, delivery_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader();
                databaseConnection.Close();
            }
            catch (Exception ex) { }
        }

        public List<Point> getPoints(int delivery_id)
        {
            List<Point> points = new List<Point>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM point WHERE delivery_id={0} ORDER BY queue_nr", delivery_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();
                Point point;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        point = new Point
                        {
                            Id = reader.GetInt32(0),
                            Coordinates = reader.GetString(1),
                            Queue_nr = reader.GetInt32(2),
                            Delivery_id = reader.GetInt32(3)
                        };
                        points.Add(point);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No rows found!");
                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return points;
        }

        public int getDistanceToPackage(string origin, string destination)
        {
            string requestUri = string.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric&origins={1}&destinations={0}&key=AIzaSyD0fJTwlRylJMp5EdC-gfdAfLgI8G9BaXk", Uri.EscapeDataString(destination), Uri.EscapeDataString(origin));
            System.Diagnostics.Debug.WriteLine(requestUri);
            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());
            XElement result = xdoc.Element("DistanceMatrixResponse").Element("row");

            int distance = 0;
            XElement distanceElement = result.Element("element").Element("distance");
            if (distanceElement != null)
            {
                distance = Int32.Parse(distanceElement.Element("value").Value);
            }
            return distance;
        }

        public List<Package> getRegionsPackageList(string[] regions, PackageController packageController)
        {
            List<Package> regionPackageList = new List<Package>();
            for (int i = 0; i < regions.Count(); i++)        //AVAILABLE PACKAGES IN SELECTED REGIONS
            {
                regionPackageList.AddRange(packageController.getAvailablePackages(regions[i]));
            }
            return regionPackageList;
        }

        public string getPackageCoordinates(Package package)
        {
            string address = package.Address + " " + package.City;
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

            return coordinates;
        }

        public string getAddressCoordinates(string address)
        {
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

            return coordinates;
        }

        public List<Package> addInitialDeliveries(List<string> packageCoordinates, List<Package> regionPackageList)
        {
            List<Package> deliveryPackages = new List<Package>();
            for (int i = 0; i < packageCoordinates.Count && deliveryPackages.Count <= 5; i++) //ADD TO DELIVERY LIST UNTIL SET AMOUNT
            {
                //System.Diagnostics.Debug.WriteLine(packageCoordinates[i]);
                deliveryPackages.Add(regionPackageList[i]);
            }
            return deliveryPackages;
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
            Delivery delivery = getLastDelivery(1);
            List<Package> deliveryPackages = getDeliveryPackages(delivery);
            List<Point> packagePoints = getPoints(delivery.Id);
            delivery.Packages.AddRange(deliveryPackages);
            delivery.SetPoints(packagePoints);

            string[] coordinates = new string[packagePoints.Count];
            for (int i = 0; i < packagePoints.Count; i++)
            {
                coordinates[i] = packagePoints[i].Coordinates;
            }

            ViewData["PointList"] = coordinates;

            return View("MapView");
        }

        [ActionName("CreateDelivery")]
        public ActionResult createDelivery()
        {
            string[] regions = new string[] { "Kaunas" };
            getRegionsPackageAmount(regions);

            return openDeliveryView();
        }

        public List<Package> getDeliveryPackages(Delivery delivery)
        {
            int delivery_id = 19; //delivery.Id;
            List<Package> packages = new List<Package>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = "SELECT * FROM `package` WHERE `delivery_id`= " + delivery_id;
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            Package package;
            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();
                // Success, now list 

                // If there are available rows
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        package = new Package
                        {
                            Id = reader.GetInt32(0),
                            Dimensions = reader.GetString(1),
                            Weight = reader.GetDouble(2),
                            Due = reader.GetDateTime(3),
                            Address = reader.GetString(4),
                            Status = reader.GetString(5),
                            CostModifier = reader.GetDouble(6),
                            Priority = Convert.ToBoolean(reader.GetInt32(7)),
                            City = reader.GetString(8),
                            Warehouse_id = reader.GetInt32(9),
                            Delivery_id = reader.GetInt32(10),
                            Order_by = reader.GetInt32(11)
                        };
                        packages.Add(package);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                foreach (var item in packages)
                {
                    System.Diagnostics.Debug.WriteLine(item.Id);
                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return packages;
        }
    }
}