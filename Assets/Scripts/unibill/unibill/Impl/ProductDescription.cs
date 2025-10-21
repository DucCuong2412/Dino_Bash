namespace Unibill.Impl
{
	public class ProductDescription
	{
		public string PlatformSpecificID { get; private set; }

		public string Price { get; private set; }

		public string Title { get; private set; }

		public string Description { get; private set; }

		public string ISOCurrencyCode { get; private set; }

		public decimal PriceDecimal { get; private set; }

		public string Receipt { get; private set; }

		public string TransactionID { get; set; }

		public bool Consumable { get; set; }

		public ProductDescription(string id, string price, string title, string description, string isoCode, decimal priceD, string receipt = null, string transactionId = null)
		{
			PlatformSpecificID = id;
			Price = price;
			Title = title;
			Description = description;
			ISOCurrencyCode = isoCode;
			PriceDecimal = priceD;
			Receipt = receipt;
			TransactionID = transactionId;
		}
	}
}
