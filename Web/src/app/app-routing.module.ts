import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guard/auth.guard';

import { SignInComponent } from './auth/pages/sign-in/sign-in.component';
import { CompaniesComponent } from './separate/pages/companies/companies.component';
import { ProductsComponent } from './separate/pages/companies/products/products.component';
import { HomeComponent } from './separate/pages/home/home.component';

const routes: Routes = [

  {
    path: '',
    redirectTo: 'sign-in',
    pathMatch: 'full'
  },
  {
    path: 'sign-in',
    component: SignInComponent,
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'companies',
    canActivate: [AuthGuard],
    component: CompaniesComponent,
  },
  {
    path: 'products',
    canActivate: [AuthGuard],
    component: ProductsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
