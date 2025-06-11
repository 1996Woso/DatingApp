import { Component, EventEmitter, inject, input, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  /**
   * Parent to child communication
   */
  //  @Input({required: true}) usersFromHomeComp: any;
  usersFromHomeComp = input.required<any>();

  /**
   * Child to parent communication
   */
  // @Output() cancelRegister = new EventEmitter<boolean>();
  cancelRegister = output<boolean>();
  /* ****************************************** */
  model: any = {};

  Register(){
    this.accountService.Register(this.model).subscribe({
      next: (response) => {
        console.log(response);
        this.Cancel();
      },
      error: (error) =>{
        console.log(error);
      }
    });
  }

  Cancel(){

    this.cancelRegister.emit(false);
    console.log('Cancelled!');
    
  }
}
