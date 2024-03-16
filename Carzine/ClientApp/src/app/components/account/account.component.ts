import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderStatusModel } from 'src/app/models/OrderStatusModel';
import { PreOrderModel } from 'src/app/models/PreOrderModel';
import { MessageService } from 'src/app/services/message.service';
import { OrderService } from 'src/app/services/order.service';
import { PaymentService } from 'src/app/services/payment.service';
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
  payOrderId = '';

  constructor(private orderService: OrderService, private searchService: SearchService,
    private payService: PaymentService, private messageService: MessageService,
    private activateRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.payOrderId = this.activateRoute.snapshot.queryParams["orderId"];
    if (this.payOrderId) {
      this.payService.checkOrder(this.payOrderId).then(data => {
        this.messageService.sendMessage(data.paymentAmountInfo.paymentState, 5000);
      });
    }

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

  payClick(orderId: any) {
    this.payService.payOrder(orderId).then(data => {
      if (data.errorCode === 1) {
        this.messageService.sendErrorMessage(data.errorMessage);
        return;
      }
      
      window.location.assign(data.formUrl);
    })
    .catch(err =>
      this.messageService.sendErrorMessage(err.message)
    );
  }

}
