import { Injectable } from '@angular/core';
import { ApicallService } from 'src/app/shared/utilities/apicallservice.service';

@Injectable({
  providedIn: 'root',
})
export class DevicesService {

  constructor(
    private apicallService: ApicallService
  ) { }

  addDevices() {
    return this.apicallService.get('Devices/AddDevices');
  }

  getAllDevices(model: any) {
    return this.apicallService.post('Devices/GetAllDevices', model);
  }

  getSingleDevice(id: string) {
    return this.apicallService.get('Devices/GetSingleDevice?id=' + id);
  }

  getFilters() {
    return this.apicallService.get('Devices/GetFilters');
  }

}
