import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/auth/services/auth-service';
import { CompaniesService } from '../../services/companies-service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit {

  companiesList: any[] = [];
  token: any;

  constructor(
    public authService: AuthService,
    private companiesService: CompaniesService,
  ) {
    this.getAllCompanies();
    this.token = localStorage.getItem("token");
  }

  ngOnInit(): void { }

  getAllCompanies() {
    this.companiesService
      .getAllCompanies().subscribe((result: any) => {
        if (result) {
          this.companiesList = result.data;
        }
      });
  }
}
