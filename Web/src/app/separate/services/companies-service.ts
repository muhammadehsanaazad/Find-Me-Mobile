import { Injectable } from '@angular/core';
import { ApicallService } from 'src/app/shared/utilities/apicallservice.service';

@Injectable({
  providedIn: 'root',
})
export class CompaniesService {
  constructor(
    private apicallService: ApicallService
  ) { }

  addCompany(model: any) {
    return this.apicallService.post('Companies/AddCompany', model);
  }

  updateCompany(model: any) {
    return this.apicallService.post('Companies/UpdateCompany', model);
  }

  getAllCompanies() {
    return this.apicallService.get('Companies/GetAllCompanies');
  }

  getSingleCompany(id: string) {
    return this.apicallService.get('Companies/GetSingleCompany?id=' + id);
  }

  deleteCompany(id: string) {
    return this.apicallService.delete('Companies/DeleteCompany?id=' + id);
  }
}
