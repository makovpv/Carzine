using CarzineCore.Models;

namespace CarzineCore
{
	public class CarzineCalculator
	{
		public List<ProductModel> MakePrice(List<ApmProduct> apmProducts)
		{
			var result = new List<ProductModel>();

			foreach (var apmProduct in apmProducts)
			{
				result.Add(new ProductModel()
				{
					Name = apmProduct.name,
					Price = apmProduct.price + 100,
					Make = apmProduct.make,
					PriceName = apmProduct.priceName,
				});
			}
			
			return result;
		}
	}
}