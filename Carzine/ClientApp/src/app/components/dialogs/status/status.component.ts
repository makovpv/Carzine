import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { OrderStatusModel } from 'src/app/models/OrderStatusModel';

@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.css'],
  preserveWhitespaces: true
})
export class StatusComponent implements OnInit {
  status: number | undefined;
  statusList: OrderStatusModel[] = [];

  constructor(
    public dialogRef: MatDialogRef<StatusComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
    this.statusList = this.data.statuses;
    this.status = this.data.statusId;
  }

  closeDialog(action: any) {
    this.dialogRef.close({event: action, data: this.status});
  }

}
