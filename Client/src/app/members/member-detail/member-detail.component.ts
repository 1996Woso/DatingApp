import {
  Component,
  computed,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberCardComponent } from '../member-card/member-card.component';
import { LikesService } from '../../_services/likes.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from '../../_services/message.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
import { HeadingComponent } from "../../shared/heading/heading.component";

@Component({
  selector: 'app-member-detail',
  imports: [
    TabsModule,
    GalleryModule,
    TimeagoModule,
    DatePipe,
    MemberCardComponent,
    MemberMessagesComponent,
    HeadingComponent
],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  memberTabs? = viewChild<TabsetComponent>('memberTabs');
  // @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  private memberService = inject(MembersService);
  private likeService = inject(LikesService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  accountService = inject(AccountService);
  private router = inject(Router);
  member: Member = {} as Member;
  id: number = 0;
  hasLiked = computed(() => this.likeService.likeIds().includes(this.id));
  images: GalleryItem[] = [];
  activeTab?: TabDirective;

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
        this.member &&
          this.member.photos.filter(x => x.isApproved === true).map((p) => {
            this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
          });
      },
    });
    //To direct you to the new message when you click new message notification 
    this.route.paramMap.subscribe({
      next: (_) => this.onRouteParamsChange(),
    });
    //This is to open messahe tab when user clicks message button
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs()?.tabs.find(
        (x) => x.heading === heading
      );
      if (messageTab) messageTab.active = true;
    }
  }

  onRouteParamsChange() {
    const user = this.accountService.currentUser();
    if (!user) return;
    if (
      this.messageService.hubConnection?.state ===
        HubConnectionState.Connected &&
      this.activeTab?.heading === 'Messages'
    ) {
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user, this.member.username);
      });
    }
  }

  //To load messages only when messages tab is clicked
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    //Update queryParams when new tab is navigated
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activeTab.heading },
      queryParamsHandling: 'merge',
    });

    if (this.activeTab.heading === 'Messages' && this.member) {
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.username);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
