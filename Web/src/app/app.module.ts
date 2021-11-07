import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ToastrModule } from 'ngx-toastr';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpResponseInterceptor } from './shared/utilities/http.interceptor';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { SignInComponent } from './auth/pages/sign-in/sign-in.component';
import { HomeComponent } from './separate/pages/home/home.component';
import { CompaniesComponent } from './separate/pages/companies/companies.component';
import { ProductsComponent } from './separate/pages/companies/products/products.component';
import { DevicesComponent } from './separate/pages/devices/devices.component';
import { DeviceDetailsComponent } from './separate/pages/devices/device-details/device-details.component';
import { DeviceComparisonComponent } from './separate/pages/devices/device-details/device-comparison/device-comparison.component';
import { NavBarComponent } from './separate/pages/nav-bar/nav-bar.component';
import { AddCompanyComponent } from './separate/pages/companies/add-company/add-company.component';
import { EditCompanyComponent } from './separate/pages/companies/edit-company/edit-company.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    HomeComponent,
    CompaniesComponent,
    ProductsComponent,
    DevicesComponent,
    DeviceDetailsComponent,
    DeviceComparisonComponent,
    NavBarComponent,
    AddCompanyComponent,
    EditCompanyComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    ToastrModule.forRoot(),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpResponseInterceptor,
      multi: true,
    },
    NgxSpinnerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
