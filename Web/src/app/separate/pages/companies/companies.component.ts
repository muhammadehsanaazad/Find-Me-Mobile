import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { CompaniesService } from '../../services/companies-service';
import { DevicesService } from '../../services/devices-service ';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.component.html',
  styleUrls: ['./companies.component.css']
})
export class CompaniesComponent implements OnInit {

  companiesList: any[] = [];

  constructor(
    private companiesService: CompaniesService,
    private toastrService: ToastrService,
    private devicesService: DevicesService
  ) {
    this.getAllCompanies();
  }

  ngOnInit(): void { }

  addDevices() {
    this.devicesService
      .addDevices().subscribe((result: any) => {
        if (result) {
          this.toastrService.success(result.message);
        }
      });
  }

  delete(id: string) {
    if (confirm("Are you sure to delete the company?? Once a company is deleted, you will not be able to recover that company or its devices!")) {
      this.companiesService
        .deleteCompany(id).subscribe((result: any) => {
          if (result) {
            this.getAllCompanies();
            this.toastrService.success(result.message);
          }
        });
    }
  }

  getAllCompanies() {
    this.companiesService
      .getAllCompanies().subscribe((result: any) => {
        if (result) {
          this.companiesList = result.data;
        }
      });
  }

}
