import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { RuleRangeModel } from '../models/RuleRangeModel';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  suppliers = [
    { id: 1, name: 'APM' }, 
    { id: 2, name: 'Emex' }, 
    { id: 3, name: 'Apec' }
  ];

  userTempUid = this.authService.getUserTempUid();

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private authService: AuthService) { }

  preOrder(data: ProductModel): Promise<any> {
    return this.http.post(this.baseUrl+'api/order/preorder', data).toPromise();
  }

  getOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order').toPromise();
  }

  getUserOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'api/order/own').toPromise();
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

  setOrderStatus(orderId: number, statusId: number): Promise<any> {
    return this.http.post(
      `${this.baseUrl}api/order/status/${orderId}`, statusId)
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

  addPnToCart(code: number): Promise<any> {
    return this.http.post(
      `${this.baseUrl}api/cart/add`,
      {
        uid: this.userTempUid,
        code
      }
    ).toPromise();
  }

  removeFromCart(id: number): Promise<any> {
    return this.http.post(
      `${this.baseUrl}api/cart/remove`,
      {
        code: id,
        uid: this.userTempUid
      }
    ).toPromise();
  }

  getUserCart(): Promise<any> {
    return this.http.get(`${this.baseUrl}api/cart/${this.userTempUid}`).toPromise();
  }

  makeOrder(): Promise<any> {
    return this.http.post(`${this.baseUrl}api/cart/order`, {}).toPromise();
  }

  mergeCart(): Promise<any> {
    return this.http.post(`${this.baseUrl}api/cart/merge/${this.userTempUid}`, {}).toPromise();
  }
}
