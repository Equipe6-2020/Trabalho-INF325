namespace Neo4JPayloads.Models
{
    public class Product : ModelBase
    {
        public string product_id { get; set; }
        public string product_category_name { get; set; }
        public int product_name_lenght { get; set; }
        public int product_description_lenght { get; set; }
        public int product_photos_qty { get; set; }
        public int product_weight_g { get; set; }
        public int product_length_cm { get; set; }
        public int product_height_cm { get; set; }
        public int product_width_cm { get; set; }
    }
}