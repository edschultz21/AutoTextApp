import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminService, ObservableResponse } from "./admin.service";

export interface ClientsPermission {
  Name: string;
  Action: string;
  OwningClientId: number;
  Type: string;
}

export interface ClientsResponse {
  ClientId: number;
  ApiKey: string;
  ParentClientId: number;
  ApplicationName: string;
  Permissions: Map<string, number>;
}

@Injectable({ providedIn: 'root' })
export class ClientsService {
  constructor(private http: HttpClient, private adminService: AdminService) { }

  getClients(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'admin/client/getClients', undefined, undefined, apiKey);
  }
}

