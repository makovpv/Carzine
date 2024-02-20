import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslationModel } from 'src/app/models/TranslationModel';

@Component({
  selector: 'app-add-translation',
  templateUrl: './add-translation.component.html',
  styleUrls: ['./add-translation.component.css'],
  preserveWhitespaces: true
})
export class AddTranslationComponent implements OnInit {
  translation: TranslationModel = { enName: '', ruName: '' };

  constructor(public dialogRef: MatDialogRef<AddTranslationComponent>) { 
  }

  ngOnInit(): void {
  }

  closeDialog(action: any) {
    this.dialogRef.close({event: action, data: this.translation});
  }

}
