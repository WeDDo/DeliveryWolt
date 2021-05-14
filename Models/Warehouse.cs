using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWolt.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public int Active_workers { get; set; }


        public Warehouse() { }

        public Warehouse(int id, string address, int capacity, int active_workers)
        {
            Id = id;
            Address = address;
            Capacity = capacity;
            Active_workers = active_workers;
        }
    }
}