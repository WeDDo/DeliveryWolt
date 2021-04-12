using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Models;
using MySql.Data.MySqlClient;

namespace DeliveryWolt.Controllers
{
    public class PackageController : Controller
    {
        List<Package> PackageList = new List<Package>(){};

        // GET: Delivery
        public ActionResult Index() //openPackageList()
        {
            ViewBag.ItemList = "Package Page";

            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = "SELECT * FROM package";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

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
                        PackageList.Add(new Package(
                            Convert.ToInt32(reader.GetString(0)),
                            reader.GetString(1),
                            reader.GetDouble(2),
                            reader.GetDateTime(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetDouble(6),
                            reader.GetBoolean(7) 
                        ));
                        //PackageList.Add(new Package { Id = 2, Dimensions = "20x5x20", Weight = 10.00, Due = new DateTime(2021, 07, 13), Address = "Studentų Street 67", Status = "Available", CostModifier = 0, Priority = false });
                        // Example to save in the listView1 :
                        //string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        //var listViewItem = new ListViewItem(row);
                        //listView1.Items.Add(listViewItem);
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


            return View(PackageList.OrderBy(p => p.Id).ToList());
        }

        // GET: Delivery/Details/5
        public ActionResult Details(int id)
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
                            Priority = reader.GetBoolean(7)
                        };
                        //PackageList.Add(new Package { Id = 2, Dimensions = "20x5x20", Weight = 10.00, Due = new DateTime(2021, 07, 13), Address = "Studentų Street 67", Status = "Available", CostModifier = 0, Priority = false });
                        // Example to save in the listView1 :
                        //string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        //var listViewItem = new ListViewItem(row);
                        //listView1.Items.Add(listViewItem);
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

        // GET: Delivery/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: Delivery/Create
        [HttpPost]
        public ActionResult Add(Package pack)
        {
            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                string query = String.Format("INSERT INTO `package`(`dimensions`, `weight`, `due`, `address`, `status`, `cost_modifier`, `priority`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", pack.Dimensions, pack.Weight, pack.Due, pack.Address, pack.Status, pack.CostModifier, pack.Priority);
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
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Delivery/Edit/5
        public ActionResult Edit(int id)
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
                            Priority = Convert.ToBoolean(reader.GetInt32(7))
                        };
                        //PackageList.Add(new Package { Id = 2, Dimensions = "20x5x20", Weight = 10.00, Due = new DateTime(2021, 07, 13), Address = "Studentų Street 67", Status = "Available", CostModifier = 0, Priority = false });
                        // Example to save in the listView1 :
                        //string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        //var listViewItem = new ListViewItem(row);
                        //listView1.Items.Add(listViewItem);
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

        // POST: Delivery/Edit/5
        [HttpPost]
        public ActionResult Edit(Package pack)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("UPDATE `package` SET `Id`= '{0}',`dimensions`= '{1}',`weight`= '{2}',`due`= '{3}',`address`= '{4}',`status`= '{5}',`cost_modifier`= '{6}',`priority`= '{7}' WHERE `Id`= '{8}'", pack.Id, pack.Dimensions, pack.Weight, pack.Due, pack.Address, pack.Status, pack.CostModifier, pack.Priority, pack.Id);
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
            return RedirectToAction("Index");
        }

        public ActionResult hello()
        {
            return this.Index();
        }
    }
}

