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

        /*
         /// <summary>
        /// Convert order details to Open Catalog Interface standards 
        /// </summary>
        private IEnumerable<OCInterfaceItemModel> OrderDetailsToOCInterfaceItems()
        {

            _priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();

            var items = new List<OCInterfaceItemModel>();
            
            var customer = _workContext.CurrentCustomer;

            var cart = customer.ShoppingCartItems.ToList();



            //decimal? orderTotal = null;
            //decimal orderDiscountAmount = decimal.Zero;
            //List<AppliedGiftCard> appliedGiftCards = null;
            //int redeemedRewardPoints = 0;
            //decimal redeemedRewardPointsAmount = decimal.Zero;

            //Discount orderAppliedDiscount = null;
            //orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart,
            //    out orderDiscountAmount, out orderAppliedDiscount, out appliedGiftCards,
            //    out redeemedRewardPoints, out redeemedRewardPointsAmount);
            //if (!orderTotal.HasValue)
            //    throw new NopException("Order total couldn't be calculated");

            //Create order details
            var order = new Order()
            {
                OrderGuid = Guid.NewGuid(),
                CustomerId = customer.Id,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                OrderStatus = OrderStatus.Quote,
                CreatedOnUtc = DateTime.UtcNow,
                //ShippingAddress = billingAddress,
                //BillingAddress = billingAddress,
                //OrderTotal = orderTotal.Value
            };
            
            //_orderService.InsertOrder(order);
            
            
            var cartItems = customer.ShoppingCartItems;

            var itemIndex = 1;

            foreach (var item in cartItems)
            {
                var uomId = item.ProductVariant.UnitOfMeasureId ?? 0;

                var uomName = _productService.GetUOMAbbreviationById(uomId);

                var ocItem = new OCInterfaceItemModel();
                

                //ocItem.HtmlInputName = "EW_ITEM-DESCRIPTION[" + itemIndex + "]";

                //NEW_ITEM-EXT_PRODUCT_ID ORD_PRODUCT_CODE Product Code
                ocItem.ProductCode = item.ProductVariant.Sku;
                ocItem.ProductCodeHtmlInputName = "NEW_ITEM-EXT_PRODUCT_ID[" + itemIndex + "]";
                //NEW_ITEM-DESCRIPTION ORD_DESC Item Description
                ocItem.Description = item.ProductVariant.Name;
                ocItem.DescriptionHtmlInputName = "NEW_ITEM-DESCRIPTION[" + itemIndex + "]";
                //NEW_ITEM-MANUFACTCODE PRO_MANUFACTURER_CODE Item Code - Manufacturer or Product Code
                ocItem.ItemCode = item.ProductVariant.ManufacturerPartNumber;
                ocItem.ItemCodeHtmlInputName = "NEW_ITEM-MANUFACTCODE[" + itemIndex + "]";
                //NEW_ITEM-PRICE ORD_APRICE Price per item eg. 10.55
                ocItem.Price = item.ProductVariant.Price;
                ocItem.PriceHtmlInputName = "NEW_ITEM-PRICE[" + itemIndex + "]";
                ocItem.PriceHtmlInputId = "NEW_ITEM-PRICE_" + itemIndex;
                //NEW_ITEM-PRICEUNIT ORD_PRICEUNIT Price is per x units - Typically 1
                ocItem.PricePerXUnits = 1;
                ocItem.PricePerXUnitsHtmlInputName = "NEW_ITEM-PRICEUNIT[" + itemIndex + "]";
                //NEW_ITEM-QUANTITY ORD_QUANTITY Order Quantity
                ocItem.Quantity = item.Quantity;
                ocItem.QuantityHtmlInputName = "NEW_ITEM-QUANTITY[" + itemIndex + "]";
                //NEW_ITEM-UNIT ORD_QUANTITY_UNIT Unit of measure - EA, Carton etc.
                ocItem.UnitOfMeasure = uomName;
                ocItem.UnitOfMeasureHtmlInputName = "NEW_ITEM-UNIT[" + itemIndex + "]";
                //NEW_ITEM-CURRENCY Item currency 
                ocItem.Currency = "AUD";
                ocItem.CurrencyHtmlInputName = "NEW_ITEM-CURRENCY[" + itemIndex + "]";
                //NEW_ITEM-VENDORMAT Vendor product number for the item
                ocItem.VendorProductNumber = item.ProductVariant.Sku;
                ocItem.VendorProductNumberHtmlInputName = "NEW_ITEM-VENDORMAT[" + itemIndex + "]";
                //NEW_ITEM-EXT_SCHEMA_TYPE Name of a schema via which it was imported in the SRM Server
                ocItem.SchemaType = "UNSPSC";
                ocItem.SchemaTypeHtmlInputName = "NEW_ITEM-EXT_SCHEMA_TYPE[" + itemIndex + "]";
                //NEW_ITEM-EXT_CATEGORY_ID Unique key for an external category from the schema above, independent of the version of the schema
                ocItem.CategoryId = "52000000";
                ocItem.CategoryIdHtmlInputName = "NEW_ITEM-EXT_CATEGORY_ID[" + itemIndex + "]";
                //NEW_ITEM-CUST_FIELD1 ORD_VAT_PER GST Percent
                ocItem.GSTPercent = item.ProductVariant.IsTaxExempt == false && item.ProductVariant.TaxCategoryId == 6 ? 10 : 0; ;
                ocItem.GSTPercentHtmlInputName = "NEW_ITEM-CUST_FIELD1[" + itemIndex + "]";
                //NEW_ITEM-CUST_FIELD2 ORD_VAT_AMOUNT GST Amount (unit price x gst % x qty)
                ocItem.GSTAmount = item.ProductVariant.IsTaxExempt == false && item.ProductVariant.TaxCategoryId == 6 ? (item.ProductVariant.Price / 100) * 10 : 0;
                ocItem.GSTAmountHtmlInputName = "NEW_ITEM-CUST_FIELD2[" + itemIndex + "]";
                ocItem.GSTAmountHtmlInputId = "NEW_ITEM-CUST_FIELD2_" + itemIndex;

                items.Add(ocItem);
                
                var taxRateTemp = decimal.Zero;
                var scUnitPrice = _priceCalculationService.GetUnitPrice(item, true);
                var scSubTotal = _priceCalculationService.GetSubTotal(item, true);
                
                var scUnitPriceInclTax = _taxService.GetProductPrice(item.ProductVariant, scUnitPrice, true, customer, out taxRateTemp);
                var scUnitPriceExclTax = _taxService.GetProductPrice(item.ProductVariant, scUnitPrice, false, customer, out taxRateTemp);
                var scSubTotalInclTax = _taxService.GetProductPrice(item.ProductVariant, scSubTotal, true, customer, out taxRateTemp);
                var scSubTotalExclTax = _taxService.GetProductPrice(item.ProductVariant, scSubTotal, false, customer, out taxRateTemp);


                var itemWeight = _shippingService.GetShoppingCartItemWeight(item);

                //save order item
                var opv = new OrderProductVariant()
                {
                    OrderProductVariantGuid = Guid.NewGuid(),
                    Order = order,
                    ProductVariantId = item.ProductVariantId,
                    UnitPriceInclTax = scUnitPriceInclTax,
                    UnitPriceExclTax = scUnitPriceExclTax,
                    PriceInclTax = scSubTotalInclTax,
                    PriceExclTax = scSubTotalExclTax,
                    Quantity = item.Quantity,
                    ItemWeight = itemWeight
                };

                order.OrderProductVariants.Add(opv);
                
                itemIndex++;
            }

            //shipping total


            decimal taxRate;
            Discount shippingTotalDiscount = null;
            var orderShippingTotalInclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true, out taxRate, out shippingTotalDiscount);
            var orderShippingTotalExclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, false);
            if (orderShippingTotalInclTax.HasValue && orderShippingTotalExclTax.HasValue)
            {
                var ocItem = new OCInterfaceItemModel();

                //NEW_ITEM-EXT_PRODUCT_ID ORD_PRODUCT_CODE Product Code
                ocItem.ProductCode = "Freight";
                ocItem.ProductCodeHtmlInputName = "NEW_ITEM-EXT_PRODUCT_ID[" + itemIndex + "]";
                //NEW_ITEM-DESCRIPTION ORD_DESC Item Description
                ocItem.Description = "Freight Charges";
                ocItem.DescriptionHtmlInputName = "NEW_ITEM-DESCRIPTION[" + itemIndex + "]";
                //NEW_ITEM-MANUFACTCODE PRO_MANUFACTURER_CODE Item Code - Manufacturer or Product Code
                ocItem.ItemCode = "Freight Line";
                ocItem.ItemCodeHtmlInputName = "NEW_ITEM-MANUFACTCODE[" + itemIndex + "]";
                //NEW_ITEM-PRICE ORD_APRICE Price per item eg. 10.55
                ocItem.Price = orderShippingTotalExclTax.Value;
                ocItem.PriceHtmlInputName = "NEW_ITEM-PRICE[" + itemIndex + "]";
                ocItem.PriceHtmlInputId = "NEW_ITEM-PRICE_FREIGHT";
                //NEW_ITEM-PRICEUNIT ORD_PRICEUNIT Price is per x units - Typically 1
                ocItem.PricePerXUnits = 1;
                ocItem.PricePerXUnitsHtmlInputName = "NEW_ITEM-PRICEUNIT[" + itemIndex + "]";
                //NEW_ITEM-QUANTITY ORD_QUANTITY Order Quantity
                ocItem.Quantity = 1;
                ocItem.QuantityHtmlInputName = "NEW_ITEM-QUANTITY[" + itemIndex + "]";
                //NEW_ITEM-UNIT ORD_QUANTITY_UNIT Unit of measure - EA, Carton etc.
                ocItem.UnitOfMeasure = "FRT";
                ocItem.UnitOfMeasureHtmlInputName = "NEW_ITEM-UNIT[" + itemIndex + "]";
                //NEW_ITEM-CURRENCY Item currency 
                ocItem.Currency = "AUD";
                ocItem.CurrencyHtmlInputName = "NEW_ITEM-CURRENCY[" + itemIndex + "]";
                //NEW_ITEM-VENDORMAT Vendor product number for the item
                ocItem.VendorProductNumber = "Freight";
                ocItem.VendorProductNumberHtmlInputName = "NEW_ITEM-VENDORMAT[" + itemIndex + "]";
                //NEW_ITEM-EXT_SCHEMA_TYPE Name of a schema via which it was imported in the SRM Server
                ocItem.SchemaType = "";
                ocItem.SchemaTypeHtmlInputName = "NEW_ITEM-EXT_SCHEMA_TYPE[" + itemIndex + "]";
                //NEW_ITEM-EXT_CATEGORY_ID Unique key for an external category from the schema above, independent of the version of the schema
                ocItem.CategoryId = "";
                ocItem.CategoryIdHtmlInputName = "NEW_ITEM-EXT_CATEGORY_ID[" + itemIndex + "]";
                //NEW_ITEM-CUST_FIELD1 ORD_VAT_PER GST Percent
                ocItem.GSTPercent = taxRate;
                ocItem.GSTPercentHtmlInputName = "NEW_ITEM-CUST_FIELD1[" + itemIndex + "]";

                //NEW_ITEM-CUST_FIELD2 ORD_VAT_AMOUNT GST Amount (unit price x gst % x qty)
                ocItem.GSTAmount = orderShippingTotalInclTax.Value - orderShippingTotalExclTax.Value;
                ocItem.GSTAmountHtmlInputName = "NEW_ITEM-CUST_FIELD2[" + itemIndex + "]";
                ocItem.GSTAmountHtmlInputId = "NEW_ITEM-CUST_FIELD2_FREIGHT";

                items.Add(ocItem);

                order.TaxRates = taxRate.ToString();
                order.OrderShippingInclTax = orderShippingTotalInclTax.Value;
                order.OrderShippingExclTax = orderShippingTotalExclTax.Value;
                //_orderService.UpdateOrder(order);
            }

            _httpContext.Session["PunchoutOrderInfo"] = order;

            return items;
        }

         */
    }
}
