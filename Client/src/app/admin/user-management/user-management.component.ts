import { Component, computed, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { PageChangedEvent, PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-user-management',
  imports: [FormsModule, PaginationModule, ButtonsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css',
})
export class UserManagementComponent implements OnInit {
  adminService = inject(AdminService);
  private modalService = inject(BsModalService);
  private toastr = inject(ToastrService);
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();

  ngOnInit(): void {
    if (!this.adminService.paginatedResult()) this.getUsersWithRoles();
  }

  openRolesModal(user: User) {
    const initialState: ModalOptions = {
      class: 'modal-lg',
      initialState: {
        title: `${user.username} roles`,
        username: user.username,
        selectedRoles: [...user.roles],
        availableRoles: ['Admin', 'Moderator', 'Member'],
        users: this.users,
        rolesUpdated: false,
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated) {
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService
            .updateUserRoles(user.username, selectedRoles)
            .subscribe({
              next: (roles) => (user.roles = roles),
              // error: (error) => {
              //   this.toastr.error(error.error);
              // },
              // complete: () => {
              //   this.toastr.success('Roles has been successfully updated');
              // },
            });
        }
      },
    });
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles();
  }

  pageChanged(event: any) {
    if (this.adminService.adminParams().pageNumber != event.page) {
      this.adminService.adminParams().pageNumber = event.page;
      this.getUsersWithRoles();
    }
  }
  resetFilters() {
    this.adminService.resetAdminParams();
    this.getUsersWithRoles();
  }
}
