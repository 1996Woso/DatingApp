import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  /**
   *
   */
  constructor(private http: HttpClient) {}
  // private http = inject(HttpClient);
  private likeService = inject(LikesService);
  apiUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  defaultImage: string = "https://res.cloudinary.com/dfaqqc2ge/image/upload/v1752435852/user_ol7be4.png";

  Login(model: any) {
    return this.http.post<User>(this.apiUrl + 'account/login', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  Logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
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
  }
}
