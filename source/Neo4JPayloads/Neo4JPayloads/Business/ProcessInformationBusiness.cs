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
        private const int LimitData = 100000;

        private const string PathProducts = @"C:\scripts\olist_products_dataset.json";
        private const string PathOrderItems = @"C:\scripts\olist_order_items_dataset.json";
        private const string PathOrders = @"C:\scripts\olist_orders_dataset.json";
        private const string PathCustomers = @"C:\scripts\olist_customers_dataset.json";
        private const string PathSeller = @"C:\scripts\olist_sellers_dataset.json";

        public void Starter()
        {
            var orders = GetDataJson<Order>(PathOrders).Take(LimitData).ToList();
            var orderIds = orders.Select(x => x.order_id);
            var customerIds = orders.Select(x => x.customer_id);

            var customers = GetDataJson<Customer>(PathCustomers).Where(x => customerIds.Contains(x.customer_id)).ToList();

            var orderItems = GetDataJson<OrderItem>(PathOrderItems).Where(x => orderIds.Contains(x.order_id)).ToList();
            var productsIds = orderItems.Select(x => x.product_id).ToList();

            var products = GetDataJson<Product>(PathProducts).Where(x => productsIds.Contains(x.product_id)).ToList();

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

            foreach (var orderItem in orderItems)
            {
                orderItem.product = products.FirstOrDefault(x => x.product_id == orderItem.product_id);
                orderItem.seller = regions.FirstOrDefault(x => x.seller_id == orderItem.seller_id);
            }

            foreach (var order in orders)
            {
                order.Customer = customers.FirstOrDefault(x => x.customer_id == order.customer_id);
                order.OrderItems = orderItems.Where(x => x.order_id == order.order_id);
            }
            
            var finalJson = JsonConvert.SerializeObject(orders);
            File.WriteAllText(@"C:\Users\Mateus\Documents\data_total.json", finalJson);
        }

        public void GenerateInserts()
        {
            var products = GetDataJson<Product>(PathProducts).Take(LimitData).ToList();
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