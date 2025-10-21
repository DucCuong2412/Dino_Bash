using Unibill.Impl;

namespace unibill.Dummy
{
	public interface IWindowsIAPCallback
	{
		void OnProductListReceived(ProductDescription[] products);

		void OnProductListError(string message);

		void OnPurchaseSucceeded(string productId, string receipt, string transactionId);

		void OnPurchaseFailed(string productId, string error);

		void logError(string error);

		void log(string message);
	}
}
