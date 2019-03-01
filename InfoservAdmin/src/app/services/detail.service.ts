import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AdminService, ObservableResponse } from "./admin.service";

@Injectable({ providedIn: 'root' })
export class DetailService {
  constructor(private http: HttpClient, private adminService: AdminService) { }

  getDetails(url: string, cid: string, apiKey: string, query: string): ObservableResponse {
    return this.adminService.httpPostCallback(url, 'detail/query', undefined, cid, apiKey, query);
  }
}

