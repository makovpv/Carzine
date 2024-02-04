import { EventEmitter, Injectable, Output } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  @Output() carzineMessage: EventEmitter<any> = new EventEmitter();

  constructor() { }

  sendMessage(text: string, duration: number) {
    this.carzineMessage.emit({ text, duration, type: 'message'} );
  }

  sendErrorMessage(text: string) {
    this.carzineMessage.emit({ text, duration: 10000, type: 'error'} );
  }
}
