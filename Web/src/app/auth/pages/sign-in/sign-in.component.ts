import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthModel } from '../../models/auth-model';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

  constructor(
    public authModel: AuthModel,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (this.authService.getToken() != null)
      this.router.navigate(['/companies']);
    else
      this.authModel.loginBindingModel.reset();
  }

  login() {
    if (this.authModel.loginBindingModel.invalid)
      this.authModel.loginBindingModel.markAllAsTouched();
    else {
      this.authService
        .login(this.authModel.loginBindingModel.value)
        .subscribe((result: any) => {
          if (result) {
            localStorage.setItem('token', result.data.token);
            this.router.navigate(['/companies']);
          }
        });
    }
  }

}
