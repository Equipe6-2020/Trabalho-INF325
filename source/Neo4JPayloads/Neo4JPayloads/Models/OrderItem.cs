namespace Neo4JPayloads.Models
{
    public class OrderItem
    {
        public string order_id { get; set; }
        public int order_item_id { get; set; }
        public string product_id { get; set; }
        public Product product { get; set; }
        public string seller_id { get; set; }
        public Region seller { get; set; }
        public string shipping_limit_date { get; set; }
        public double price { get; set; }
        public double freight_value { get; set; }
    }
}