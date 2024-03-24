using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarzineTest
{
	public class Tests
	{
		private IDbDataRepository _mockDataRep;
		
		[SetUp]
		public void Setup()
		{
			_mockDataRep = new MockDataRepository();
		}

		[Test]
		public async Task CalculatePriceCountTest()
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

			var productsRub = await new CarzineCalculator(_mockDataRep).CalcProductComponentsAsync(productsUsd, (decimal)12.34);

			Assert.That(productsRub.Count, Is.EqualTo(1));
		}

		[Test]
		[TestCase(1000, 10, 5, 2000)]
		[TestCase(5000, 10, 5, 8500)]
		[TestCase(3, 10, 5, 15)]
		public async Task CalculateExtraChargePartTest(decimal price, decimal volume, decimal weight, decimal extraCharge)
		{
			var productsUsd = new List<StandardProductModel>()
			{
				new StandardProductModel()
				{
					Price = price,
					Volume = volume,
					Weight = weight
				}
			};

			var productsRub = await new CarzineCalculator(_mockDataRep).CalcProductComponentsAsync(productsUsd, 100);

			Assert.That(productsRub.First().ExtraCharge, Is.EqualTo(extraCharge));
		}

		[Test]
		[TestCase(107, 107, 107)]
		[TestCase(4, 15, 14)]
		public async Task CalculateDeliveryPartsTest(int deliveryMin, int deliveryMax, int result)
		{
			var products = new List<StandardProductModel>()
			{
				new StandardProductModel()
				{
					Price = 1,
					DeliveryMinOriginal = deliveryMin,
					DeliveryMaxOriginal = deliveryMax
				}
			};

			products = await new CarzineCalculator(_mockDataRep).CalcProductComponentsAsync(products, 1);

			Assert.That(products.First().DeliveryMin, Is.EqualTo(result));
		}

		[Test]
		public async Task ZeroPriceProductTest()
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

			Assert.ThrowsAsync<Exception>(async () => 
				await new CarzineCalculator(_mockDataRep)
				.CalcProductComponentsAsync(productsUsd, (decimal)12.34)
			);
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

	public class MockDataRepository : IDbDataRepository
	{
		public Task<int> AddPreOrderAsync(StandardProductModel product, string userName)
		{
			throw new NotImplementedException();
		}

		public Task AddRuleRangeAsync(RuleRangeDto ruleRange)
		{
			throw new NotImplementedException();
		}

		public Task DeleteRuleRangeAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<StatusDto>> GetClientStatusesAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync(RuleRangeType rangeType)
		{
			var res = new List<RuleRangeDto>
			{
				new RuleRangeDto(null, null, 10, 5, RuleRangeType.price),
				new RuleRangeDto(null, 10, 50, 4, RuleRangeType.price),
				new RuleRangeDto(null, 50, 120, 3, RuleRangeType.price),
				new RuleRangeDto(null, 120, 1000, 2, RuleRangeType.price),
				new RuleRangeDto(null, 1000, null, (decimal)1.7, RuleRangeType.price),

				new RuleRangeDto(null, 0, 7, 14, RuleRangeType.delivery),
			};


			return await Task.FromResult(res.Where(x => x.Type == rangeType));
		}

		public Task SetPaymentOrderIdAsync(int orderId, string paymentOrderId, string paymentStatus)
		{
			throw new NotImplementedException();
		}

		public Task SetOrderClientStatus(int orderId, ClientStatus status)
		{
			throw new NotImplementedException();
		}

		Task<IEnumerable<OrderDto>> IDbDataRepository.GetOrdersAsync()
		{
			throw new NotImplementedException();
		}

		Task<OrderDto> IDbDataRepository.GetOrderAsync(int preOrderId)
		{
			throw new NotImplementedException();
		}

		Task<IEnumerable<OrderDto>> IDbDataRepository.GetOrdersByUserAsync(string userEmail)
		{
			throw new NotImplementedException();
		}
	}
}