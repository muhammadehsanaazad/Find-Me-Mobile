import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
  ) { }

  canActivate(_route: ActivatedRouteSnapshot, _state: RouterStateSnapshot) {
    const token = localStorage.getItem('token')
    if (token)
      // authorised so return true
      return true;
    else {
      // not authorised so return false
      this.router.navigate(['sign-in']);
      return false;
    }
  }
}
