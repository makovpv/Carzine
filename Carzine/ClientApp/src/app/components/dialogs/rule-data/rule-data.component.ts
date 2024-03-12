import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { RuleRangeModel } from 'src/app/models/RuleRangeModel';

@Component({
  selector: 'app-rule-data',
  templateUrl: './rule-data.component.html',
  styleUrls: ['./rule-data.component.css'],
  preserveWhitespaces: true
})
export class RuleDataComponent implements OnInit {
  ruleRange: RuleRangeModel = { id: null, max: null, min: null, type: undefined, value: undefined };

  constructor(public dialogRef: MatDialogRef<RuleDataComponent>) {
  }

  ngOnInit(): void {
  }

  closeDialog(action: any) {
    this.dialogRef.close({event: action, data: this.ruleRange});
  }

}
