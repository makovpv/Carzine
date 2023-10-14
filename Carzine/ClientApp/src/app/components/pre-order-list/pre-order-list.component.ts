import { Component, OnInit } from '@angular/core';
import { PreOrderModel } from 'src/app/models/PreOrderModel';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-pre-order-list',
  templateUrl: './pre-order-list.component.html',
  styleUrls: ['./pre-order-list.component.css'],
  preserveWhitespaces: true
})
export class PreOrderListComponent implements OnInit {
  preOrders: PreOrderModel[] = [];

  constructor(
    private orderService: OrderService) {
      this.orderService.getPreOrders()
        .then((data: any) => this.preOrders = data);
    }

  ngOnInit(): void {
  }

}
