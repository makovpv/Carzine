import { Component, OnInit } from '@angular/core';
import { OrderStatusModel } from 'src/app/models/OrderStatusModel';
import { PreOrderModel } from 'src/app/models/PreOrderModel';
import { OrderService } from 'src/app/services/order.service';
import { SearchService } from 'src/app/services/search.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  userPreorders: PreOrderModel[] = [];
  userCars: any[] = [];
  statuses: OrderStatusModel[] = [];
  inProgress = false;

  constructor(private orderService: OrderService, private searchService: SearchService) { }

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

    this.searchService.getUserGarage(3).then(data => {this.userCars = data});
  }

}
