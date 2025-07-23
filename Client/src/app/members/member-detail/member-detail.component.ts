import { Component, computed, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import {GalleryItem, GalleryModule, ImageItem} from 'ng-gallery'
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberCardComponent } from "../member-card/member-card.component";
import { LikesService } from '../../_services/likes.service';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberCardComponent, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit{
   memberTabs? = viewChild<TabsetComponent>('memberTabs');
  // @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  private memberService = inject(MembersService);
  private likeService = inject(LikesService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  member: Member = {} as Member;
  id:number = 0;
  hasLiked = computed(() => this.likeService.likeIds().includes(this.id));
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
         this.member && this.member.photos.map(p => {
          this.images.push(new ImageItem({src: p.url, thumb: p.url}))
        })
      }
    })
      //This is to open messahe tab when user clicks message button
      this.route.queryParams.subscribe({
        next: params => {
          params['tab'] && this.selectTab(params['tab'])
        }
      })
  }

  onUpdateMessages(event: Message) {
    this.messages.push(event);
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs()?.tabs.find(x => x.heading === heading);
      if (messageTab) messageTab.active = true;
    }
  }

  //To load messages only when messages tab is clicked
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member) {
        this.messageService.getMessageThread(this.member.username).subscribe({
      next: messages => this.messages = messages
    });
    }
  }

  // loadMember() {
  //   const username = this.route.snapshot.paramMap.get("username");
  //   if (!username) return;
  //   this.memberService.getMemberByUsername(username).subscribe({
  //     next: member => {
  //       this.member = member;
  //       this.id = this.member.id;
  //       member.photos.map(p => {
  //         this.images.push(new ImageItem({src: p.url, thumb: p.url}))
  //       })
  //     }
  //   })
  // }

}
