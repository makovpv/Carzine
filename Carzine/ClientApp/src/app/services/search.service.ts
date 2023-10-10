import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  search(searchCode: string, mode: number): Promise<any> {
    return this.http.get<ProductSearchResultModel>(
      this.baseUrl + 'search?code=' +searchCode + '&mode=' + mode)
      .toPromise();
  }
}
