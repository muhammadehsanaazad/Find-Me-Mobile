import { Injectable } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class BindingModels {

  filterModel = {
    company: '',
    operatingSystem: '',
    priceFrom: '',
    priceTo: '',
    model: '',
    ram: '',
    rom: '',
    screenType: '',
    battery: '',
    category: ''
  };

}
