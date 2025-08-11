import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  constructor(private http: HttpClient) {}
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  apiUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  defaultImage: string = "https://res.cloudinary.com/dfaqqc2ge/image/upload/v1752435852/user_ol7be4.png";
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      const role = JSON.parse(atob(user.token.split('.')[1])).role;
      return Array.isArray(role) ? role : [role];
    }
    return [];
  });

  Login(model: any) {
    return this.http.post<User>(this.apiUrl + 'account/login', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return null;
      })
    );
  }

  Logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.presenceService.stopHubConnection();
  }

  Register(model: any) {
    return this.http.post<User>(this.apiUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    //Get like ids
    this.likeService.getLikeIds();
    this.presenceService.createHubConnection(user);
  }
}
 