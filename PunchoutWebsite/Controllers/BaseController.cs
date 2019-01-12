using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PunchoutWebsite.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PunchoutWebsite.Controllers
{
    public class BaseController : Controller
    {
        protected CartModel GetCartForLoggedInUser()
        {
            // Return cart items added selected by user in the product catalog page

            return new CartModel
            {
                CartItems = new List<CartItemModel>
                {
                    new CartItemModel { Id = 1, Quantity = 2, Sku = "SKU88765", UnitPrice = 10.25m, Description = "sample product 1" },
                    new CartItemModel { Id = 2, Quantity = 5, Sku = "SKU88900", UnitPrice = 25.55m, Description = "sample product 2" }
                }
            };

        }
    }
}
