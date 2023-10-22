import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';
import { MatDialog } from '@angular/material/dialog';
import { PreOrderComponent } from '../components/dialogs/pre-order/pre-order.component';
import { SearchService } from '../services/search.service';
import { OrderService } from '../services/order.service';
import { PreOrderModel } from '../models/PreOrderModel';
import { CarModificationModel } from '../models/CarModificationModel';
import { CarPartsGroupModel } from '../models/CarPartsGroupModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MarkModel, CarModel } from '../models/CarModel';
import { PartNumberModel } from '../models/PartNumberModel';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  preserveWhitespaces: true
})
export class HomeComponent {
  searchResult = new ProductSearchResultModel();
  inProgress = false;
  searchCode = ""; //"31126753992";
  searchVinCode = "";
  includeAnalog = true;
  modification = new CarModificationModel();
  model = new CarModel();
  mark = new MarkModel();
  carType = "";
  partGroups: CarPartsGroupModel[] = [];
  currentGroupId = "";
  partNumbers: PartNumberModel[] = [];

  @ViewChild('dialogRef')
  dialogRef!: TemplateRef<any>;

  constructor(
    private searchService: SearchService,
    private orderService: OrderService,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar) { }

  search() {
    if (!this.searchCode) {
      this.showSnack('Укажите код детали для поиска', 3000);
      return;
    }

    this.inProgress = true;

    this.searchService.search(this.searchCode, this.includeAnalog)
      .then((result: ProductSearchResultModel) => {
        this.inProgress = false;
        this.searchResult = result;
        
        if (result.products.length === 0) {
          this.showSnack("К сожалению сейчас нет той детали, которую вы ищете, проверьте правильность номера", 10000);
        }
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error)
      });
  }

  searchVIN() {
    if (!this.searchVIN) {
      this.showSnack('Укажите VIN для поиска', 3000);
      return;
    }

    this.inProgress = true;

    this.searchService.searchVIN(this.searchVinCode)
      .then((result: any) => {
        this.inProgress = false;

	      this.modification = result.modification;
        this.model = result.model;
        this.mark = result.mark;
	      this.partGroups = result.groups;
        this.carType = result.type.id;
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error)
      });
  }

  getGroupItems(group: CarPartsGroupModel) {
    this.inProgress = true;

    if (group.hasSubgroups) {
      this.searchService.getGroupItems(this.carType, group.id, this.mark.id, this.modification.id, this.model.id, this.currentGroupId ?? '')
        .then((result: any) => {
          this.inProgress = false;

          this.currentGroupId = group.id;
          this.partGroups = result.groups;
        });
    };

    if (group.hasParts) {
      this.searchService.getGroupParts(this.carType, group.id, this.mark.id, this.modification.id, this.model.id, this.currentGroupId ?? '')
        .then((result: any) => {
          this.inProgress = false;

          this.partNumbers = result.numbers;
        });
    };

  }

  showSnack(message: string, duration: number) {
    this._snackBar.open(message, "OK", {
      duration: duration,
      panelClass: ['cz-snack-panel'],
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
          deliveryMin: product.deliveryMin,
          id: undefined,
          sourceId: product.sourceId
        }

        this.orderService.preOrder(preOrderData).then(() => {
          this.showSnack('Создан предзаказ', 10000);
        })
        .catch(err => {
          alert(err.error);
        })
      }
    })
  }

}
