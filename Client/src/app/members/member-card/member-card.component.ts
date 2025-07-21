import { Component, computed, inject, input, ViewEncapsulation } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../_services/account.service';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
  // encapsulation: ViewEncapsulation.None//This allows the style of this comp to be global
})
export class MemberCardComponent {
  private likeService = inject(LikesService);
  member = input.required<Member>();
  accountService = inject(AccountService);
  showButtons = input<boolean>(false);

  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id));

  toggleLike() {
    this.likeService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likeService.likeIds.update(ids => ids.filter(x => x !== this.member().id));
        }else{
          this.likeService.likeIds.update(ids => [...ids, this.member().id]);
        }
      }
    })
  }
}
