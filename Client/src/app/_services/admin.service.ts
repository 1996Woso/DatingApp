import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';
import { AdminParams } from '../_models/admin-params';
import { map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  apiUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<User[]> | null>(null);
  memberCache = new Map();
  adminParams = signal<AdminParams>(new AdminParams());

  resetAdminParams() {
    this.adminParams.set(new AdminParams());
  }

  getUserWithRoles(): Observable<PaginatedResult<User[]>> {

     const cacheKey = Object.values(this.adminParams()).join('-');
    const cached = this.memberCache.get(cacheKey);
    if (cached) {
      return of(cached);
    }
    let params = setPaginationHeaders(
      this.adminParams().pageNumber,
      this.adminParams().pageSize
    );

    params = params.append('username',  this.adminParams().username);

   return this.http
      .get<User[]>(`${this.apiUrl}admin/users-with-roles`, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          const paginated: PaginatedResult<User[]> = {
            items: response.body!,
            pagination: JSON.parse(response.headers.get('Pagination')!),
          };
          this.memberCache.set(cacheKey, paginated);
          return paginated;
        })
      );
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(`${this.apiUrl}admin/edit-roles/${username}?roles=${roles}`,{});
  }
}
