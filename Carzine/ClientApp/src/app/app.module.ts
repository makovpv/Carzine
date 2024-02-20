import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PreOrderComponent } from './components/dialogs/pre-order/pre-order.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatTableModule } from '@angular/material/table';
import { PreOrderListComponent } from './components/pre-order-list/pre-order-list.component';
import { IntCurrencyPipe } from './pipes/int-currency.pipe';
import { AppFooterComponent } from './components/app-footer/app-footer.component';
import { LoginComponent } from './components/login/login.component';
import { AuthInterceptor } from './auth.interceptor';
import { AccountComponent } from './components/account/account.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { DeliveryPeriodPipe } from './pipes/delivery-period.pipe';
import { StatusComponent } from './components/dialogs/status/status.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AboutUsComponent } from './components/info/about-us/about-us.component';
import { PublicOfferComponent } from './components/info/public-offer/public-offer.component';
import { DetailsComponent } from './components/info/details/details.component';
import { DeliveryComponent } from './components/customer/delivery/delivery.component';
import { ReturnComponent } from './components/customer/return/return.component';
import { PaymentMethodComponent } from './components/customer/payment-method/payment-method.component';
import { TranslationComponent } from './components/translation/translation.component';
import { AddTranslationComponent } from './components/dialogs/add-translation/add-translation.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PreOrderComponent,
    PreOrderListComponent,
    IntCurrencyPipe,
    AppFooterComponent,
    LoginComponent,
    AccountComponent,
    SignUpComponent,
    DeliveryPeriodPipe,
    StatusComponent,
    AboutUsComponent,
    PublicOfferComponent,
    DetailsComponent,
    DeliveryComponent,
    ReturnComponent,
    PaymentMethodComponent,
    TranslationComponent,
    AddTranslationComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'pre-orders', component: PreOrderListComponent },
      { path: 'login', component: LoginComponent },
      { path: 'account', component: AccountComponent },
      { path: 'signup', component: SignUpComponent },
      { path: 'about', component: AboutUsComponent },
      { path: 'offer', component: PublicOfferComponent },
      { path: 'details', component: DetailsComponent },
      { path: 'return', component: ReturnComponent },
      { path: 'delivery', component: DeliveryComponent },
      { path: 'payment', component: PaymentMethodComponent },
      { path: 'translation', component: TranslationComponent },
    ]),
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatInputModule,
    MatTreeModule,
    MatIconModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatTableModule,
    MatTooltipModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
