using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliveryWolt.Models;
using MySql.Data.MySqlClient;

namespace DeliveryWolt.Controllers
{
    public class PackageController : Controller
    {
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
                        //PackageList.Add(new Package { Id = 2, Dimensions = "20x5x20", Weight = 10.00, Due = new DateTime(2021, 07, 13), Address = "Student?? Street 67", Status = "Available", CostModifier = 0, Priority = false });
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

        

        //public ActionResult hello()
        //{
        //    return this.Index();
        //}
        //-----------------------------------------------------------------------------------------------------------------
        public List<Package> getWarehouse(int id)
        {
            List<Package> packages = new List<Package>() { }; 
            int temp = select(id);
            Console.WriteLine(temp);
            packages = getPackagesFromWarehouse(temp);

            return packages;
        }

        public string getWarehouseAddress(int warehouseId)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM `warehouse` WHERE id = {0}", warehouseId);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            string warehouseAddress = "";
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
                        warehouseAddress = reader.GetString(1);
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

            return warehouseAddress;
        }

        public int select(int id)
        {
            int idwarehouse = 0;
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM `warehouse_manager` WHERE id = {0}", id);
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
                        idwarehouse = Convert.ToInt32(reader.GetString(2));
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
            return idwarehouse;
        }
        public List<Package> getPackagesFromWarehouse(int id)
        {
            List<Package> PackageList = new List<Package>() { };
            ViewBag.ItemList = "Package Page";

            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("SELECT * FROM `package` WHERE warehouse_id = {0}", id);

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
                            reader.GetBoolean(7),
                            reader.GetString(8),
                            Convert.ToInt32(reader.GetString(9)),
                            Convert.ToInt32(reader.GetString(10)),
                            Convert.ToInt32(reader.GetString(12))
                        ));

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
            return PackageList;
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
        //-------------------------------------------------------------------------------------
        


        //-------------------------------------------------------------------------------------



        public List<Package> getPackages(int delivery_id)
        {
            DataTable table = new DataTable();
            List<Package> packages = new List<Package>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM package WHERE delivery_id={0} ORDER BY Order_by", delivery_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    int del_id;
                    if (row[9] == "")
                    {
                        del_id = 0;
                    }
                    else
                    {
                        del_id = (int)row[9];
                    }

                    int war_id;
                    if (row[10] == "")
                    {
                        war_id = 0;
                    }
                    else
                    {
                        war_id = (int)row[10];
                    }
                    packages.Add(
                        new Package(
                            (int) row[0],
                            (string)row[1],
                            (double)row[2],
                            (DateTime)row[3], 
                            (string)row[4],
                            (string)row[5],
                            (double)row[6],
                            (bool)row[7],
                            (string)row[8],
                            del_id,
                            war_id,
                            (int)row[12]));
                }
            }
            else
            {
                return null;
            }
            databaseConnection.Close();

            return packages;
        }

        //-------------------------------------------------------------------------------------
        public List<Package> getPackages2(int delivery_id)
        {
            DataTable table = new DataTable();
            List<Package> packages = new List<Package>();
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("SELECT * FROM package WHERE reserved_by={0} AND status=\"reserved\" ORDER BY `order_by`", delivery_id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            databaseConnection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    int del_id;
                    if (row[9] == "")
                    {
                        del_id = 0;
                    }
                    else
                    {
                        del_id = (int)row[9];
                    }

                    int war_id;
                    if (row[10] == "")
                    {
                        war_id = 0;
                    }
                    else
                    {
                        war_id = (int)row[10];
                    }
                    packages.Add(
                        new Package(
                            (int)row[0],
                            (string)row[1],
                            (double)row[2],
                            (DateTime)row[3],
                            (string)row[4],
                            (string)row[5],
                            (double)row[6],
                            (bool)row[7],
                            (string)row[8],
                            del_id,
                            war_id,
                            (int)row[12]));
                }
            }
            else
            {
                return packages;
            }
            databaseConnection.Close();

            return packages;
        }

        public void changeState(int id, string status)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = String.Format("UPDATE package SET status = \"{0}\" WHERE id = {1}", status, id);
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            databaseConnection.Open();
            cmd.ExecuteNonQuery();
            databaseConnection.Close();
        }

        //-------------------------------------------------------------------------------------
        public void updateStatus(int id, string status, int? delivery_id)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            string query = "";
            if (delivery_id == null)
            {
                query = String.Format("UPDATE package SET status = \"{0}\", delivery_id = NULL WHERE id = {1}", status, id);
            }
            else
            {
                query = String.Format("UPDATE package SET status = \"{0}\", delivery_id = {2} WHERE id = {1}", status, id, delivery_id);
            }
            
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
            cmd.CommandTimeout = 60;
            databaseConnection.Open();
            cmd.ExecuteNonQuery();
            
            databaseConnection.Close();
        }

        //-------------------------------------------------------------------------------------
        public int getRegionsPackageAmount(string[] regions)
        {
            List<Warehouse> deliveries = new List<Warehouse>();

            List<Package> packagesInRegion = new List<Package>();

            foreach(var region in regions)
            {
                //sql uzklausa 
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
                // Select all
                string query = String.Format("SELECT * FROM package WHERE city = \"{0}\" AND status = \"available\"", region);

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
                            packagesInRegion.Add(new Package(
                                Convert.ToInt32(reader.GetString(0)),
                                reader.GetString(1),
                                reader.GetDouble(2),
                                reader.GetDateTime(3),
                                reader.GetString(4),
                                reader.GetString(5),
                                reader.GetDouble(6),
                                reader.GetBoolean(7),
                                reader.GetString(8),
                                Convert.ToInt32(reader.GetString(9)),
                                Convert.ToInt32(reader.GetString(10)),
                                Convert.ToInt32(reader.GetString(12))
                            ));

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

            }

            int package_amount = packagesInRegion.Count;
            return package_amount;
        }

        //-------------------------------------------------------------------------------------

        //------------------------------------------------------------
        public List<Package> getAvailablePackages(string name)
        {
            List<Package> PackageList = new List<Package>() { };
            ViewBag.ItemList = "Package Page";

            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=deliverywolt;";
            // Select all
            string query = String.Format("SELECT * FROM package WHERE city = \"{0}\" AND status = \"available\"", name);

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
                            reader.GetBoolean(7),
                            reader.GetString(8),
                            Convert.ToInt32(reader.GetString(9)),
                            Convert.ToInt32(reader.GetString(10)),
                            Convert.ToInt32(reader.GetString(12))
                        ));

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
            return PackageList;
        }

        //-------------------------------------------------------------------------------------

        public void getPathBetweenPoints()
        {

        }
    }
    
}

