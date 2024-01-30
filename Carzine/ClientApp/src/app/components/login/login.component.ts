import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  preserveWhitespaces: true
})
export class LoginComponent implements OnInit {
  login: string = '';
  password: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  loginClick(): void {
    this.authService.getAuthToken(this.login, this.password)
      .then((data) => {
        this.router.navigateByUrl('/');
      })
      .catch(err => {
        if (err.status === 401) {
          alert('В доступе отказано')
        }
        else {
          alert(err.message);
        }
      });
  }

  signUpClick() {
    this.authService.signUp(this.login, this.password)
    .then(() => alert('User created'))
    .catch(err => alert(err.error));
  }
}
