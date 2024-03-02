import { Component, Inject, Injectable, TemplateRef, ViewChild } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';
import { MatDialog } from '@angular/material/dialog';
import { SearchService } from '../services/search.service';
import { OrderService } from '../services/order.service';
import { CarModificationModel } from '../models/CarModificationModel';
import { CarPartsGroupModel } from '../models/CarPartsGroupModel';
import { MarkModel, CarModel } from '../models/CarModel';
import { GroupPartListModel } from '../models/PartNumberModel';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Router } from '@angular/router';
import { MessageService } from '../services/message.service';
import { ViewportScroller } from '@angular/common';

/** Flat node with expandable and level information */
export class DynamicFlatNode {
  constructor(
    public itemName: string,
    public level = 1,
    public expandable = false,
    public isLoading = false,
    public id: string,
    public parentId: string | undefined,
    public groupTypeId = '',
    public mark = '',
    public modification = '',
    public model = '',
    public hasParts = false
  ) {}
}

@Injectable({providedIn: 'root'})
export class DynamicDatabase {
  partGroups: CarPartsGroupModel[] = [];
  groupTypeId = '';
  mark = '';
  modification = '';
  model = '';

  constructor(private searchService: SearchService) { }

  initialData(typeId: string, mark: string, modification: string,
    model: string, groups: CarPartsGroupModel[]): DynamicFlatNode[] {
    this.groupTypeId = typeId;
    this.mark = mark;
    this.modification = modification;
    this.model = model;
    this.partGroups = groups;

    return this.partGroups.map(x =>
      new DynamicFlatNode(x.name!, 0, x.hasSubgroups, true, x.id, x.parentId, typeId, mark, modification, model)
    );
  }

  getChildren(nodeKey: string, groupInfo: any): Promise<CarPartsGroupModel[]> | undefined {
    let ww = this.partGroups.filter(x => x.parentId === nodeKey);
    if (ww.length === 0) {
      return this.searchService.getGroupItems(this.groupTypeId, groupInfo.id, 
        groupInfo.mark, groupInfo.modification, groupInfo.model, groupInfo.parentId ?? '').then((data: any) => {
          this.partGroups.push(...data.groups);
          return data.groups;
        });
    }

    return new Promise<CarPartsGroupModel[]>((resolve, reject) => {
      resolve(ww);
    });
  }

