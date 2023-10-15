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
				var deliveryCost = 
					  Math.Max(product.Weight, product.Volume) * 12
					+ product.Price * (decimal)0.04;
				
				var totalPriceUSD = 
					  product.Price
					+ deliveryCost
					+ GetExtraCharge(product.Price)
					+ product.Price * (decimal)0.03;

				product.PriceRub = totalPriceUSD * usdRate;
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

			return products.MinBy(x => (Array.IndexOf(prices, x.PriceRub) + Array.IndexOf(delivery, x.DeliveryMin)) / 2.0);
		}

		private decimal GetExtraCharge(decimal price)
		{
			if (price <= 0)
				throw new Exception("Price cannot be equal or less than 0");
			
			if (price <= 10)
				return price * (5 - 1);
			else if (price <= 50)
				return price * (4 - 1);
			else if (price <= 120)
				return price * (3 - 1);
			else if (price <= 1000)
				return price * (2 - 1);
			else
				return price * (decimal)(1.7 - 1.0);
		}
	}
}