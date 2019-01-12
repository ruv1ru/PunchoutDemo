using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PunchoutWebsite.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PunchoutWebsite.Controllers
{
    public class OciCartController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var userCart = GetCartForLoggedInUser();

            userCart.PunchOutOciOrderItems = GetPunchoutOciCartDetails(userCart);
            userCart.BrowserFormPostUrl = PunchoutUserService.GetProcurementSystemPostUrl();


            return View(userCart);
        }

        /// <summary>
        /// Gets the punchout cart details in Open Calatog Interface format to be sent to procurement system.
        /// </summary>
        /// <returns>The punchout cart details as a collection of OciItems.</returns>
        /// <param name="userCart">User cart.</param>
        List<OciItemModel> GetPunchoutOciCartDetails(CartModel userCart)
        {

            var items = new List<OciItemModel>();

            var cartItems = userCart.CartItems;

            int itemIndex = 1;

            foreach (var item in cartItems)
            {

                items.Add(new OciItemModel
                {
                    ItemIndex = itemIndex,
                    ProductCode = item.Sku,
                    Description = item.Description,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity
                });

                itemIndex++;

            }

            // Add a seperate line to include shipping total
            items.Add(new OciItemModel
            {
                ItemIndex = itemIndex,
                Price = userCart.ShippingTotal,
                ProductCode = "FREIGHT",
                Description = "Freight"
            });

            return items;



        }
    }
}
