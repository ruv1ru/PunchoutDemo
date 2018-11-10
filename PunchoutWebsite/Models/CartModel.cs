using System;
using System.Collections.Generic;

namespace PunchoutWebsite.Models
{
    public class CartModel
    {
        public decimal Total;
		internal decimal ShippingTotal;

		public List<CartItemModel> CartItems { get; set; }
        public string PunchOutCartDetails { get; set; }
        public string BrowserFormPostUrl { get; set; }
    }
}
