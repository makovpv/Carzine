using Carzine.Models;
using CarzineCore;
using CarzineCore.Models;

namespace Carzine
{
	public static class CarzineDataTransfom
	{
		public static CartItemViewModel ToViewModel(this StandardProductModel product)
		{
			return new CartItemViewModel
			{
				Code = product.GetDeterministicHashCode(),
				Name = product.Name,
				PriceRub = product.PriceRub,
				DeliveryMax = product.DeliveryMax,
				DeliveryMin = product.DeliveryMin,
				IsOriginal = product.IsOriginal,
				Manufacturer = product.Manufacturer,
				PartNumber = product.PartNumber
			};
		}

		public static IEnumerable<CartItemViewModel> ToViewModel(this IEnumerable<StandardProductModel> products)
		{
			return products.Select(x => x.ToViewModel());
		}
	}
}
