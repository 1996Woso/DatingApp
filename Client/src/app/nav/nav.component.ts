import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { TitleCasePipe } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';
@Component({
  selector: 'app-nav',
  imports: [
    FormsModule,
    BsDropdownModule,
    RouterLink,
    TitleCasePipe,
    HasRoleDirective,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  model: any = {};
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  isHamMenuActive: boolean = false;
  currentRoute: string = '';

  constructor() {
    this.router.events.subscribe(() => {
      this.currentRoute = this.router.url;
    });
  }
  toggleHamMenu() {
    this.isHamMenuActive = !this.isHamMenuActive;
  }
  navigateByUrl(url: string) {
    this.isHamMenuActive = false;
    this.router.navigateByUrl(url);
  }
  closeHamMenu() {
    this.isHamMenuActive = false;
  }
  isRouteActive(route: string) {
    return this.currentRoute === route;
  }

  Login() {
    this.accountService.Login(this.model).subscribe({
      next: (response) => {
        this.isHamMenuActive = false;
        this.router.navigateByUrl('/members');
      },

      error: (error) => {
        this.toastr.error(error.error);
      },
      complete: () => {
        this.toastr.success("You've successfully logged in!");
      },
    });
  }

  Logout() {
    this.accountService.Logout();
    this.isHamMenuActive = false;
    this.router.navigateByUrl('/');
    this.toastr.success("You've successfully logged out!");
  }
}
