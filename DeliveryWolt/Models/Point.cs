using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWolt.Models
{
    public class Point
    {
        public int Id { get; set; }
        public string Coordinates { get; set; }
        public int Queue_nr { get; set; }
        public int Delivery_id { get; set; }

        public Point() { }

        public Point(int id, string coordinates, int queue_nr, int delivery_id)
        {
            Id = id;
            Coordinates = coordinates;
            Queue_nr = queue_nr;
            this.Delivery_id = delivery_id;
        }
    }
}