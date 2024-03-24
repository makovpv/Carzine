export class OrderModel {
    id: number | undefined;
    phone: string | undefined;
    user_email: string | undefined;
    date: Date | undefined;
    client_status_id: number | undefined;
    clientStatusName: string = '';
    payment_Order_State: string = '';
    total_Sum: number = 0;
}