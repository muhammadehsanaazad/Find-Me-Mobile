import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CompanyModel } from 'src/app/auth/models/company-model';
import { CompaniesService } from 'src/app/separate/services/companies-service';

@Component({
  selector: 'app-add-company',
  templateUrl: './add-company.component.html',
  styleUrls: ['./add-company.component.css']
})
export class AddCompanyComponent implements OnInit {

  constructor(
    public companyModel: CompanyModel,
    private companiesService: CompaniesService,
    private toastrService: ToastrService,
    private router: Router) {
    this.companyModel.addOrUpdateCompanyModel.reset();
  }

  ngOnInit(): void { }


  addCompany() {
    if (this.companyModel.addOrUpdateCompanyModel.invalid)
      this.companyModel.addOrUpdateCompanyModel.markAllAsTouched();
    else {
      this.companiesService
        .addCompany(this.companyModel.addOrUpdateCompanyModel.value)
        .subscribe((result: any) => {
          if (result) {
            this.toastrService.success(result.message);
            this.router.navigate(['/companies']);
          }
        });
    }
  }
}
