using System.Collections.Generic;

namespace Neo4JPayloads.Models
{
    public class Order
    {
        public string order_id { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public string customer_id { get; set; }
        public Customer Customer { get; set; }
        public string order_status { get; set; }
        public string order_purchase_timestamp { get; set; }
        public string order_approved_at { get; set; }
        public string order_delivered_carrier_date { get; set; }
        public string order_delivered_customer_date { get; set; }
        public string order_estimated_delivery_date { get; set; }
    }
}