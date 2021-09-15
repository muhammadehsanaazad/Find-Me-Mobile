import { Injectable } from '@angular/core';
import { ApicallService } from 'src/app/shared/utilities/apicallservice.service';

@Injectable({
  providedIn: 'root',
})
export class CompaniesService {
  constructor(
    private apicallService: ApicallService
  ) { }

  getAllCompanies() {
    return this.apicallService.get('Companies/GetAllCompanies');
  }
}
