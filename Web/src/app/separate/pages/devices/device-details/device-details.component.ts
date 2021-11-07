import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DevicesService } from 'src/app/separate/services/devices-service ';

@Component({
  selector: 'app-device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.css']
})
export class DeviceDetailsComponent implements OnInit {

  device: any;

  constructor(
    private devicesService: DevicesService,
    private activatedRoute: ActivatedRoute
  ) {
    this.getSingleDevice(this.activatedRoute.snapshot.params.id);
  }

  ngOnInit(): void { }

  getSingleDevice(id: string) {
    this.devicesService
      .getSingleDevice(id).subscribe((result: any) => {
        if (result) {
          this.device = result.data;
        }
      });
  }

}
