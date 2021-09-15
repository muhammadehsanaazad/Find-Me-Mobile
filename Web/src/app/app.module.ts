import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpResponseInterceptor } from './shared/utilities/http.interceptor';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { SignInComponent } from './auth/pages/sign-in/sign-in.component';
import { HomeComponent } from './separate/pages/home/home.component';
import { CompaniesComponent } from './separate/pages/companies/companies.component';
import { ToastrModule } from 'ngx-toastr';
import { ProductsComponent } from './separate/pages/companies/products/products.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    HomeComponent,
    CompaniesComponent,
    ProductsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserModule,
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
