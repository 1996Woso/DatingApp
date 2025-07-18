import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/user-params';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  apiUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserPararms() {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers() {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));
    if (response) return this.setPaginatedResponse(response);
    console.log(Object.values(this.userParams()).join('-'));
    let params = this.setPaginationHeaders(
      this.userParams().pageNumber,
      this.userParams().pageSize
    );

    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);
    return this.http
      .get<Member[]>(`${this.apiUrl}users`, { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.memberCache.set(Object.values(this.userParams()).join('-'),response);
        },
      });
  }

  private setPaginatedResponse(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    if (pageNumber && pageSize) {
      params = params.append('pageSize', pageSize);
      params = params.append('pageNumber', pageNumber);
    }
    return params;
  }

  getMemberByUsername(username: string) {
    const member: Member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((m: Member)=> m.username === username);

    if (member) return of(member);//return member as an observable
    return this.http.get<Member>(`${this.apiUrl}users/${username}`);
  }

  updateMember(member: Member) {
    //Whem updating member, also update members array
    return this.http
      .put(this.apiUrl + 'users', member)
      .pipe
      ();
  }
  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.apiUrl + 'users/set-main-photo/' + photo.id, {})
      .pipe();
  }
  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.apiUrl + 'users/delete-photo/' + photo.id)
      .pipe
      ();
  }
}
