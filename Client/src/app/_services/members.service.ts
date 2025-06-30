import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import {of,tap} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  apiUrl = environment.apiUrl;
  members = signal<Member[]>([]);
  getMembers() {
    return this.http.get<Member[]>(`${this.apiUrl}users`).subscribe({
      next: members => this.members.set(members)
    });
  }
  getMemberByUsername(username: string){
    const member = this.members().find(x => x.username === username);
    if(member !== undefined) return of(member);//returns the member as an observable
    return this.http.get<Member>( `${this.apiUrl}users/${username}`);
  }
  updateMember(member: Member) {
    //Whem updating member, also update members aray
    return this.http.put(this.apiUrl + 'users',member).pipe(
      tap(() => {
        this.members.update(members => members.map(m => m.username === member.username ? member : m))
      })
    );
  }

}