  isExpandable(item: CarPartsGroupModel): boolean {
    return item.hasSubgroups!;
  }
}

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
  includeAnalog = true;
  modification = new CarModificationModel();
  model = new CarModel();
  mark = new MarkModel();
  fastSearch: any;
  carType = "";
  currentGroup = new CarPartsGroupModel();
  groupParts = new GroupPartListModel();
  treeControl: FlatTreeControl<DynamicFlatNode>;
  dataSource: any;
  schemeImage: any;
  showAllProducts = false;
  myRootPartGroups: CarPartsGroupModel[] = [];
  myPartGroups1: CarPartsGroupModel[] = [];
  myPartGroups2: any[] = [];
  myPartGroups3: any[] = [];
  breadCrumbGroups: any[] = [{name: '<-- Назад', bcLevel: 0}];
  
  getLevel = (node: DynamicFlatNode) => node.level;
  isExpandable = (node: DynamicFlatNode) => node.expandable;
  hasChild = (_: number, node: any) => node.expandable;

  options: string[] | undefined = [];

  @ViewChild('dialogRef')
  dialogRef!: TemplateRef<any>;

  constructor(
    private searchService: SearchService,
    private orderService: OrderService,
    private messageService: MessageService,
    private router: Router,
    private scroller: ViewportScroller,
    public dialog: MatDialog,
    private database: DynamicDatabase) { 
      this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    }

  ngOnInit() {
    const searchHistory = localStorage.getItem('searchHistory');
    this.options = searchHistory?.split(';') ?? [];

    //if auth
    this.searchService.getUserGarage(1).then((data: any[]) => {
      if (!!data && data.length === 1) {
        this.fastSearch = data[0];
      }
    }).catch(err => {
      this.fastSearch = null;
    });
  }

  onAreaMouseOver(lbl: any) {}
  onAreaMouseOut() { }

  searchById(labelId: any) {
    const PN = this.groupParts.numbers.find(x => x.labelId === labelId)?.number;
    this.searchByPn(PN!);
  }

  getNodeParts(node: any) {
    this.searchService.getGroupParts(node.groupTypeId, node.id, node.mark, node.modification, 
      node.model, node.parentId ?? '')
        .then((result: any) => {
          this.inProgress = false;

          this.groupParts = result;
        });

    this.schemeImage = this.searchService.getSchemeUrl(node.groupTypeId, node.id, node.mark, node.modification,
     node.model, node.parentId ?? '');
  }

  isVinCode(code: string): boolean {
    if (code === '123') // !!!
      return true;

    return code.length === 17 && 
      !(code.toUpperCase().includes('I') || code.toUpperCase().includes('O') || code.toUpperCase().includes('Q'));
  }

  search() {
    if (!this.searchCode) {
      this.messageService.sendMessage('Укажите код для поиска', 3000);
      return;
    }

    if ((this.options?.indexOf(this.searchCode) ?? -1) === -1) {
      this.options?.push(this.searchCode);
      localStorage.setItem('searchHistory', this.options?.join(';') ?? '');
    }

    this.inProgress = true;

    if (this.isVinCode(this.searchCode)) {
      this.searchByVin(this.searchCode)
    }
    else {
      this.searchByPn(this.searchCode);
    }
  }

  searchByPn(pn: string) {
    this.scroller.scrollToAnchor("pn-search-result-anchor");
    
    this.searchService.search(pn, this.includeAnalog)
      .then((result: ProductSearchResultModel) => {
        this.inProgress = false;
        this.showAllProducts = false;
        this.searchResult = result;
        
        if (result.products.length === 0) {
          this.messageService.sendErrorMessage("К сожалению сейчас нет той детали, которую вы ищете, проверьте правильность номера");
          return;
        }

        this.scroller.scrollToAnchor("pn-search-result");
      })
      .catch((err) => {
        this.inProgress = false;
        this.messageService.sendErrorMessage(err.error);
      });
  }

  searchByVin(vin: string) {
    this.searchService.searchVIN(vin)
      .then((result: any) => {
        this.inProgress = false;

        this.schemeImage = null;
        this.groupParts.numbers = [];

        if (!result.type || !result.model) {
          this.messageService.sendErrorMessage("Ничего не найдено по данному VIN");
          return;
        }

        this.modification = result.modification;
        this.model = result.model;
        this.mark = result.mark;
        this.carType = result.type.id;

        this.myRootPartGroups = result.groups;
        this.myPartGroups1 = result.groups.map((x: any) => ({...x, bcLevel: 1}));
        this.breadCrumbGroups = [this.breadCrumbGroups.find(x => x.bcLevel === 0)];
      })
      .catch((err) => {
        this.inProgress = false;
        this.messageService.sendErrorMessage(err.error);
      });
  }

  addPreorder(product: ProductModel) {
    if (!product.partNumber) {
      this.messageService.sendErrorMessage('Не определен номер з/части');
      return;
    }
    
    this.orderService.preOrder(product).then(() => {
      this.messageService.sendMessage('Создан предзаказ', 10000);
    })
    .catch(err => {
      if (err.status === 401) {
        this.router.navigateByUrl('/login');
      }
      else {
        this.messageService.sendErrorMessage('Что то пошло не так');
      }
    })
  }

  onSearchKeyDown(e: any) {
    if (e.code === 'Enter') {
      this.search();
    }
  }

  foo(selectedGroup: CarPartsGroupModel, level: number) {
    if (this.breadCrumbGroups.length === selectedGroup.bcLevel + 1)
      return;

    this.searchResult.products = [];

    if (selectedGroup.bcLevel === 0) {
      this.breadCrumbGroups = [selectedGroup];
      this.myPartGroups1 = this.myRootPartGroups.map((x: any) => ({...x, bcLevel: 1}));
      this.groupParts.labels = [];
      return;
    }
    
    this.breadCrumbGroups = this.breadCrumbGroups.filter(x => x.bcLevel <= selectedGroup.bcLevel);

    this.breadCrumbGroups.push(selectedGroup);
    
    if (selectedGroup.hasSubgroups) {
      this.searchService.getGroupItems(this.carType, selectedGroup.id, this.mark.id, this.modification.id, this.model.id, selectedGroup.parentId!) //ParentGroup
      .then(data => {
        
        this.myPartGroups1 = data.groups.map((x: any) => ({...x, bcLevel: selectedGroup.bcLevel + 1}));

        if (level === 0) {
          while (this.breadCrumbGroups.length > (selectedGroup.bcLevel + 1)) {
            this.breadCrumbGroups.pop();
          }
        }
      });
    }
    else {
      this.myPartGroups1 = [];
    }

    if (selectedGroup.hasParts) {
      this.searchService.getGroupParts(this.carType, selectedGroup.id, this.mark.id, this.modification.id, this.model.id, selectedGroup.parentId!)
        .then(data => {
          this.groupParts = data;
        })

      this.schemeImage = this.searchService.getSchemeUrl(this.carType, selectedGroup.id, this.mark.id, this.modification.id, this.model.id, selectedGroup.parentId ?? '');
    }
    else {
      this.groupParts.labels = [];
    }
  }

  fastSearchOnClick() {
    this.searchCode = this.fastSearch.vin; 
    this.searchByVin(this.fastSearch.vin);
  }
}
