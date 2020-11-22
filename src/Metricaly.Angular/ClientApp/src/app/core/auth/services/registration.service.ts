import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { UserRegistration } from '../models/user.registration.interface';

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  constructor(private http: HttpClient) { }

  register(user: UserRegistration) {
    return this.http.post(`https://localhost:44344/api/auth/signup`, user);
    //return this.http.post(`${config.apiUrl}/users/register`, user);
  }

}
