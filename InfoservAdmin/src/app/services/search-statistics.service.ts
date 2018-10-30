import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminService, ObservableResponse } from "./admin.service";

export interface SearchStatistics
{
  Index: string;
  Aliases: string[];
  CreationDate: string;
  IsActive: string;
  DocsCount: string;
  Health: string;
  Status: string;
  StoreSize: string;
  TotalMemory: string;
}

@Injectable({ providedIn: 'root' })
export class SearchStatisticsService {
  constructor(private http: HttpClient, private adminService: AdminService) { }

  getSearchIndexStatistics(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'admin/segment/searchstats', undefined, cid, apiKey);
  }
}
