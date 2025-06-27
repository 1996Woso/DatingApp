import { Component, EventEmitter, inject, input, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

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
  private toastr = inject(ToastrService);
  private router = inject(Router);
  isCancelled:boolean = false;
  Register(){
    this.accountService.Register(this.model).subscribe({
      next: (response) => {
        this.router.navigateByUrl('/members');
        console.log(response);
      },
      complete: () => {
        this.toastr.success('You\'ve successfully registered!')
      }
    });
  }

  Cancel(){
    this.isCancelled = true;
    this.cancelRegister.emit(false);
    console.log('Cancelled!');
    
  }
}
