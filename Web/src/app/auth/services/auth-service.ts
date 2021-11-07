import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApicallService } from 'src/app/shared/utilities/apicallservice.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private router: Router,
    private apicallService: ApicallService
  ) { }

  getToken() {
    return localStorage.getItem('token');
  }

  login(model: any) {
    return this.apicallService.post('Account/SignIn', model);
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/sign-in');
  }
}
