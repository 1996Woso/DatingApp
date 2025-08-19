import { Component, inject, OnInit } from '@angular/core';
import { PhotosService } from '../../_services/photos.service';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { Photo } from '../../_models/photo';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-photo-management',
  imports: [PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent implements OnInit {
  photoService = inject(PhotosService);
  accountService = inject(AccountService);
  member?: Member;
  pageSize = 5;
  pageNumber = 1;
  
  ngOnInit(): void {
    if (!this.photoService.paginatedResult()) this.loadPhotos();
  }

  approvePhoto(photo: Photo) {
    this.photoService.approvePhoto(photo).subscribe({
      next: (_) => {
        this.loadPhotos();
      },
    });
  }

  rejectPhoto(photo: Photo) {
    this.photoService.rejectPhoto(photo).subscribe({
      next: _ => {
        this.loadPhotos();
      }
    });
  }

  loadPhotos() {
    this.photoService.getUnUprovedPhotos(this.pageNumber, this.pageSize);
  }

  pageChanged(event: any) {
    if (this.pageNumber != event.page) {
      this.pageNumber = event.page;
      this.loadPhotos();
    }
  }
}
