using Neo4JPayloads.Models;
using Neo4JPayloads.Querys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo4JPayloads.Business
{
    public class ProcessInformationBusiness
    {
        private const int CountData = 100;

        private const string PathProducts = @"C:\scripts\olist_products_dataset.json";
        private const string PathOrderItems = @"C:\scripts\olist_order_items_dataset.json";
        private const string PathSeller = @"C:\scripts\olist_sellers_dataset.json";

        public void Starter()
        {
            var products = GetDataJson<Product>(PathProducts).Take(CountData).ToList();

            var productIds = products.Select(x => x.product_id);
            var orderItems = GetDataJson<OrderItem>(PathOrderItems).Where(x => productIds.Contains(x.product_id)).ToList();

            var sellerIds = orderItems.Select(x => x.seller_id);
            var regions = GetDataJson<Region>(PathSeller)
                .Where(x => sellerIds.Contains(x.seller_id))
                .GroupBy(x => new { x.seller_zip_code_prefix, x.seller_city, x.seller_id })
                .Select(x => new Region
                {
                    seller_zip_code_prefix = x.Key.seller_zip_code_prefix, 
                    seller_city = x.Key.seller_city, 
                    seller_id = x.Key.seller_id
                })
                .ToList();
            
            foreach (var order in orderItems)
            {
                order.product = products.FirstOrDefault(x => x.product_id == order.product_id);
                order.seller = regions.FirstOrDefault(x => x.seller_id == order.seller_id);
            }

            var finalJson = JsonConvert.SerializeObject(orderItems);
        }

        public void GenerateInserts()
        {
            var products = GetDataJson<Product>(PathProducts).Take(CountData).ToList();
            var categories = products.Select(x => x.product_category_name).Distinct();

            var productIds = products.Select(x => x.product_id);
            var orderItems = GetDataJson<OrderItem>(PathOrderItems).Where(x => productIds.Contains(x.product_id)).ToList();

            var sellerIds = orderItems.Select(x => x.seller_id);
            var regions = GetDataJson<Region>(PathSeller)
                .Where(x => sellerIds.Contains(x.seller_id))
                .GroupBy(x => new { x.seller_zip_code_prefix, x.seller_city, x.seller_id })
                .Select(x => new Region { seller_zip_code_prefix = x.Key.seller_zip_code_prefix, seller_city = x.Key.seller_city, seller_id = x.Key.seller_id })
                .ToList();

            var query = new Query();

            var relations = new List<string>();

            foreach (var product in products)
            {
                var traceCategoryToProduct =
                    query.AddTraceBetweenCategoryAndProduct(product.product_id, product.product_category_name);

                var orderItem = orderItems.FirstOrDefault(x => x.product_id == product.product_id);

                if (orderItem == null)
                    throw new NullReferenceException();

                var region = regions.FirstOrDefault(x => x.seller_id == orderItem.seller_id);

                if (region == null)
                    throw new NullReferenceException();

                var traceProductToRegion =
                    query.AddTraceBetweenProductAndRegion(product.product_id, region.seller_zip_code_prefix);

                relations.Add(traceCategoryToProduct);
                relations.Add(traceProductToRegion);
            }

            var final = new List<string>();

            var createProductScripts = query.AddProducts(products);
            var createCategoriesScripts = query.AddCategory(categories);
            var createRegions = query.AddRegion(regions);

            var traces = string.Join("; ", relations);

            final.Add(createProductScripts);
            final.Add(createCategoriesScripts);
            final.Add(createRegions);
            final.Add(traces);

            var finalQueries = string.Join("; ", final);
        }

        private static IEnumerable<T> GetDataJson<T>(string path)
        {
            var file = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<IEnumerable<T>>(file);
        }

    }
}