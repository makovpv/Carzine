import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslationModel } from 'src/app/models/TranslationModel';
import { TranslationService } from 'src/app/services/translation.service';
import { AddTranslationComponent } from '../dialogs/add-translation/add-translation.component';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-translation',
  templateUrl: './translation.component.html',
  styleUrls: ['./translation.component.css']
})
export class TranslationComponent implements OnInit {
  translations: TranslationModel[] = [];
  inProgress = false;

  constructor(private translationService: TranslationService, private messageService: MessageService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.inProgress = true;

    this.translationService.getAllTranslation()
    .then(data => {
      this.inProgress = false;
      this.translations = data;
    })
    .catch(err => {
      this.inProgress = false;
      alert(err.message);
    });
  }

  addTranslationClick() {
    this.dialog.open(AddTranslationComponent)
    .afterClosed()
    .subscribe((res) => {
      if (res && res.event === 'ok' && res.data) {
        this.translationService.addTranslation(res.data)
        .then(() => {
          this.messageService.sendMessage('Перевод добавлен', 3000);
          this.translations.push(res.data);
        })
        .catch(err => {
          this.messageService.sendErrorMessage(err.message);
        })
      }
    })
  }

  deleteTranslationClick(key: string) {
    if (confirm("Удалить перевод?")) {
      this.inProgress = true;
      
      this.translationService.deleteTranslation(key).then(() => {
        this.inProgress = false;
        this.translations = this.translations.filter(x => x.enName !== key);
      });
    }
  }

}
