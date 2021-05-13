﻿using System;
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
            List<Delivery> deliveries = getDeliveries(1);
            PackageController packageController = new PackageController();
            packageController.getPackages(1);
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