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
        public string City { get; set; }
        public int Warehouse_id { get; set; }
        public int Delivery_id { get; set; }
        public int Order_by { get; set; }


        public Package()
        {

        }

        public Package(int id, string dimensions, double weight, DateTime due, string address, string status, double costModifier, bool priority, string city, int warehouse_id, int delivery_id, int order_by)
        {
            Id = id;
            Dimensions = dimensions;
            Weight = weight;
            Due = due;
            Address = address;
            Status = status;
            CostModifier = costModifier;
            Priority = priority;
            City = city;
            Warehouse_id = warehouse_id;
            Delivery_id = delivery_id;
            Order_by = order_by;
        }

        public List<string> Statuses = new List<string>()
        {
            "available",
            "reserved",
            "in_transit",
            "unsuccessfuldelivery",
            "lost",
            "delivered"
        };

        public List<string> getStatuses()
        {
            
            return Statuses;
        }

    }
}