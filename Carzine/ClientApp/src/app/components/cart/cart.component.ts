import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { MessageService } from 'src/app/services/message.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  items: any[] = [];
  isUserLogged = false;

  constructor(private orderService: OrderService, private messageService: MessageService,
    private authService: AuthService) { }

  ngOnInit(): void {
    this.orderService.getUserCart().then((data) => this.items = data);

    this.isUserLogged = this.authService.isLoggedIn();
  }

  totalSum = () => this.items.map(x => x.price_rub).reduce((a,b) => a + b, 0);

  makeOrder() {
    this.orderService.makeOrder()
      .then(data => {
        this.messageService.sendMessage('Заказ оформлен', 3000);
        this.items = [];
      });
  }

  removeFromCart(id: number) {
    this.orderService.removeFromCart(id)
      .then(data => {
        this.items = this.items.filter(x => x.id !== id)
      })
      .catch(err => this.messageService.sendErrorMessage(err.error));
  }

}
