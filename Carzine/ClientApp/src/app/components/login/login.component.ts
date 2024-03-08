import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
  }

  loginClick(): void {
    this.authService.getAuthToken(this.login, this.password)
      .then((data) => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
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
}
