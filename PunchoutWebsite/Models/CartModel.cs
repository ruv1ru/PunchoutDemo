using System;
using System.Collections.Generic;

namespace PunchoutWebsite.Models
{
    public class CartModel
    {
        public decimal Total;
        public decimal ShippingTotal => 0;

		public List<CartItemModel> CartItems { get; set; }
        public string PunchOutCartDetails { get; set; }
        public string BrowserFormPostUrl { get; set; }
		public List<OciItemModel> PunchOutOciOrderItems { get; set; }
	}
}
