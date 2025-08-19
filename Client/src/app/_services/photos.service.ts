import { inject, Injectable, signal } from '@angular/core';
import { Photo } from '../_models/photo';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class PhotosService {
  http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Photo[]> | null>(null);

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.apiUrl + 'photos/set-main-photo/' + photo.id, {})
      .pipe();
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.apiUrl + 'photos/delete-photo/' + photo.id)
      .pipe();
  }

  rejectPhoto(photo: Photo) {
    return this.http
      .delete(this.apiUrl + 'photos/reject-photo/' + photo.id)
      .pipe();
  }

  approvePhoto(photo: Photo) {
    return this.http
      .put(`${this.apiUrl}photos/approve-photo/${photo.id}`, {})
      .pipe();
  }

  getUnUprovedPhotos(pageNumber: number, pageSize: number) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    return this.http
      .get<Photo[]>(`${this.apiUrl}photos`, { observe: 'response', params })
      .subscribe({
        next: (response) => setPaginatedResponse(response, this.paginatedResult)
      });
  }
}
