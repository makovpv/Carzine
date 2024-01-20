import { IntCurrencyPipe } from './int-currency.pipe';

describe('CurrencyPipePipe', () => {
  it('create an instance', () => {
    const pipe = new IntCurrencyPipe();
    expect(pipe).toBeTruthy();
  });
});
