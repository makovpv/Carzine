using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public enum ApiSource
	{
		Apm = 1,
		Emex = 2,
		Apec = 3
	}

	public enum ApmDeliveryType
	{
		SelfPickup = 1,
		//Delivery = 2 ??
		Kiev = 2,
		NewPost = 3,
		UkrainianPost = 4,
		RussianPost = 5
	}

	public enum ClientStatus
	{
		InProgress = 1,
		WaitForCall = 2,
		WaitForPayment = 3,
		InDelivery = 4,
		ReadyForGet = 5,
		Done = 6,
		Canceled = 7
	}

	public enum SupplyerStatus
	{
		New = 1,
		zzz = 2,                // закуплено
		nnn = 3,                // нет в наличии
		transitEmirates = 4,    // транзит в Эмиратах (путь от поставщика до самолета)
		transitMoscow = 5,      // транзит Москва (путь от терминалов до ТК, либо клиента)
		Delivery = 6,           // передано в ТК
		Finished = 7            // получено клиентом
	}
}
