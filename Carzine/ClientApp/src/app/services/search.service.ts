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
        `${this.baseUrl}search/searchVIN?vin=${searchVIN}&requestEcoMode=${searchVIN === '123'}`)
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

  getSchemeUrl(groupType: string,
    groupId: string,
    mark: string,
    modification: string,
    model: string,
    parentGroup: string): string {
    return `${this.baseUrl}search/scheme?groupType=${groupType}&group=${groupId}&Mark=${mark}&Modification=${modification}&Model=${model}&ParentGroup=${parentGroup}`;
  }

  getUserGarage(count: number): Promise<any> {
    const tokenExpires = localStorage.getItem("access_token_expires");

    if (!tokenExpires || (new Date(tokenExpires) < new Date()))
      return new Promise((resolve, reject) => { reject() });

    return this.http.get<any>(`${this.baseUrl}search/garage/${count}`).toPromise();
  }
}
