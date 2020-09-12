namespace Neo4JPayloads.Models
{
    public class Region : ModelBase
    {
        public string seller_id { get; set; }
        public string seller_zip_code_prefix { get; set; }
        public string seller_city { get; set; }
        public string seller_state { get; set; }
    }
}