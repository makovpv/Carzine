using CarzineCore.Models;

namespace CarzineCore
{
	public static class CarzineExtension
	{
		public static int FirstInt(this string s)
		{
			return Convert.ToInt32(s[..s.IndexOf('/')]);
		}

		public static int SecondInt(this string s)
		{
			return Convert.ToInt32(s[(s.IndexOf('/') + 1)..]);
		}

		public static IEnumerable<StandardProductModel> ToStandard(this List<ApmProduct> products)
		{
			return products.Select(product => new StandardProductModel()
			{
				Name = product.name,
				PartNumber = product.code,
				Manufacturer = product.make,
				Price = Math.Round(product.price),
				MaxOrderAmount = product.available,
				MinOrderAmount = product.minLot,
				Weight = product.weight,
				DeliveryMin = product.delivery.FirstInt(),
				DeliveryMax = product.delivery.SecondInt(),
				Source = "Apm"
			});
		}

		public static IEnumerable<StandardProductModel> ToStandard(this EmexServiceReference.FindByNumber[] products)
		{
			return products.Select(product => new StandardProductModel()
			{
				DeliveryMax = Convert.ToInt32(product.GuaranteedDay),
				DeliveryMin = Convert.ToInt32(product.Delivery),
				Volume = Convert.ToDecimal(product.VolumeAdd),
				Weight = Convert.ToDecimal(product.WeightGr / 1000.0),
				Price = Math.Round(Convert.ToDecimal(product.Price)),
				MaxOrderAmount = Convert.ToInt32(product.Available),
				Manufacturer = product.MakeName,
				Name = product.PartNameRus,
				PartNumber = product.DetailNum,
				Source = "Emex"
			});
		}

		public static IEnumerable<StandardProductModel> ToStandard(this ApecProduct[] products, string originalBrandName)
		{
			return products.Select(product => new StandardProductModel()
			{
				DeliveryMax = product.DeliveryDays,
				DeliveryMin = product.DeliveryDays,
				Volume = product.WeightVolume,
				Weight = product.WeightPhysical,
				Price = Math.Round(product.Price),
				MaxOrderAmount = product.QtyInStock,
				Manufacturer = product.Brand,
				Name = product.PartDescription,
				PartNumber = product.PartNumber,
				Source = "Apec",
				IsOriginal = originalBrandName == product.Brand
			});
		}
	}
}
