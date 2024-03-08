import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { PublicOfferComponent } from './components/info/public-offer/public-offer.component';

const routes: Routes = [
  { path: '', component: PublicOfferComponent }
];

@NgModule({
  declarations: [
    PublicOfferComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class MiscModule { }
