import { Component, OnInit } from '@angular/core';
import { OrderStatusModel } from 'src/app/models/OrderStatusModel';
import { PreOrderModel } from 'src/app/models/PreOrderModel';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  userPreorders: PreOrderModel[] = [];
  statuses: OrderStatusModel[] = [];
  inProgress = false;

  constructor(private orderService: OrderService) { }

  ngOnInit(): void {
    const f1 = this.orderService.getClientStatuses();
    const f2 = this.orderService.getUserPreOrders();

    Promise.all([f1, f2]).then(data => {
      this.statuses = data[0];
      this.userPreorders = data[1];
      this.userPreorders.forEach(x => x.clientStatusName = this.statuses.find(s => s.id === x.clientStatus)?.name ?? '??');

      this.inProgress = false;
    })
    .catch(err => alert(err.message));
  }

}
