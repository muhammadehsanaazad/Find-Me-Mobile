import { Component, OnInit } from '@angular/core';
import { CompaniesService } from '../../services/companies-service';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.component.html',
  styleUrls: ['./companies.component.css']
})
export class CompaniesComponent implements OnInit {

  companiesList: any[] = [];

  constructor(
    private companiesService: CompaniesService,
  ) {
    this.getAllCompanies();
  }

  ngOnInit(): void {
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
