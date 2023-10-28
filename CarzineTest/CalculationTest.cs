using CarzineCore;
using CarzineCore.Models;
using NUnit.Framework;
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

			var productsRub = CarzineCalculator.CalcPriceRub(productsUsd, (decimal)12.34);

			Assert.That(productsRub.Count, Is.EqualTo(1));
		}
	}
}