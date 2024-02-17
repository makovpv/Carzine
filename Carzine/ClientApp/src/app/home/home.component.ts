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
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Observable } from 'rxjs/internal/Observable';
import { CollectionViewer, DataSource, SelectionChange } from '@angular/cdk/collections';
import { merge } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MessageService } from '../services/message.service';

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

export class DynamicDataSource implements DataSource<DynamicFlatNode> {
  dataChange = new BehaviorSubject<DynamicFlatNode[]>([]);

  get data(): DynamicFlatNode[] {
    return this.dataChange.value;
  }
  set data(value: DynamicFlatNode[]) {
    this._treeControl.dataNodes = value;
    this.dataChange.next(value);
  }

  constructor(
    private _treeControl: FlatTreeControl<DynamicFlatNode>,
    private _database: DynamicDatabase,
  ) {}

  connect(collectionViewer: CollectionViewer): Observable<DynamicFlatNode[]> {
    this._treeControl.expansionModel.changed.subscribe(change => {
      if (
        (change as SelectionChange<DynamicFlatNode>).added ||
        (change as SelectionChange<DynamicFlatNode>).removed
      ) {
        this.handleTreeControl(change as SelectionChange<DynamicFlatNode>);
      }
    });

    return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
  }

  disconnect(collectionViewer: CollectionViewer): void {}

   /** Handle expand/collapse behaviors */
   handleTreeControl(change: SelectionChange<DynamicFlatNode>) {
    if (change.added) {
      change.added.forEach(node => this.toggleNode(node, true));
    }
    if (change.removed) {
      change.removed
        .slice()
        .reverse()
        .forEach(node => this.toggleNode(node, false));
    }
  }

  /**
   * Toggle the node, remove from display list
   */
  toggleNode(node: DynamicFlatNode, expand: boolean) {
    //const children = this._database.getChildren(node.item);////////!!!!!!!
    this._database.getChildren(node.id, node)?.then((data: any) => {
      const children = data;

      const index = this.data.indexOf(node);
      if (!children || index < 0) {
        // If no children, or cannot find the node, no op
        return;
      }

      node.isLoading = true;

      if (expand) {
        const nodes = children.map(
          (item: any) => new DynamicFlatNode(
            item.name!, node.level + 1, 
            this._database.isExpandable(item), 
            false,
            item.id, item.parentId,
            node.groupTypeId,
            node.mark,
            node.modification,
            node.model,
            item.hasParts),
        );
        this.data.splice(index + 1, 0, ...nodes);
      } else {
        let count = 0;
        for (
          let i = index + 1;
          i < this.data.length && this.data[i].level > node.level;
          i++, count++
        ) {}
        this.data.splice(index + 1, count);
      }

        // notify the change
        this.dataChange.next(this.data);
        node.isLoading = false;
    }) ;
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
  carType = "";
  currentGroup = new CarPartsGroupModel();
  groupParts = new GroupPartListModel();
  treeControl: FlatTreeControl<DynamicFlatNode>;
  dataSource: any;
  schemeImage: any;
  showAllProducts = false;
  
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
    public dialog: MatDialog,
    private database: DynamicDatabase) { 
      this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    }

  ngOnInit() {
    const searchHistory = localStorage.getItem('searchHistory');
    this.options = searchHistory?.split(';') ?? [];
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
      this.messageService.sendMessage('Укажите код для поиска', 3000); // showSnack('Укажите код для поиска', 3000);
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
    this.searchService.search(pn, this.includeAnalog)
      .then((result: ProductSearchResultModel) => {
        this.inProgress = false;
        this.showAllProducts = false;
        this.searchResult = result;
        
        if (result.products.length === 0) {
          this.messageService.sendErrorMessage("К сожалению сейчас нет той детали, которую вы ищете, проверьте правильность номера");
        }
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error)
      });
  }

  searchByVin(vin: string) {
    this.searchService.searchVIN(vin)
      .then((result: any) => {
        this.inProgress = false;

        if (!result.type || !result.model) {
          this.messageService.sendErrorMessage("Ничего не найдено по данному VIN");
          return;
        }

	      this.modification = result.modification;
        this.model = result.model;
        this.mark = result.mark;
        this.carType = result.type.id;

        this.dataSource = new DynamicDataSource(this.treeControl, this.database);
        this.dataSource!.data = this.database.initialData(
          result.type.id,
          this.mark.id, 
          this.modification.id,
          this.model.id, 
          result.groups);
      })
      .catch((err) => {
        this.inProgress = false;
        alert(err.error)
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
}
