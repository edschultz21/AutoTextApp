import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ObservableResponse {
  observable: Observable<any>;
  apiRequest: string;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  [x: string]: any;
  constructor(private http: HttpClient) { }

  private getParameter(parameter: string, value: string) {
    return parameter + '=' + value;
  }

  private getWebapi(url: string, path: string, params: string, cid: string, apiKey: string): string {
    var webapi = url + '/api/v1/' + path + '?';
    var separator = '';

    if (params) {
      webapi += params + '&';
    }

    if (cid) {
      webapi += separator + this.getParameter('cid', cid);
      separator = '&';
    }
    if (apiKey) {
      webapi += separator + this.getParameter('apiKey', apiKey);
    }

    return webapi;
  }

  httpGetCallback(url: string, path: string, params: string, cid: string, apiKey: string): ObservableResponse {
    var webapiRequest = this.getWebapi(url, path, params, cid, apiKey);

    return <ObservableResponse> { observable: this.http.get<Object>(webapiRequest), apiRequest: webapiRequest };
  }

  httpPostCallback(url: string, path: string, params: string, cid: string, apiKey: string): ObservableResponse {
    var webapiRequest = this.getWebapi(url, path, params, cid, apiKey);

    return <ObservableResponse> { observable: this.http.post<Object>(webapiRequest, null), apiRequest: webapiRequest };
  }

  reloadSegments(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'admin/segment/reload', undefined, cid, apiKey);
  }

  reloadAllSegments(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'admin/segment/reloadall', undefined, undefined, apiKey);
  }

  getMetaData(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpGetCallback(url, 'metadata', undefined, cid, apiKey);
  }

  getConfiguration(url: string, cid: string, apiKey: string, configType: string): ObservableResponse {
    // configType: DATA, ENTITY, SEARCH
    var webapiRequest = this.adminService.getWebapi(url, 'admin/config', this.adminService.getParameter('type', configType), cid, apiKey);

    return <ObservableResponse> { observable: this.adminService.http.get(webapiRequest, { responseType: 'text' }), apiRequest: webapiRequest };
  }

  reloadConfig(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.httpPostCallback(url, 'admin/config/reload', undefined, cid, apiKey);
  }

  private processSearchIndexRequest(url: string, cid: string, apiKey: string, op: string): ObservableResponse {
    return this.httpPostCallback(url, 'admin/segment/search', this.getParameter('op', op), cid, apiKey);
  }

  rebuildSearchIndex(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.processSearchIndexRequest(url, cid, apiKey, 'rebuild');
  }

  reindexSearchIndex(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.processSearchIndexRequest(url, cid, apiKey, 'reindex');
  }

  syncSearchIndex(url: string, cid: string, apiKey: string): ObservableResponse {
    return this.adminService.processSearchIndexRequest(url, cid, apiKey, 'sync');
  }
}
