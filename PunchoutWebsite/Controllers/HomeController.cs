using System;
using Microsoft.AspNetCore.Mvc;

namespace PunchoutWebsite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string token)
        {
            var customerToken = PunchoutUserService.GetUserToken();

            if(customerToken != token)
            {
                //Prevent user from accessing the products catalog  
                //Invalid token - show access denied error page
                return StatusCode(401);
            }

            // Valid token: login user and display catalog page where user can view products
            // and add required products to their cart 

            return View();
        }
    }
}
