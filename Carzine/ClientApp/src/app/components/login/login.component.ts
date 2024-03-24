import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { MessageService } from 'src/app/services/message.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  preserveWhitespaces: true
})
export class LoginComponent implements OnInit {
  login: string = '';
  password: string = '';
  inProgress = false;

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute,
    private messageServer: MessageService, private orderService: OrderService) { }

  ngOnInit(): void {
  }

  loginClick(): void {
    this.inProgress = true;

    this.authService.getAuthToken(this.login, this.password)
      .then((data) => {
        this.inProgress = false;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      })
      .then((data) => {
        this.orderService.mergeCart();
      })
      .catch(err => {
        this.inProgress = false;
        
        if (err.status === 401) {
          this.messageServer.sendErrorMessage('В доступе отказано');
        }
        else if (err.status === 504) {
          this.messageServer.sendErrorMessage('Сервер не отвечает (504)');
        }
        else {
          this.messageServer.sendErrorMessage(err.message);
        }
      });
  }
}
