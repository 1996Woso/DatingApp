import { Component } from '@angular/core';
import { RegisterComponent } from '../register/register.component';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-home',
  imports: [RegisterComponent, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  registerMode = false;
  users: any;
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }
}
