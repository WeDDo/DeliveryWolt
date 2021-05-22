using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Models;
using MySql.Data.MySqlClient;

namespace DeliveryWolt.Controllers
{
    public class WHMController : Controller
    {
        PackageController packageController = new PackageController() { };
        
        public ActionResult index() 
        {
            return View();
        }

        [ActionName("PackageList")]
        public ActionResult openPackageList()
        {
            int id = 7;
            List<Package> packages = new List<Package>() { };
            packages = packageController.getWarehouse(id);
            return openPackageListView(packages);

        }

        public ActionResult openPackageListView(List<Package> packages)
        {
            return View("PackageList",packages.OrderBy(p => p.Id).ToList());
        }

        public ActionResult registerPackage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult registerPackage(Package pack)
        {
            bool la = checkInfo(pack);
            if (la == true)
            {
                return addPackage(pack);
            }
            else return View();

        }
        public ActionResult addPackage(Package pack)
        {
            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                string query = String.Format("INSERT INTO `package`(`dimensions`, `weight`, `due`, `address`, `status`, `cost_modifier`, `priority`,`warehouse_id`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", pack.Dimensions, pack.Weight, pack.Due, pack.Address, pack.Status, pack.CostModifier, pack.Priority, 1);
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
                return RedirectToAction("PackageList");
            }
            catch
            {
                return View();
            }
        }
        public bool checkInfo(Package pack) {
            if (pack.Address  != null)
            {
                return true;
            }
            return false;
        }
        [HttpPost]
        public ActionResult changeState(Package pack)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `status`= '{0}' WHERE `Id`= '{1}'", pack.Status,pack.Id);
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
            return RedirectToAction("PackageList");
          
        }
        public ActionResult changeState(int id)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            //string query = String.Format("UPDATE `package` SET `Id`= '[value-1]',`dimensions`= '[value-2]',`weight`= '[value-3]',`due`= '[value-4]',`address`= '[value-5]',`status`= '[value-6]',`cost_modifier`= '[value-7]',`priority`= '[value-8]' WHERE 1");
            string query = "SELECT * FROM `package` WHERE `Id`= " + id;

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;


            Package package = new Package();
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
                            Warehouse_id = reader.GetInt32(8)

                        };
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return View(package);
        }
    }   
}

