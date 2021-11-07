import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BindingModels } from 'src/app/auth/models/binding-models';
import { DevicesService } from 'src/app/separate/services/devices-service ';

@Component({
  selector: 'app-device-comparison',
  templateUrl: './device-comparison.component.html',
  styleUrls: ['./device-comparison.component.css']
})
export class DeviceComparisonComponent implements OnInit {

  devicesList: any[] = [];
  deviceId: string = '';
  device: any;
  device2: any;
  device3: any;
  device4: any;

  constructor(
    public bindingModels: BindingModels,
    private devicesService: DevicesService,
    private activatedRoute: ActivatedRoute
  ) {
    this.getSingleDevice(this.activatedRoute.snapshot.params.id);
  }

  ngOnInit(): void { }

  getAllDevices(id: string) {
    this.devicesService
      .getAllDevices(this.bindingModels.filterModel).subscribe((result: any) => {
        if (result) {
          this.devicesList = result.data;
          this.deviceId = id;
        }
      });
  }

  getSingleDevice(id: string) {
    this.devicesService
      .getSingleDevice(id).subscribe((result: any) => {
        if (result) {
          this.getAllDevices(id);
          this.device = result.data;
        }
      });
  }

  getDeviceToCompare(id: any, condition: number) {
    this.devicesService
      .getSingleDevice(id.value).subscribe((result: any) => {
        if (result) {
          if (condition === 1)
            this.device = result.data;
          else if (condition === 2)
            this.device2 = result.data;
          else if (condition === 3)
            this.device3 = result.data;
          else if (condition === 4)
            this.device4 = result.data;
        }
      });
  }

}