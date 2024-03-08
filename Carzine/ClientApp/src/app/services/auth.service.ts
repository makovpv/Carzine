import { HttpClient } from '@angular/common/http';
import { EventEmitter, Inject, Injectable, Output } from '@angular/core';
import { UserModel, UserSessionModel } from '../models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  @Output() changeUserName: EventEmitter<UserModel> = new EventEmitter();

  sessionData: UserSessionModel;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.sessionData = this.getSessionData();
  }

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

  private getSessionData(): UserSessionModel {
    return {
      access_token: localStorage.getItem('access_token'),
      access_token_expires: new Date(localStorage.getItem('access_token_expires') ?? 0),
      userName: localStorage.getItem('userName'),
      isProfUser: localStorage.getItem('isProfUser') == 'true'
    };
  }

  private setSession(authResult: any) {
    this.sessionData.access_token = authResult.access_token
    this.sessionData.access_token_expires = new Date(authResult.expires);
    
    localStorage.setItem('access_token', authResult.access_token);
    localStorage.setItem('access_token_expires', authResult.expires);
    localStorage.setItem('userName', authResult.userName);
    localStorage.setItem('isProfUser', authResult.isProfUser);
  }

  logout() {
    this.sessionData.access_token = null;

    this.changeUserName.emit({email: undefined, isProfUser: false});
    localStorage.removeItem("access_token");
    localStorage.removeItem("access_token_expires");
    localStorage.removeItem("userName");
    localStorage.removeItem("isProfUser");
  }

  public isLoggedIn() {
    return !!this.sessionData.access_token && (this.sessionData.access_token_expires?.getTime()! > Date.now());
  }

// isLoggedOut() {
//     return !this.isLoggedIn();
// }
}
