namespace Neo4JPayloads.Models
{
    public class Customer
    {
        public string customer_id { get; set; }
        public string customer_unique_id { get; set; }
        public string customer_zip_code_prefix { get; set; }
        public string customer_city { get; set; }
        public string customer_state { get; set; }
    }
}