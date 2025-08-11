import { Component, inject, input, output, ViewChild, viewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-member-messages',
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent {
  // messageForm? = viewChild<NgForm>('messageForm');
  @ViewChild('messageForm') messageForm?: NgForm;
  messageService = inject(MessageService);
  accountService = inject(AccountService);
  username = input.required<string>();
  messageContent = '';
  sendMessage() {
    this.messageService.sendMessage(this.username(), this.messageContent).then(() => {
      this.messageForm?.reset();
    });
  }
}
