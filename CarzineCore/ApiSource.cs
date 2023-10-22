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
}
