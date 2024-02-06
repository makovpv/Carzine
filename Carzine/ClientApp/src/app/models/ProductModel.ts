export type DeliveryInfo = {
	deliveryMin: number | undefined;
	deliveryMax: number | undefined;
}

export class ProductModel implements DeliveryInfo {
	name: string = "";
	sourceId: number | undefined;
	manufacturer: string | undefined;
	partNumber: string | undefined;
	priceRub: number = 0;
	price: number = 0;
	weight: number | undefined;
	volume: number | undefined;
	deliveryMin: number | undefined;
	deliveryMax: number | undefined;
	minOrderAmount: number | undefined;
	maxOrderAmount: number | undefined;
	isOriginal: boolean | undefined;
    extraCharge: number | undefined;
	deliveryCost: number | undefined;
}
