import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AdminService, ObservableResponse } from "./admin.service";

@Injectable({ providedIn: 'root' })
export class DataService {
  constructor(private http: HttpClient, private adminService: AdminService) { }

  getData(url: string, cid: string, apiKey: string, query: string): ObservableResponse {
    return this.adminService.httpPostCallback(url, 'data', 'ReturnResults=ALL_AT_ONCE', cid, apiKey, query);
  }
}

