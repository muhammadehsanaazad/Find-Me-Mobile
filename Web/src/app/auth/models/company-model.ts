import { Injectable } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class CompanyModel {
  constructor(private formBuilder: FormBuilder) { }

  addOrUpdateCompanyModel = this.formBuilder.group({
    id: '',
    name: ['', Validators.required],
    contactNumber: '',
  });
}
