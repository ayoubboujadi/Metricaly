import { Injectable } from '@angular/core';
import { AuthClient, RegisterCommand } from '@app/web-api-client';
import { UserRegistration } from '../models/user.registration.interface';

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  constructor(private authClient: AuthClient) { }

  register(user: UserRegistration) {
    const registerCommand = RegisterCommand.fromJS({
      email: user.email,
      name: user.fullName,
      password: user.password
    });

    return this.authClient.signUp(registerCommand);
  }
}
