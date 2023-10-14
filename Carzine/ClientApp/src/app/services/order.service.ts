import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { PreOrderModel } from '../models/PreOrderModel';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  preOrder(data: PreOrderModel): Promise<any> {
    return this.http.post(this.baseUrl+'order/preorder', data).toPromise();
  }

  getPreOrders(): Promise<any> {
    return this.http.get(this.baseUrl+'order/preorder').toPromise();
  }
}
