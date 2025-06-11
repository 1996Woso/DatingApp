import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  /**
   *
   */
  constructor(private http: HttpClient) { 
  }
  // private http = inject(HttpClient);
  apiUrl = 'https://localhost:5001/api/';
  currentUser = signal<User | null>(null);

  Login(model: any){
    return this.http.post<User>(this.apiUrl + 'account/login', model).pipe(
      map(user =>{
        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
        }
      })
    );
  }

  Logout(){
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  Register(model: any){
    return this.http.post<User>(this.apiUrl + 'account/register',model).pipe(
      map(user => {
        if(user){
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  }

}
