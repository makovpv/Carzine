using CarzineCore.Models;

namespace CarzineCore
{
	public class CarzineCalculator
	{
		public CarzineCalculator()
		{
			
		}

		public async Task<List<StandardProductModel>> CalcPriceRubAsync(List<StandardProductModel> products)
		{
			var usdRate = await DataCollector.GetCbrCursAsync("USD");

			var result = products.ToList();

			foreach (var product in result)
			{
				product.PriceRub = GetChargedPrice(product.Price) * usdRate;
			}
			
			return result;
		}

		public StandardProductModel GetOptimalProduct(List<StandardProductModel> products)
		{
			// need for tests!!

			var prices = products.Select(x => x.PriceRub).Distinct().ToArray();
			Array.Sort(prices);

			var delivery = products.Select(x => x.DeliveryMin).Distinct().ToArray();
			Array.Sort(delivery);

			var rrrr = products.MinBy(x => (Array.IndexOf(prices, x.PriceRub) + Array.IndexOf(delivery, x.DeliveryMin)) / 2.0);

			return rrrr;
		}

		private decimal GetChargedPrice(decimal price)
		{
			if (price <= 0)
				throw new Exception("Price cannot be equal or less than 0");
			
			if (price <= 10)
				return price * 5;
			else if (price <= 50)
				return price * 4;
			else if (price <= 120)
				return price * 3;
			else if (price <= 1000)
				return price * 2;
			else
				return price * (decimal)1.7;
		}
	}
}