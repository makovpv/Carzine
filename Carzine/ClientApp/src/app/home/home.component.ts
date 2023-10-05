import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ProductModel } from '../models/ProductModel';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  preserveWhitespaces: true
})
export class HomeComponent {
  searchResult = new ProductSearchResultModel();
  inProgress = false;
  searchCode: string = "31126753992";

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  search1() {
    console.log(this.searchCode);

    this.searchResult = new ProductSearchResultModel();
    this.inProgress = true;

     this.http.get<ProductSearchResultModel>(this.baseUrl + 'search?code=' +this.searchCode  + '&mode=1')
     .subscribe((result: ProductSearchResultModel) => {
      this.inProgress = false;
       this.searchResult = result;
       console.log(result.products);
     }, error => console.error(error));
  }

  search2() {
    this.inProgress = true;
    this.searchResult = new ProductSearchResultModel();

     this.http.get<ProductSearchResultModel>(this.baseUrl + 'search?code=' +this.searchCode + '&mode=2')
     .subscribe((result: ProductSearchResultModel) => {
      this.inProgress = false;
      this.searchResult = result;
      console.log(result.products);
     }, error => {
	console.error(error);
	alert(error.error)
	}
);
  }

}
