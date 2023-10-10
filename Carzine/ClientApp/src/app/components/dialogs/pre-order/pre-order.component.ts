import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-pre-order',
  templateUrl: './pre-order.component.html',
  styleUrls: ['./pre-order.component.css'],
  preserveWhitespaces: true
})
export class PreOrderComponent implements OnInit {
  phoneNumber = "";

  constructor(
    public dialogRef: MatDialogRef<PreOrderComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
  }

  closeDialog(action: any) {
    this.dialogRef.close({event: action, data: this.phoneNumber});
  }
}
