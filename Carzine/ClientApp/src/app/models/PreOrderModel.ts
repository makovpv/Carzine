import {ProductModel} from './ProductModel'

export class PreOrderModel {
    id: number | undefined;
    phone: string | undefined;
    userEmail: string | undefined;
    date: Date | undefined;
    product: ProductModel = new ProductModel();
}