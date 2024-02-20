import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { TranslationModel } from '../models/TranslationModel';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getAllTranslation(): Promise<any> {
    return this.http.get(this.baseUrl+'api/translation').toPromise();
  }

  addTranslation(translation: TranslationModel): Promise<any> {
    return this.http.post(this.baseUrl+'api/translation', translation).toPromise();
  }

  deleteTranslation(key: string): Promise<any> {
    return this.http.delete(this.baseUrl+`api/translation/${key}`).toPromise();
  }
}
