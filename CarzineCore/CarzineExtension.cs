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
				SourceId = ApiSource.Apm
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
				SourceId = ApiSource.Emex
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
				SourceId = ApiSource.Apec,
				IsOriginal = originalBrandName == product.Brand
			});
		}

		public static PreOrderModel ToPreOrderModel(this PreOrderDto preOrder)
		{
			return new PreOrderModel
			{
				Id = preOrder.Id,
				Date = preOrder.Date,
				Phone = preOrder.Phone,
				UserEmail = preOrder.User_email,
				ClientStatus = preOrder.Client_status,
				SupplyerStatus = preOrder.Supplyer_status,
				Product = new StandardProductModel
				{
					SourceId = (ApiSource)preOrder.Source_Id,
					PartNumber = preOrder.PN,
					DeliveryCost = preOrder.Delivery_cost,
					ExtraCharge = preOrder.Extra_charge,
					Manufacturer = preOrder.Manufacturer,
					PriceRub = preOrder.Price_Rub,
					Price = preOrder.Supplyer_price,
					Volume = preOrder.Volume,
					Weight = preOrder.Weight,
				}
			};
		}

		public static PreOrderModel ToUserPreOrderModel(this PreOrderDto preOrder)
		{
			return new PreOrderModel
			{
				Id = preOrder.Id,
				Date = preOrder.Date,
				ClientStatus = preOrder.Client_status,
				Product = new StandardProductModel
				{
					PartNumber = preOrder.PN,
					Manufacturer = preOrder.Manufacturer,
					PriceRub = preOrder.Price_Rub,
					Volume = preOrder.Volume,
					Weight = preOrder.Weight,
				}
			};
		}

		public static IEnumerable<StandardProductModel> FillEmptyNames(this List<StandardProductModel> products)
		{
			var defaultName = products.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name))?.Name;

			if (string.IsNullOrEmpty(defaultName))
				return products;

			foreach (var product in products.Where(x => string.IsNullOrEmpty(x.Name)))
			{
				product.Name = defaultName;
			}

			return products;
		}
	}
}
