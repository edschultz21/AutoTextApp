import { Injectable } from '@angular/core';
import { AdminService, ObservableResponse } from "./admin.service";
import { ClientsService } from "./clients.service";
import { DiscoveryService } from "./discovery.service";
import { SearchStatisticsService } from "./search-statistics.service";
import { DataService } from "./data.service";
import { DetailService } from "./detail.service";
import { SegmentSearchService } from "./segment-search.service";
import * as convertXml from 'xml-js';
import * as formatXml from 'xml-formatter';

@Injectable({ providedIn: 'root' })
export class InfoservService {
  constructor(
    private adminService: AdminService,
    private clientsService: ClientsService,
    private discoveryService: DiscoveryService,
    private searchStatisticsService: SearchStatisticsService,
    private dataService: DataService,
    private detailService: DetailService,
    private segmentSearchService: SegmentSearchService
  ) { }

  private convertXmlToJson(xml: string): string {
    return convertXml.xml2json(xml, { compact: true, spaces: 4, ignoreComment: true })
  }

  private outputResults(result: any): object {
    return { stringResult: JSON.stringify(result, null, 4), rawResults: result };
  }

  private outputXmlResults(result: any): object {
    return { stringResult: formatXml(result, { indentation: '      ' }), rawResults: result };
  }

  private outputJsonResults(result: any): object {
    return { stringResult: JSON.stringify(result, null, 4), rawResults: result };
  }

  private outputXmlAsJson(result: any): object {
    return { stringResult: this.convertXmlToJson(result), rawResults: result };
  }

  public getConfigurationData(callback: Function): void {
    callback(this.adminService.getConfiguration.bind(this), this.outputXmlResults.bind(this), 'DATA');
  }

  public getConfigurationEntity(callback: Function): void {
    callback(this.adminService.getConfiguration.bind(this), this.outputXmlResults.bind(this), 'ENTITY');
  }

  public getConfigurationSearch(callback: Function): void {
    callback(this.adminService.getConfiguration.bind(this), this.outputXmlAsJson.bind(this), 'SEARCH');
  }

  public reloadConfig(callback: Function): void {
    callback(this.adminService.reloadConfig.bind(this), this.outputResults.bind(this));
  }

  public reloadSegments(callback: Function): void {
    callback(this.adminService.reloadSegments.bind(this), this.outputResults.bind(this));
  }

  public reloadAllSegments(callback: Function): void {
    callback(this.adminService.reloadAllSegments.bind(this), this.outputResults.bind(this));
  }

  public getSearchIndexStatistics(callback: Function): void {
    callback(this.searchStatisticsService.getSearchIndexStatistics.bind(this), this.outputResults.bind(this));
  }

  public rebuildSearchIndex(callback: Function): void {
    callback(this.adminService.rebuildSearchIndex.bind(this), this.outputResults.bind(this));
  }

  public reindexSearchIndex(callback: Function): void {
    callback(this.adminService.reindexSearchIndex.bind(this), this.outputResults.bind(this));
  }

  public syncSearchIndex(callback: Function): void {
    callback(this.adminService.syncSearchIndex.bind(this), this.outputResults.bind(this));
  }

  public getDiscovery(callback: Function): void {
    callback(this.discoveryService.getDiscovery.bind(this), this.outputResults.bind(this));
  }

  public getMetaData(callback: Function): void {
    callback(this.adminService.getMetaData.bind(this), this.outputResults.bind(this));
  }

  public getClients(callback: Function): void {
    callback(this.clientsService.getClients.bind(this), this.outputResults.bind(this));
  }

  // Direct calls (wrappers)
  public getDiscoveryWrapper(url: string, apiKey: string): ObservableResponse {
    return this.discoveryService.getDiscovery(url, undefined, apiKey);
  }

  public getClientsWrapper(url: string, apiKey: string): ObservableResponse {
    return this.clientsService.getClients(url, undefined, apiKey);
  }

  public getData(callback: Function): void {
    callback(this.dataService.getData.bind(this), this.outputResults.bind(this), 'POST');
  }

  public getDetails(callback: Function): void {
    callback(this.detailService.getDetails.bind(this), this.outputResults.bind(this), 'POST');
  }

  public searchSegments(callback: Function): void {
    callback(this.segmentSearchService.searchSegments.bind(this), this.outputResults.bind(this), 'POST');
  }
}

