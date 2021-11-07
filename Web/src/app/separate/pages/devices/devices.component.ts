import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BindingModels } from 'src/app/auth/models/binding-models';
import { DevicesService } from '../../services/devices-service ';

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.css']
})
export class DevicesComponent implements OnInit {

  devicesList: any[] = [];

  filtersList: any = {
    company: [],
    operatingSystem: [],
    model: [],
    ram: [],
    rom: [],
    screenType: [],
    battery: [],
    category: [],

  };

  constructor(
    public bindingModels: BindingModels,
    private devicesService: DevicesService,
    private activatedRoute: ActivatedRoute
  ) {
    this.getFilters();
    this.activatedRoute.params.subscribe(param => {
      let id = param.id;
      if (id) {
        if (id === 'random')
          this.bindingModels.filterModel.company = '';
        else
          this.bindingModels.filterModel.company = id;
        this.getAllDevices();
      }
      else this.getAllDevices();
    })

  }

  ngOnInit(): void { }

  getAllDevices() {
    this.devicesService
      .getAllDevices(this.bindingModels.filterModel).subscribe((result: any) => {
        if (result) {
          this.devicesList = result.data;
        }
      });
  }

  getFilters() {
    this.devicesService
      .getFilters().subscribe((result: any) => {
        if (result) {
          this.filtersList = result.data;
        }
      });
  }

  applyFilters() {
    this.devicesService
      .getFilters().subscribe((result: any) => {
        if (result) {
          this.filtersList = result.data;
        }
      });
  }

}
