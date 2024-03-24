using CarzineCore.Models;
using Newtonsoft.Json;

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
				PartNumberType = ItemCodeType.PN,
				Manufacturer = product.make,
				Price = Math.Round(product.price),
				MaxOrderAmount = product.available,
				MinOrderAmount = product.minLot,
				Weight = product.weight,
				DeliveryMinOriginal = product.delivery.FirstInt(),
				DeliveryMaxOriginal = product.delivery.SecondInt(),
				SourceId = ApiSource.Apm
			});
		}

		public static IEnumerable<StandardProductModel> ToStandard(this EmexServiceReference.FindByNumber[] products)
		{
			return products.Select(product => new StandardProductModel()
			{
				DeliveryMaxOriginal = Convert.ToInt32(product.GuaranteedDay),
				DeliveryMinOriginal = Convert.ToInt32(product.Delivery),
				Volume = Convert.ToDecimal(product.VolumeAdd),
				Weight = Convert.ToDecimal(product.WeightGr / 1000.0),
				Price = Math.Round(Convert.ToDecimal(product.Price)),
				MaxOrderAmount = Convert.ToInt32(product.Available),
				Manufacturer = product.MakeName,
				Name = product.PartNameRus,
				PartNumber = product.DetailNum,
				PartNumberType = ItemCodeType.PN,
				SourceId = ApiSource.Emex
			});
		}

		public static IEnumerable<StandardProductModel> ToStandard(this ApecProduct[] products, string originalBrandName)
		{
			return products.Select(product => new StandardProductModel()
			{
				DeliveryMaxOriginal = product.DeliveryDays,
				DeliveryMinOriginal = product.DeliveryDays,
				Volume = product.WeightVolume,
				Weight = product.WeightPhysical,
				Price = Math.Round(product.Price),
				MaxOrderAmount = product.QtyInStock,
				Manufacturer = product.Brand,
				Name = product.PartDescription,
				PartNumber = product.PartNumber,
				PartNumberType = ItemCodeType.PN,
				SourceId = ApiSource.Apec,
				IsOriginal = originalBrandName == product.Brand
			});
		}

		//public static PreOrderModel ToPreOrderModel(this PreOrderDto preOrder)
		//{
		//	return new PreOrderModel
		//	{
		//		Id = preOrder.Id,
		//		Date = preOrder.Date,
		//		Phone = preOrder.Phone,
		//		UserEmail = preOrder.User_email,
		//		ClientStatus = preOrder.Client_status,
		//		SupplyerStatus = preOrder.Supplyer_status,
		//		Product = new StandardProductModel
		//		{
		//			SourceId = (ApiSource)preOrder.Source_Id,
		//			PartNumber = preOrder.PN,
		//			DeliveryCost = preOrder.Delivery_cost,
		//			ExtraCharge = preOrder.Extra_charge,
		//			Manufacturer = preOrder.Manufacturer,
		//			PriceRub = preOrder.Price_Rub,
		//			Price = preOrder.Supplyer_price,
		//			Volume = preOrder.Volume,
		//			Weight = preOrder.Weight,
		//			DeliveryMin = preOrder.Delivery_Min ?? 0
		//		},
		//		PaymentOrderState = preOrder.Payment_Order_State
		//	};
		//}

		//public static PreOrderModel ToUserPreOrderModel(this PreOrderDto preOrder)
		//{
		//	return new PreOrderModel
		//	{
		//		Id = preOrder.Id,
		//		Date = preOrder.Date,
		//		ClientStatus = preOrder.Client_status,
		//		Product = new StandardProductModel
		//		{
		//			PartNumber = preOrder.PN,
		//			Manufacturer = preOrder.Manufacturer,
		//			PriceRub = preOrder.Price_Rub,
		//			Volume = preOrder.Volume,
		//			Weight = preOrder.Weight,
		//			DeliveryMin = preOrder.Delivery_Min ?? 0
		//		},
		//		PaymentOrderState = preOrder.Payment_Order_State
		//	};
		//}

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

		public static int GetDeterministicHashCode(this StandardProductModel product)
		{
			return JsonConvert.SerializeObject(product).GetDeterministicHashCode();
		}

		public static int GetDeterministicHashCode(this string str)
		{
			unchecked
			{
				int hash1 = (5381 << 16) + 5381;
				int hash2 = hash1;

				for (int i = 0; i < str.Length; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ str[i];
					if (i == str.Length - 1)
						break;
					hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
				}

				return hash1 + (hash2 * 1566083941);
			}
		}
	}

	public class CarzineException : Exception
	{
		public CarzineException()
		{
		}

		public CarzineException(string message)
			: base(message)
		{
		}

		public CarzineException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
