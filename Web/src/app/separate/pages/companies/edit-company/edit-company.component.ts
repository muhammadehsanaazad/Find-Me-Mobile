import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CompanyModel } from 'src/app/auth/models/company-model';
import { CompaniesService } from 'src/app/separate/services/companies-service';

@Component({
  selector: 'app-edit-company',
  templateUrl: './edit-company.component.html',
  styleUrls: ['./edit-company.component.css']
})
export class EditCompanyComponent implements OnInit {


  constructor(
    public companyModel: CompanyModel,
    private companiesService: CompaniesService,
    private toastrService: ToastrService,
    private activatedRoute: ActivatedRoute,
    private router: Router) {
    this.companyModel.addOrUpdateCompanyModel.reset();
    this.getSingleCompany(this.activatedRoute.snapshot.params.id)
  }

  ngOnInit(): void { }

  getSingleCompany(id: string) {
    this.companiesService
      .getSingleCompany(id)
      .subscribe((result: any) => {
        if (result) {
          this.companyModel.addOrUpdateCompanyModel.controls.id.setValue(result.data.id);
          this.companyModel.addOrUpdateCompanyModel.controls.name.setValue(result.data.name);
          this.companyModel.addOrUpdateCompanyModel.controls.contactNumber.setValue(result.data.contactNumber);
        }
      });
  }

  updateCompany() {
    if (this.companyModel.addOrUpdateCompanyModel.invalid)
      this.companyModel.addOrUpdateCompanyModel.markAllAsTouched();
    else {
      this.companiesService
        .updateCompany(this.companyModel.addOrUpdateCompanyModel.value)
        .subscribe((result: any) => {
          if (result) {
            this.toastrService.success(result.message);
            this.router.navigate(['/companies']);
          }
        });
    }
  }
}
