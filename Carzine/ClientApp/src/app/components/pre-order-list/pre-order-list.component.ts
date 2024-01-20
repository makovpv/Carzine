import { Component, OnInit } from '@angular/core';
import { PreOrderModel } from '../../models/PreOrderModel';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-pre-order-list',
  templateUrl: './pre-order-list.component.html',
  styleUrls: ['./pre-order-list.component.css'],
  preserveWhitespaces: true
})
export class PreOrderListComponent implements OnInit {
  preOrders: PreOrderModel[] = [];
  suppliers: any[] = [];
  inProgress = false;

  constructor(
    private orderService: OrderService) {

    this.inProgress = true;

    this.orderService.getPreOrders().then((data: any) => {
      this.preOrders = data;
      this.inProgress = false;
	  });
  }

  ngOnInit(): void {
  }

  openDialog(preOrderId: number | undefined) {
    if (!preOrderId)
      return;
    
    this.orderService.createOrder(preOrderId)
      .then((x: any) => {
        alert(x.res.info);
      }
        )
      .catch((err: any) => {
        alert(err.error.text ?? err.error)
      });
  }

  getSupplierName(id?: number): string | undefined {
    return this.orderService.suppliers.find((x: any)=> x?.id === id)?.name;
  }
}
