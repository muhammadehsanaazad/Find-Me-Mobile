import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/services/auth-service';

@Injectable()
export class HttpResponseInterceptor implements HttpInterceptor {
  constructor(
    private toastrService: ToastrService,
    private authService: AuthService) { }
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // add a custom header
    let customReq;
    const token = localStorage.getItem('token')
      ? localStorage.getItem('token')
      : '';
    const lastheader = request.headers.has('isFile');
    if (lastheader) {
      customReq = request.clone({
        headers: request.headers.set('Authorization', 'Bearer ' + token),
      });
    } else {
      customReq = request.clone({
        headers: request.headers
          .append('Content-Type', 'application/json')
          .append('Authorization', 'Bearer ' + token),
      });
    }
    // pass on the modified request object
    return next.handle(customReq).pipe(
      tap((ev: HttpEvent<any>) => {
        if (ev instanceof HttpResponse) {
        }
      }),
      catchError((response) => {
        if ([403].indexOf(response.status) !== -1) {
          //  403 Forbidden response returned from api
          this.toastrService.error('You are Not Allowed to perform this Action!');
        } else if ([401].indexOf(response.status) !== -1) {
          //auto logout if 401 Unauthorized
          this.authService.logout();
        }
        const error = response.error?.message || response.statusText;
        return throwError(error);
      })
    );
  }
}
