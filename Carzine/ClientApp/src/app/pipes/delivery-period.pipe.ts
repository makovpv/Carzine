import { Pipe, PipeTransform } from '@angular/core';
import { DeliveryInfo } from '../models/ProductModel';

@Pipe({
  name: 'deliveryPeriod'
})
export class DeliveryPeriodPipe implements PipeTransform {

  transform(value: DeliveryInfo, ...args: unknown[]): string | number {
    if (value.deliveryMin === value.deliveryMax)
      return value.deliveryMax ?? '';
    
    return `${value.deliveryMin} - ${value.deliveryMax}`;
  }

}
