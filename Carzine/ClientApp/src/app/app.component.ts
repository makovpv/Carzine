import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MessageService } from './services/message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'Zild';

  constructor(private messageService: MessageService, private _snackBar: MatSnackBar) {
    this.messageService.carzineMessage.subscribe(data => {
      this.showSnack(data.text, data.duration);
    });
  }

  showSnack(message: string, duration: number) {
    this._snackBar.open(message, "OK", {
      duration: duration,
      panelClass: ['cz-snack-panel'],
      verticalPosition: 'top'
    });
  }
}
