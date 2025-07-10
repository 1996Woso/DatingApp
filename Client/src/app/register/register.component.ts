import {
  Component,
  EventEmitter,
  inject,
  input,
  Input,
  OnInit,
  output,
  Output,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { TestErrorsComponent } from "../errors/test-errors/test-errors.component";
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe, NgIf, TextInputComponent, TestErrorsComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
   private accountService = inject(AccountService);
  isCancelled: boolean = false;
  maxDate = new Date();
  validationErrors: string[] | undefined;
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
  registerForm: FormGroup = new FormGroup({});

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }
  //Initialize form using FormGroup
  /**
   * initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(30),
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(30),
      ]),
      confirmPassword: new FormControl('', [
        Validators.required,
        this.matchValues('password'),
      ]),
    });
    //Whenever password input changes, the validity of confirmPassword is updated
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }
   */
  
  //Initialze form using FormBuilder
  initializeForm() {
    this.registerForm =this.fb.group ({
      gender: ['male'],
      username: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(30),
      ]],
      knownAs:['',Validators.required],
      dateOfBirth: ['',Validators.required],
      city: ['',Validators.required],
      country: ['', Validators.required],
      password: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(30),
      ]],
      confirmPassword: ['', [
        Validators.required,
        this.matchValues('password'),
      ]]
    });
    //Whenever password input changes, the validity of confirmPassword is updated
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true }; //return true if values do not match
    };
  }
  Register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth: dob});
    this.accountService.Register(this.registerForm.value).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        this.validationErrors = error;
      },
      complete: () => {
        this.toastr.success('You\'ve successfully registered!');
      }
    });
  }

  Cancel() {
    this.isCancelled = true;
    this.cancelRegister.emit(false);
    console.log('Cancelled!');
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}
