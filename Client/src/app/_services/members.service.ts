import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  apiUrl = environment.apiUrl;

  getMembers() {
    return this.http.get<Member[]>(`${this.apiUrl}users`);
  }
  getMemberByUsername(username: string){
    return this.http.get<Member>( `${this.apiUrl}users/${username}`);
  }

}
