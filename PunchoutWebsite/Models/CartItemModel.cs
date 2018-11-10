using System;
namespace PunchoutWebsite.Models
{
    public class CartItemModel
    {
        

        public int Id { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }


    }
}
