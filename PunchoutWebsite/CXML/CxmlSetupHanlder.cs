using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace PunchoutWebsite.CXML
{
    public class CxmlSetupHanlder
    {
        

        RequestDelegate _next;
        string responseStatus = "Fail";
        string responseCode = "ERROR";
        string resposeText = "400";
        string startPageUrl = "";


        public CxmlSetupHanlder(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                string xml = reader.ReadToEnd();

                XDocument document = XDocument.Load(new StringReader(xml));

                XElement cXml = document.Element("cXML");

                if (GetSenderSharedSecretFromRequest(cXml) == GetSenderSharedSecretFromDatabase())
                {
                    responseStatus = "Success";
                    resposeText = "OK";
                    responseCode = "200";

                    startPageUrl = context.Request.Scheme + "://" + context.Request.Host.Value + "/start.cxml?token=" + GetCustomerToken();
                }


            }


            XDocument responseDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            XElement statusElement = new XElement("Status", responseStatus);
            statusElement.Add(new XAttribute("code", responseCode));
            statusElement.Add(new XAttribute("text", resposeText));

            XElement response =
                    new XElement("cXML",
                    new XElement("Response",
                        statusElement,
                            new XElement("PunchOutSetupResponse",
                                new XElement("StartPage",
                                new XElement("URL", startPageUrl)))));


            responseDocument.Add(response);
            context.Response.ContentType = "text/xml";


            await context.Response.WriteAsync
                         (responseDocument.ToString());

        }

        /// <summary>
        /// Gets the unique customer token of specific user who is punching in to the system.
        /// This is a unique value that is generated on each succesfull setup request
        /// </summary>
        /// <returns>The customer token.</returns>
        string GetCustomerToken()
        {
            //return Guid.NewGuid();
            return "7d92c2ed-32ae-4bd4-9453-1a337bd7cb33";
        }

        string GetSenderSharedSecretFromRequest(XElement cXml)
        {
            //Sender element of the request body allows the receiving party to identify and authenticate the party that opened the HTTP connection. 
            //It contains a stronger authentication Credential than the ones in the From or To elements, because the receiving
            //party must authenticate who is asking it to perform work

            //The SharedSecret element is used when the Sender has a password that the requester recognizes.
            return cXml.Element("Header").Element("Sender").Element("Credential").Element("SharedSecret").Value;
        }

        string GetSenderSharedSecretFromDatabase()
        {
            //SharedSecret is can be securely stored in database or using any other repository
            return "abracadabra";
        }
    }
}
