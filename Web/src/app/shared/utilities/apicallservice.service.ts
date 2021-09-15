import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { catchError, map } from "rxjs/operators";
import { NgxSpinnerService } from "ngx-spinner";
import { throwError } from "rxjs";
import { ToastrService } from "ngx-toastr";

@Injectable({
  providedIn: "root",
})
export class ApicallService {

  constructor(
    private httpClient: HttpClient,
    private toastrService: ToastrService,
    private ngxSpinnerService: NgxSpinnerService
  ) { }

  post(url: string, model: any) {
    this.ngxSpinnerService.show();
    return this.httpClient.post(environment.apiUrl + url, model).pipe(
      map((result: any) => {
        this.ngxSpinnerService.hide();
        if (result.isSuccess)
          return result;
        else
          return null;
      }),
      catchError((err) => {
        this.ngxSpinnerService.hide();
        this.toastrService.error('An error occurred while performing this operation. Please try again later!')
        return throwError(err);
      })
    );
  }

  get(url: string) {
    this.ngxSpinnerService.show();
    return this.httpClient.get(environment.apiUrl + url).pipe(
      map((result: any) => {
        this.ngxSpinnerService.hide();
        if (result.isSuccess)
          return result;
        else
          return null;
      }),
      catchError((err) => {
        this.ngxSpinnerService.hide();
        this.toastrService.error('An error occurred while performing this operation. Please try again later!')
        return throwError(err);
      })
    );
  }
}
