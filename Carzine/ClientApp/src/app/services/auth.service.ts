import { HttpClient } from '@angular/common/http';
import { EventEmitter, Inject, Injectable, Output } from '@angular/core';
import { UserModel } from '../models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  @Output() changeUserName: EventEmitter<UserModel> = new EventEmitter();

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getAuthToken(email: string, password: string): Promise<any> {
    return this.http.post(this.baseUrl+'user/token', { email, password })
      .toPromise()
      .then((res: any) => {
        this.changeUserName.emit({email, isProfUser: res.value.isProfUser});
        this.setSession(res.value);
      });
  }

  signUp(email: string, password: string, phone: string): Promise<any> {
    return this.http.post(this.baseUrl+'user/signup', { 
      name: email,
      phone: phone,
      pwd: password })
      .toPromise();
  }

  private setSession(authResult: any) {
    localStorage.setItem('access_token', authResult.access_token);
    localStorage.setItem('access_token_expires', authResult.expires);
    localStorage.setItem('userName', authResult.userName);
    localStorage.setItem('isProfUser', authResult.isProfUser);
  }

  logout() {
    this.changeUserName.emit({email: undefined, isProfUser: false});
    localStorage.removeItem("access_token");
    localStorage.removeItem("access_token_expires");
    localStorage.removeItem("userName");
    localStorage.removeItem("isProfUser");
  }

// public isLoggedIn() {
//     return moment().isBefore(this.getExpiration());
// }

// isLoggedOut() {
//     return !this.isLoggedIn();
// }
}
