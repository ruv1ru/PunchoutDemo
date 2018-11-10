using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using PunchoutWebsite.Models;

namespace PunchoutWebsite.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var userCart = GetCartForLoggedInUser();

            userCart.PunchOutCartDetails = GetPunchoutCartDetails(userCart);
            userCart.BrowserFormPostUrl = PunchoutUserService.GetProcurementSystemPostUrl();


            return View(userCart);
        }
        /// <summary>
        /// Gets the punchout cart details in XML format to be sent to procurement system.
        /// </summary>
        /// <returns>The punchout cart details.</returns>
        /// <param name="userCart">User cart.</param>
        string GetPunchoutCartDetails(CartModel userCart)
        {

            var xmlString = string.Empty;

            XDocument orderDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));


            XDocumentType documentType = new XDocumentType("cXML", null, "http://xml.cxml.org/schemas/cXML/1.1.009/cXML.dtd", null);
            orderDocument.Add(documentType);

            XElement orderMessage = new XElement("PunchOutOrderMessage");
            orderMessage.Add(new XElement("BuyerCookie", "vk4593733")); // Get current user's cookie received in the first setup request 
            orderMessage.Add(new XElement("PunchOutOrderMessageHeader", new XAttribute("operationAllowed", "create"),
                new XElement("Total",
                new XElement("Money", new XAttribute("currency", "AUD"), userCart.Total))));

            var cartItems = userCart.CartItems;

            int lineNumber = 1;

            foreach (var item in cartItems)
            {
                
                orderMessage.Add(new XElement("ItemIn", new XAttribute("quantity", item.Quantity),
                    new XAttribute("lineNumber", lineNumber),
                    new XElement("ItemID",
                        new XElement("SupplierPartID", item.Sku)),
                    new XElement("ItemDetail",
                        new XElement("UnitPrice",
                            new XElement("Money", new XAttribute("currency", "AUD"), item.UnitPrice)),
                        new XElement("Description", item.Description),
                        new XElement("UnitOfMeasure", "EACH"),
                                 new XElement("Classification", new XAttribute("domain", "UNSPSC"), "99999999"),
                        new XElement("ManufacturerName", ""),
                        new XElement("AttachmentReference",
                            new XElement("Name", new XAttribute(XNamespace.Xml + "lang", "en-AU"), "NA"),
                            new XElement("Description", new XAttribute(XNamespace.Xml + "lang", "en-AU"), "Product Image"),
                            new XElement("InternalID", new XAttribute("domain", "Local"), "NA"),
                            new XElement("URL", ""))),
                    new XElement("Tax",
                        new XElement("Money", new XAttribute("currency", "AUD"), ""), new XElement("Description", "GST"))
                ));

                lineNumber++;



            }

            // Add a seperate line to include shipping total
            orderMessage.Add(new XElement("ItemIn", new XAttribute("quantity", 1),
                    new XAttribute("lineNumber", lineNumber),
                    new XElement("ItemID",
                        new XElement("SupplierPartID", "FREIGHT")),
                    new XElement("ItemDetail",
                        new XElement("UnitPrice",
                            new XElement("Money", new XAttribute("currency", "AUD"),
                                userCart.ShippingTotal)),
                        new XElement("Description", "Freight"),
                        new XElement("UnitOfMeasure", "EA"),
                        new XElement("Classification", new XAttribute("domain", "UNSPSC"), "99999999"),
                        new XElement("ManufacturerName", ""),
                        new XElement("AttachmentReference",
                            new XElement("Name", new XAttribute(XNamespace.Xml + "lang", "en-AU"), "NA"),
                            new XElement("Description", new XAttribute(XNamespace.Xml + "lang", "en-AU"),
                                "Product Image"),
                            new XElement("InternalID", new XAttribute("domain", "Local"), "NA"),
                            new XElement("URL", ""))),
                    new XElement("Tax",
                        new XElement("Money", new XAttribute("currency", "AUD"), ""),
                        new XElement("Description", "GST"))

                ));

            XElement xmlHeader =
                new XElement("Header",
                new XElement("From",
                new XElement("Credential",
                new XAttribute("domain", "DUNS"), // Specifies the type of credential. This attribute allows documents to contain multiple types of credentials for multiple authentication domains. 
                                                  //DUNS for a D-U - N - S number
                new XElement("Identity", "759877702"))),

                new XElement("To",
                new XElement("Credential",
                new XAttribute("domain", "DUNS"), // NetworkId for a preassigned ID.
                new XElement("Identity", "749986717"))), // To-do get dynamic values for ID from client

                new XElement("Sender",
                new XElement("Credential",
                new XAttribute("domain", ""), //Authentication details of the buying organization including Identity, SharedSecret (password), and
                                              //AribaNetworkId, which is specified by Credential domain.The SharedSecret is the supplier's password or login to the PunchOut site
                new XElement("Identity", "techadmin@procit.com.au"), new XElement("SharedSecret", PunchoutUserService.GetUserSharedSecret())), // To-do get dynamic values for ID from client
                new XElement("UserAgent", "Procurement IT")) // A unique identifier for the application sending the PunchOutSetupRequest. Consists of the software company
                                                                   //name, product name, and version.Version details can appear in parentheses.
                );



            orderDocument.Add(new XElement("cXML",
                new XAttribute("payloadID", "1596.377374.2015-04-02T12:58:33-04:00@procit.com"),
                new XAttribute("timestamp", "2015-04-02T12:58:33-04:00"),
                new XAttribute(XNamespace.Xml + "lang", "en"),
                        xmlHeader,
                        new XElement("Message", orderMessage)));


            var stringWriter = new Utf8StringWriter();

            orderDocument.Save(stringWriter, SaveOptions.None);

            xmlString = stringWriter.ToString();

            return xmlString.Replace("\"", "'");



        }

        CartModel GetCartForLoggedInUser()
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
