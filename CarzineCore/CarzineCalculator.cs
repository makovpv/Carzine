using CarzineCore.Interfaces;
using CarzineCore.Models;

namespace CarzineCore
{
	public class CarzineCalculator
	{
		private readonly IDbDataRepository _dataRepository;

		public CarzineCalculator(IDbDataRepository dataRepository)
		{
			_dataRepository = dataRepository;
		}

		public async Task<List<StandardProductModel>> CalcProductComponentsAsync(IEnumerable<StandardProductModel> products, decimal usdRate)
		{
			List<StandardProductModel>? result = products.ToList();

			foreach (var product in result)
			{
				product.DeliveryCost =
					  Math.Max(product.Weight, product.Volume) * 12
					+ product.Price * (decimal)0.04;

				product.ExtraCharge = await GetExtraChargeAsync(product.Price);

				var totalPriceUSD = 
					  product.Price
					+ product.DeliveryCost
					+ product.ExtraCharge
					+ product.Price * (decimal)0.03;

				product.PriceRub = totalPriceUSD * usdRate;

				product.DeliveryMin = await GetTransfornedDeliveryDurationAsync(product.DeliveryMinOriginal);
				product.DeliveryMax = Math.Max(
					await GetTransfornedDeliveryDurationAsync(product.DeliveryMaxOriginal),
					product.DeliveryMin);
			}
			
			return result;
		}

		public static StandardProductModel GetOptimalProduct(List<StandardProductModel> products)
		{
			// need for tests!!

			var prices = products.Select(x => x.PriceRub).Distinct().ToArray();
			Array.Sort(prices);

			var delivery = products.Select(x => x.DeliveryMin).Distinct().ToArray();
			Array.Sort(delivery);

			return products.MinBy(x => (Array.IndexOf(prices, x.PriceRub) + Array.IndexOf(delivery, x.DeliveryMin)) / 2.0);
		}

		public static StandardProductModel? GetBestPriceProduct(List<StandardProductModel> products)
		{
			if (!products.Any())
				return null;
			
			var minPrice = Math.Round(products.Min(x => x.PriceRub));

			return products.Where(x => Math.Round(x.PriceRub) == minPrice).MinBy(x => x.DeliveryMin);
		}

		private async Task<decimal> GetExtraChargeAsync(decimal price)
		{
			if (price <= 0)
				throw new Exception("Price cannot be equal or less than 0");

			var ranges = await _dataRepository.GetRuleRangesAsync(RuleRangeType.price);

			var extraFactor = ranges.FirstOrDefault(x => 
					(x.Min ?? int.MinValue) < price &&
					(x.Max ?? int.MaxValue) >= price
				)?.Value ?? 0;

			return price * extraFactor;


			//if (price <= 10)
			//	return price * (5 - 1);
			//else if (price <= 50)
			//	return price * (4 - 1);
			//else if (price <= 120)
			//	return price * (3 - 1);
			//else if (price <= 1000)
			//	return price * (2 - 1);
			//else
			//	return price * (decimal)(1.7 - 1.0);
		}

		private async Task<int> GetTransfornedDeliveryDurationAsync(int duration)
		{
			var ranges = await _dataRepository.GetRuleRangesAsync(RuleRangeType.delivery);

			var result = ranges.FirstOrDefault(x =>
					(x.Min ?? int.MinValue) < duration &&
					(x.Max ?? int.MaxValue) >= duration
				)?.Value ?? duration;

			return Convert.ToInt32(result);
		}
	}
}