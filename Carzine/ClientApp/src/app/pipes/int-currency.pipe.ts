import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'intCurrency'
})
export class IntCurrencyPipe implements PipeTransform {

  transform(value: number | undefined, ...args: unknown[]): unknown {
    return Math.round(value ?? 0).toLocaleString('fr');
  }

}
