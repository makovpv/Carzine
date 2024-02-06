import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { PreOrderModel } from '../models/PreOrderModel';
import { ProductModel } from '../models/ProductModel';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  suppliers = [
    { id: 1, name: 'APM' }, 
    { id: 2, name: 'Emex' }, 
    { id: 3, name: 'Apec' }
  ];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  preOrder(data: ProductModel): Promise<any> {
    return this.http.post(this.baseUrl+'order/preorder', data).toPromise();
  }

  getPreOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'order/preorder').toPromise();
  }

  getUserPreOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'order/user-preorders').toPromise();
  }

  createOrder(preorderId: number): Promise<any> {
    return this.http.post(`${this.baseUrl}order/order/${preorderId}`, {}).toPromise();
  }

  getSuppliers(): Promise<any> {
    return this.http.get(this.baseUrl+'order/suppliers').toPromise();
  }
}
