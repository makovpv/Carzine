import { HttpClient } from '@angular/common/http';
import { EventEmitter, Inject, Injectable, Output } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  @Output() changeUserName: EventEmitter<string> = new EventEmitter();

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getAuthToken(email: string, password: string): Promise<any> {
    return this.http.post(this.baseUrl+'user/token', { email, password })
      .toPromise()
      .then((res: any) => {
        this.changeUserName.emit(email);
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
  }

  logout() {
    this.changeUserName.emit('');
    localStorage.removeItem("access_token");
  }

// public isLoggedIn() {
//     return moment().isBefore(this.getExpiration());
// }

// isLoggedOut() {
//     return !this.isLoggedIn();
// }
}
