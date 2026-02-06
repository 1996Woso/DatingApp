import { Component } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagementComponent } from "../user-management/user-management.component";
import { PhotoManagementComponent } from "../photo-management/photo-management.component";
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { HeadingComponent } from "../../shared/heading/heading.component";

@Component({
  selector: 'app-admin-panel',
  imports: [TabsModule, UserManagementComponent, PhotoManagementComponent, HasRoleDirective, HeadingComponent],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {

}
