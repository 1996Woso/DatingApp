import { Component, input, ViewEncapsulation } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
  // encapsulation: ViewEncapsulation.None//This allows the style of this comp to be global
})
export class MemberCardComponent {
  member = input.required<Member>();
}
