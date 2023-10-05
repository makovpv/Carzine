export class ProductModel {
	public name: string = "";
	public source: string = "";
	manufacturer: string | undefined;
	partNumber: string | undefined;
	priceRub: number = 0;
	weight: number | undefined;
	volume: number | undefined;
	deliveryMin: number | undefined;
	deliveryMax: number | undefined;
	minOrderAmount: number | undefined;
	maxOrderAmount: number | undefined;
}
