using System.Collections.Generic;

namespace CTL
{
    public class ShipTo
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
    }

    public class OrderItem
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }        
    }

    public class Order
    {
        public int Id { get; set; }
        public ShipTo ShipTo { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
    }
}
