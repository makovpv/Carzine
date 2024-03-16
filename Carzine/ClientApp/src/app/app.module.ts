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
import { TranslationComponent } from './components/translation/translation.component';
import { AddTranslationComponent } from './components/dialogs/add-translation/add-translation.component';
import { AuthGuard } from './auth.guard';
import { RulesComponent } from './components/rules/rules.component';
import { RuleDataComponent } from './components/dialogs/rule-data/rule-data.component';

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
    TranslationComponent,
    AddTranslationComponent,
    RulesComponent,
    RuleDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'search/:code', component: HomeComponent },
      { path: 'pre-orders', component: PreOrderListComponent, canActivate: [AuthGuard] },
      { path: 'login', component: LoginComponent },
      { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
      { path: 'signup', component: SignUpComponent },
      { path: 'info', loadChildren: () => import('./info.module').then(m => m.InfoModule) },
      { path: 'translation', component: TranslationComponent, canActivate: [AuthGuard] },
      { path: 'rules', component: RulesComponent, canActivate: [AuthGuard] },
      { path: '**', redirectTo: ''}
    ]),
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatInputModule,
    MatIconModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatTableModule,
    MatTooltipModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true
    },
    AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
