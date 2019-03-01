import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AdminService, ObservableResponse } from "./admin.service";

@Injectable({ providedIn: 'root' })
export class SegmentSearchService {
  constructor(private http: HttpClient, private adminService: AdminService) { }

  searchSegments(url: string, cid: string, apiKey: string, query: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'segment/search', query, cid, apiKey);
  }
}

