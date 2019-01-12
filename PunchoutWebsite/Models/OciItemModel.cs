using System;
namespace PunchoutWebsite.Models
{
    public class OciItemModel
    {
        public int ItemIndex { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string DescriptionInputName => "NEW_ITEM-DESCRIPTION[" + ItemIndex + "]";
        public string ProductCodeInputName => "NEW_ITEM-EXT_PRODUCT_ID[" + ItemIndex + "]";
        public string PriceInputName => "NEW_ITEM-PRICE[" + ItemIndex + "]";
        public string QuantityInputName => "NEW_ITEM-QUANTITY[" + ItemIndex + "]";
    }
}
