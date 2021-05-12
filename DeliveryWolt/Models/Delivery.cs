using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWolt.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public double Cost { get; set; }
        public double TotalDistance { get; set; }
        public bool Displayed { get; set; }
        public int Deliveryman_id { get; set; }

        public int DeliveryManid { get; set; }

        public Delivery() { }
        public Delivery(int id, double cost, double totalDistance, bool displayed, int i)
        {
            Id = id;
            Cost = cost;
            TotalDistance = totalDistance;
            Displayed = displayed;
            DeliveryManid = i;
        }

        public Delivery(int id, double cost, double totalDistance, bool displayed, int deliveryman_id)
        {
            Id = id;
            Cost = cost;
            TotalDistance = totalDistance;
            Displayed = displayed;
            Deliveryman_id = deliveryman_id;
        }
    }
}