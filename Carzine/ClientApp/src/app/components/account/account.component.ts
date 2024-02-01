import { Component, OnInit } from '@angular/core';
import { PreOrderModel } from 'src/app/models/PreOrderModel';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  userPreorders: PreOrderModel[] = [];
  inProgress = false;

  constructor(private orderService: OrderService) { }

  ngOnInit(): void {
    this.orderService.getUserPreOrders()
      .then(data => this.userPreorders = data)
      .catch(err => alert(err.message));
  }

}
