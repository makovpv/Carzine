import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ProductSearchResultModel } from '../models/ProductSearchResultModel';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  search(searchCode: string, analog: boolean): Promise<any> {
      return this.http.get<ProductSearchResultModel>(
        `${this.baseUrl}search?code=${searchCode}&analog=${analog}`)
        .toPromise();
  }

  searchVIN(searchVIN: string): Promise<any> {
      return this.http.get<any>(
        `${this.baseUrl}search/searchVIN?vin=${searchVIN}`)
        .toPromise();
  }

  getGroupItems(
    groupType: string, 
    groupId: string, 
    mark: string, 
    modification: string, 
    model: string, 
    parentGroup: string ): Promise<any> {
      return this.http.get<any>(
        `${this.baseUrl}search/groups?groupType=${groupType}&group=${groupId}&Mark=${mark}&Modification=${modification}&Model=${model}&ParentGroup=${parentGroup}`)
        .toPromise();
  }

  getGroupParts(
    groupType: string, 
    groupId: string, 
    mark: string, 
    modification: string, 
    model: string, 
    parentGroup: string ): Promise<any> {
      return this.http.get<any>(
        `${this.baseUrl}search/parts?groupType=${groupType}&group=${groupId}&Mark=${mark}&Modification=${modification}&Model=${model}&ParentGroup=${parentGroup}`)
        .toPromise();
  }
}
