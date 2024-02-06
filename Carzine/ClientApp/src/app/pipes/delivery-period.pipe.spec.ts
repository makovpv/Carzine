import { DeliveryPeriodPipe } from './delivery-period.pipe';

describe('DeliveryPeriodPipe', () => {
  it('create an instance', () => {
    const pipe = new DeliveryPeriodPipe();
    expect(pipe).toBeTruthy();
  });

  it('check interval', () => {
    const res = new DeliveryPeriodPipe().transform({deliveryMin: 10, deliveryMax: 20});
    expect(res).toEqual('10 - 20');
  });

  it('check equal interval', () => {
    const res = new DeliveryPeriodPipe().transform({deliveryMin: 51, deliveryMax: 51});
    expect(res).toEqual(51);
  });
});
