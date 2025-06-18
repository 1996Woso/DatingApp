import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { NgIf, TitleCasePipe } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-nav',
  imports: [FormsModule, NgIf, BsDropdownModule, RouterLink, RouterLinkActive,TitleCasePipe],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  model: any = {};
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  Login() {
    this.accountService.Login(this.model).subscribe({
      next: (response) => {
        this.router.navigateByUrl('/members')
        console.log(response);
      },
      error: (error) => {
        this.toastr.error(error.error);
        console.log(error);
      },
      complete: () => {
        this.toastr.success('You\'ve successfully logged in!');
      }
    });
  }

  Logout() {
    this.accountService.Logout();
    this.router.navigateByUrl('/');
    this.toastr.success('You\'ve successfully logged out!');
  }
}
