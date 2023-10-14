import { HttpClient } from '@angular/common/http';
import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';
import { MatDialog } from '@angular/material/dialog';
import { PreOrderComponent } from '../components/dialogs/pre-order/pre-order.component';
import { SearchService } from '../services/search.service';
import { OrderService } from '../services/order.service';
import { PreOrderModel } from '../models/PreOrderModel';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  preserveWhitespaces: true
})
export class HomeComponent {
  searchResult = new ProductSearchResultModel();
  inProgress = false;
  searchCode: string = "31126753992";

  @ViewChild('dialogRef')
  dialogRef!: TemplateRef<any>;

  constructor(
    private searchService: SearchService,
    private orderService: OrderService,
    public dialog: MatDialog) { }

  search1() {
    console.log(this.searchCode);

    this.inProgress = true;

    this.searchService.search(this.searchCode, 1)
      .then((result: ProductSearchResultModel) => {
        this.inProgress = false;
        this.searchResult = result;
        console.log(result.products);
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error)
      });
  }

  search2() {
    this.inProgress = true;

    this.searchService.search(this.searchCode, 2)
      .then((result: ProductSearchResultModel) => {
        this.inProgress = false;
        this.searchResult = result;
        console.log(result.products);
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error);
      });
  }

  openDialog(product: ProductModel) {
    this.dialog.open(PreOrderComponent)
    .afterClosed()
    .subscribe((res) => {
      if (res && res.event === 'ok' && res.data) {
        const preOrderData: PreOrderModel = {
          phone: res.data,
          partNumber: product.partNumber,
          manufacturer: product.manufacturer,
          priceRub: product.priceRub,
          deliveryMin: product.deliveryMin
        }

        this.orderService.preOrder(preOrderData).then(() => {
          alert('Pre-order has been sent');
        })
        .catch(err => {
          alert(err.error);
        })
      }
    })
  }

}
