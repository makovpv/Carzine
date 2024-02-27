using CarzineCore;
using CarzineCore.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CarzineTest
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			var productsUsd = new List<StandardProductModel>()
			{
				new StandardProductModel()
				{
					Price = (decimal)100.23,
					Volume = (decimal)0.123,
					Weight = (decimal)0.321
				}
			};

			var productsRub = CarzineCalculator.CalcPriceComponents(productsUsd, (decimal)12.34);

			Assert.That(productsRub.Count, Is.EqualTo(1));
		}

		[Test]
		public void ZeroPriceProductTest()
		{
			var productsUsd = new List<StandardProductModel>()
			{
				new StandardProductModel()
				{
					Price = 0,
					Volume = (decimal)0.123,
					Weight = (decimal)0.321
				}
			};

			Assert.Throws<Exception>(() => CarzineCalculator.CalcPriceComponents(productsUsd, (decimal)12.34));
		}

		[Test]
		public void Test2()
		{
			var emptyProducts = new List<StandardProductModel>();

			Assert.DoesNotThrow(() => {
				CarzineCalculator.GetBestPriceProduct(emptyProducts);
			});
		}

		[Test]
		public void Test3()
		{
			var products = new List<StandardProductModel>()
			{
				new StandardProductModel()
				{
					Price = (decimal)100.231234,
					PriceRub = 9000,
					DeliveryMin = 10,
					Name = "variant 1"
				},
				new StandardProductModel()
				{
					Price = (decimal)100.231234,
					PriceRub = 9000,
					DeliveryMin = 5,
					Name = "variant 2"
				},
				new StandardProductModel()
				{
					Price = (decimal)120,
					PriceRub = 11000,
					DeliveryMin = 3,
					Name = "variant 3"
				},
			};

			var res = CarzineCalculator.GetBestPriceProduct(products);

			Assert.That(res.Name, Is.EqualTo("variant 2"));
		}
	}
}