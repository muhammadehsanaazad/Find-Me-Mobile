import { Injectable } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class AuthModel {
  constructor(private formBuilder: FormBuilder) { }

  loginBindingModel = this.formBuilder.group({
    email: [
      '',
      [
        Validators.required,
        Validators.pattern('[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+.[A-Za-z]{1,63}$'),
      ],
    ],
    password: [
      '',
      [
        Validators.required,
      ],
    ],
  });
}
