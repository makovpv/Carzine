import { ProductModel } from "./ProductModel";

export class ProductSearchResultModel {
	public bestPrice = new ProductModel;
	public expressDelivery = new ProductModel;
	optimal = new ProductModel;
	public products: ProductModel[] = [];
}