import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { PreOrderModel } from '../../models/PreOrderModel';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { StatusComponent } from '../dialogs/status/status.component';
import { MessageService } from 'src/app/services/message.service';
import { OrderStatusModel } from 'src/app/models/OrderStatusModel';

@Component({
  selector: 'app-pre-order-list',
  templateUrl: './pre-order-list.component.html',
  styleUrls: ['./pre-order-list.component.css'],
  preserveWhitespaces: true
})
export class PreOrderListComponent implements OnInit {
  preOrders: PreOrderModel[] = [];
  suppliers: any[] = [];
  statuses: OrderStatusModel[] = [];
  inProgress = false;
  displayCols: string[] = ['id', 'date', 'phone', 'userEmail', 'volume', 'weight', 'supplierName',
    'price', 'deliveryCost', 'extraCharge', 'deliveryOrderStatus', 'partNumber',
    'manufacturer', 'priceRub', 'deliveryMin','clientOrderStatus', 'cmd'];

  @ViewChild('dialogRef')
  dialogRef!: TemplateRef<any>;

  constructor(
    private orderService: OrderService, 
    private messageService: MessageService,
    private router: Router,
    public dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.inProgress = true;

    const f1 = this.orderService.getClientStatuses();
    const f2 = this.orderService.getPreOrders();

    Promise.all([f1, f2]).then(data => {
      this.statuses = data[0];
      this.preOrders = data[1];
      this.preOrders.forEach(x => x.clientStatusName = this.statuses.find(s => s.id === x.clientStatus)?.name ?? '??');

      this.inProgress = false;
    })
    .catch(err => {
      this.inProgress = false;
      if (err.status === 401) {
        this.router.navigateByUrl('/login');
      }
      if (err.status === 403) {
        alert('Недостаточно прав доступа')
      }
    });
  }

  openDialog(preOrderId: number | undefined) {
    if (!preOrderId)
      return;
    
    if (confirm("Создать реальный ордер?")) {
      this.orderService.createOrder(preOrderId)
      .then((x: any) => {
        alert(x.res.info);
      }
        )
      .catch((err: any) => {
        alert(err.error.text ?? err.error)
      });
    }
  }

  openStatusDialog(x: PreOrderModel) {
    this.dialog.open(StatusComponent, {data: {
      statusId: x.clientStatus,
      statuses: this.statuses
    }})
    .afterClosed()
    .subscribe((res) => {
      if (res && res.event === 'ok' && res.data) {
        this.orderService.setPreorderStatus(x.id!, res.data)
        .then(() => {
          this.messageService.sendMessage('Статус изменен', 5000);
          x.clientStatus = res.data;
          x.clientStatusName = this.statuses.find(s => s.id === res.data)?.name!
        })
        .catch(err => {
          this.messageService.sendErrorMessage(err.message);
        })
      }
    })
  }

  getSupplierName(id?: number): string | undefined {
    return this.orderService.suppliers.find((x: any)=> x?.id === id)?.name;
  }
}
