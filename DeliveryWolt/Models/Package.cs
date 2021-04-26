using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWolt.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Dimensions { get; set; }
        public double Weight { get; set; }
        public DateTime Due { get; set; }
        public string Address { get; set; }
        public String Status { get; set; }
        public double CostModifier { get; set; }
        public bool Priority { get; set; }
        public int Warehouse_id { get; set; }

        public Package()
        {

        }

        public Package(int id, string dimensions, double weight, DateTime due, string address, string status, double costModifier, bool priority,int warehouse_id)
        {
            Id = id;
            Dimensions = dimensions;
            Weight = weight;
            Due = due;
            Address = address;
            Status = status;
            CostModifier = costModifier;
            Priority = priority;
            Warehouse_id = warehouse_id;

        }

    }

    public enum Status
    {
        available,
        reserved,
        in_transit,
        unsuccessfuldelivery,
        lost,
        delivered
    }
}