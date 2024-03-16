import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  payOrder(orderId: number): Promise<any> {
    return this.http.post(`${this.baseUrl}api/payment/pay/${orderId}`, {}).toPromise();
  }

  checkOrder(orderId: string): Promise<any> {
    return this.http.post(`${this.baseUrl}api/payment/check/${orderId}`, {}).toPromise();
  }
}
