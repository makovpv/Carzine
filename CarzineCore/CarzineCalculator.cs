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