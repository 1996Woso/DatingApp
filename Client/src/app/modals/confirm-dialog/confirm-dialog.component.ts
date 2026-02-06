import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css',
})
export class ConfirmDialogComponent {
  bsModalRef = inject(BsModalRef);
  title = 'Unsaved Changes';
  message = 'Changes you made may not be saved.';
  btnOkText = 'Ok';
  btnCancelText = 'Cancel';
  result = false;

  confirm() {
    this.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }
}
