import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { PublicOfferComponent } from './components/info/public-offer/public-offer.component';
import { ReturnComponent } from './components/customer/return/return.component';
import { AboutUsComponent } from './components/info/about-us/about-us.component';
import { DetailsComponent } from './components/info/details/details.component';
import { DeliveryComponent } from './components/customer/delivery/delivery.component';
import { PaymentMethodComponent } from './components/customer/payment-method/payment-method.component';

const routes: Routes = [
  { path: 'offer', component: PublicOfferComponent },
  { path: 'return', component: ReturnComponent },
  { path: 'about', component: AboutUsComponent },
  { path: 'details', component: DetailsComponent },
  { path: 'delivery', component: DeliveryComponent },
  { path: 'payment', component: PaymentMethodComponent }
];

@NgModule({
  declarations: [
    PublicOfferComponent,
    AboutUsComponent,
    DetailsComponent,
    DeliveryComponent,
    ReturnComponent,
    PaymentMethodComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
     RouterModule
  ]
})
export class InfoModule { }
