using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Neo4JPayloads.Models;

namespace Neo4JPayloads.Querys
{
    public class Query : QueryBase
    {
        private const string Destination = "DESTINO";
        private const string Belongs = "PERTENCE";

        public string AddTraceBetweenProductAndRegion(string productId, string zipCode) =>
            $"match (p:Product),(r:Region) where p.sysid='{productId}' and r.sysid='{zipCode}' merge (p)-[:{Destination}]->(r)";

        public string AddTraceBetweenCategoryAndProduct(string productId, string categoryName) =>
            $"match (c:Category),(p:Product) where p.sysid='{productId}' and c.sysid='{categoryName}' merge (c)-[:{Belongs}]->(p)";

        public string AddProducts(IEnumerable<Product> products)
        {
            var queries = new List<string>();

            products.ToList()
                .ForEach(product =>
                    queries.Add($@"MERGE (s{GetLabels(product)} {{sysid: '{product.product_id}'}}) on create set s.sysid='{product.product_id}' set s.name='{product.product_id}'"));

            return string.Join("; ", queries);
        }

        public string AddRegion(IEnumerable<Region> regions)
        {
            var queries = new List<string>();

            regions.ToList()
                .ForEach(region =>
                    queries.Add($@"MERGE (s{GetLabels(region)} {{sysid: '{region.seller_zip_code_prefix}'}}) on create set s.sysid='{region.seller_zip_code_prefix}' set s.name='{region.seller_zip_code_prefix}'"));

            return string.Join("; ", queries);
        }

        public string AddCategory(IEnumerable<string> categories)
        {
            var queries = new List<string>();

            categories.ToList()
                .ForEach(category => queries.Add($@"MERGE (s:Category {{sysid: '{category}'}}) on create set s.sysid='{category}' set s.name='{category}'"));

            return string.Join("; ", queries);
        }
            
    }
}