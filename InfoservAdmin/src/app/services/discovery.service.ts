import { Injectable } from '@angular/core';
import { AdminService, ObservableResponse } from "./admin.service";

export interface DiscoverySource {
  Code: string;
  ConfigUid: string;
  Description: string;
  LastModifiedDate: string;
  Status: string;
}

export interface DiscoveryResponse {
  Sources: DiscoverySource[];
  Version: string;
}

@Injectable({ providedIn: 'root' })
export class DiscoveryService {
  constructor(private adminService: AdminService) { }

  getDiscovery(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'discovery', undefined, undefined, apiKey);
  }
}

