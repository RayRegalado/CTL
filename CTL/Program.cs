using System;
using System.Collections.Generic;
using System.Configuration;
using Ctl.Data;
using RestSharp;
using System.Linq;

namespace CTL
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var rawOrders = ReadOrderFile();
            var orders = ConsolidateOrders(rawOrders);
            UploadOrders(orders);
            Console.ReadLine();
        }

        private static IEnumerable<RawOrder> ReadOrderFile()
        {
            return Formats.Csv.ReadObjects<RawOrder>("OrderData.csv");
        }

        private static IEnumerable<Order> ConsolidateOrders(IEnumerable<RawOrder> rawOrders)
        {
            return from rawOrder in rawOrders
                   group rawOrder by rawOrder.OrderId into g
                   select new Order
                   {
                       Id = g.Key,
                       Items = g.Select(ro => new OrderItem()
                       {
                           Quantity = ro.Quantity,
                           Sku = ro.SKU
                       }),
                       ShipTo = new ShipTo()
                       {
                           Address1 = g.First().Addr1,
                           City = g.First().City,
                           Name = $"{g.First().FirstName} {g.First().LastName}",
                           PostalCode = g.First().Postal,
                           StateOrProvince = g.First().State
                       }
                   };
        }

        private static void UploadOrders(IEnumerable<Order> orders)
        {
            var client = new RestClient(ConfigurationManager.AppSettings.Get("APIBaseURI"));

            client.AddDefaultHeader("X-Ctl-UserId", ConfigurationManager.AppSettings.Get("APIUserID"));
            client.AddDefaultHeader("X-Ctl-ClientId", ConfigurationManager.AppSettings.Get("APIClientID"));

            orders.ToList().ForEach(order =>
            {
                var request = new RestRequest($"/v1/Orders/{order.Id}", Method.PUT)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddBody(new
                {
                    order.ShipTo,
                    order.Items
                });

                var response = client.Execute(request);
                Console.WriteLine(response);
            });
        }
    }
}
