import {
  Component,
  HostListener,
  inject,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";
import { DatePipe } from '@angular/common';
import { TimeagoModule } from 'ngx-timeago';
import { MemberCardComponent } from "../member-card/member-card.component";
import { HeadingComponent } from "../../shared/heading/heading.component";

@Component({
  selector: 'app-member-edit',
  imports: [TabsModule, FormsModule, PhotoEditorComponent, DatePipe, TimeagoModule, MemberCardComponent, HeadingComponent],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  member?: Member;
  accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);
  //@ViewChild('editForm') editForm?: NgForm;
  editForm = viewChild<NgForm>('editForm');

  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm()?.dirty) {

      $event.returnValue = true;
    }
  }

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMemberByUsername(user.username).subscribe({
      next: (member) => (this.member = member),
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm()?.value).subscribe({
      next: _ => {
        this.editForm()?.reset(this.member);
        this.toastr.success('Profile updated successfully');
      },
    });
  }

  onMemberChange(e: Member) {
    this.member = e;
  }
}
