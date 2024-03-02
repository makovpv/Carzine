import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  userName: string | undefined | null;
  isProfUser = false;

  constructor(private authService: AuthService, private router: Router) {
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    }

    this.router.onSameUrlNavigation = 'reload';
  }

  ngOnInit() {
    this.userName = localStorage.getItem('userName');
    this.isProfUser = localStorage.getItem('isProfUser') === 'true';

    this.authService.changeUserName.subscribe(data => {
      this.userName = data.email;
      this.isProfUser = data.isProfUser;
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('/');
  }
}
