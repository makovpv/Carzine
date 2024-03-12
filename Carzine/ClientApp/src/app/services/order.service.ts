import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { RuleRangeModel } from '../models/RuleRangeModel';

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
    return this.http.post(this.baseUrl+'api/order/preorder', data).toPromise();
  }

  getPreOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/preorder').toPromise();
  }

  getUserPreOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/user-preorders').toPromise();
  }

  createOrder(preorderId: number): Promise<any> {
    return this.http.post(`${this.baseUrl}api/order/order/${preorderId}`, {}).toPromise();
  }

  getSuppliers(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/suppliers').toPromise();
  }

  getClientStatuses(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/status').toPromise();
  }

  setPreorderStatus(orderId: number, statusId: number): Promise<any> {
    return this.http.post(
      `${this.baseUrl}api/order/preorder/status/${orderId}`, statusId)
      .toPromise();
  }

  getRules(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/rules').toPromise();
  }

  addRule(data: RuleRangeModel): Promise<any> {
    return this.http.post(this.baseUrl+'api/order/rule', data).toPromise();
  }

  deleteRule(id: number): Promise<any> {
    return this.http.delete(`${this.baseUrl}api/order/rule/${id}`).toPromise();
  }
}
