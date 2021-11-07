import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guard/auth.guard';

import { SignInComponent } from './auth/pages/sign-in/sign-in.component';
import { AddCompanyComponent } from './separate/pages/companies/add-company/add-company.component';
import { CompaniesComponent } from './separate/pages/companies/companies.component';
import { EditCompanyComponent } from './separate/pages/companies/edit-company/edit-company.component';
import { DeviceComparisonComponent } from './separate/pages/devices/device-details/device-comparison/device-comparison.component';
import { DeviceDetailsComponent } from './separate/pages/devices/device-details/device-details.component';
import { DevicesComponent } from './separate/pages/devices/devices.component';
import { HomeComponent } from './separate/pages/home/home.component';

const routes: Routes = [

  {
    path: '',
    redirectTo: 'devices/random',
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
    path: 'devices/:id',
    component: DevicesComponent,
  },
  {
    path: 'device-details/:id',
    component: DeviceDetailsComponent,
  },
  {
    path: 'device-comparison/:id',
    component: DeviceComparisonComponent,
  },
  {
    path: 'companies',
    canActivate: [AuthGuard],
    component: CompaniesComponent,
  },
  {
    path: 'add-company',
    canActivate: [AuthGuard],
    component: AddCompanyComponent,
  },
  {
    path: 'edit-company/:id',
    canActivate: [AuthGuard],
    component: EditCompanyComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
