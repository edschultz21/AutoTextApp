import { Component, OnDestroy, Testability } from '@angular/core';
import { DiscoveryService, DiscoveryResponse, DiscoverySource } from '../../services/discovery.service';
import { AdminService, ObservableResponse } from '../../services/admin.service';
import { SearchStatisticsService } from '../../services/search-statistics.service';
import { ClientsService, ClientsResponse } from '../../services/clients.service';
import { HttpClient } from '@angular/common/http';
import * as convertXml from 'xml-js';
import * as formatXml from 'xml-formatter';


interface Environment {
  name: string;
  url: string;
}

@Component({
  selector: 'infoserv',
  styleUrls: ['./infoserv.component.scss'],
  templateUrl: './infoserv.component.html',
})
export class InfoservComponent {
  public url: string;
  private cid: string = 'F10B84FB252746C69AE87D36FB923408';
  private apiKey: string = 'F1FDE34C823E4671BE1926F3F892DFB5';

  results: string;
  clients: DiscoveryResponse;
  selectedClient: DiscoverySource;
  users: ClientsResponse[];
  selectedUser: ClientsResponse;
  environments: Environment[] = [
    { name: 'Local', url: 'http://localhost:3901' },
    { name: 'Development', url: 'http://engweb03.show2000.com:3901' },
    { name: 'Integration', url: 'http://engweb03.show2000.com:3902' },
    { name: 'Release', url: 'http://engweb03.show2000.com:3903' },
    { name: 'Custom', url: '' },
  ];
  selectedEnvironment: Environment = this.environments[0];
  apiRequest: string = '';

  constructor(
    private http: HttpClient,
    private discoveryService: DiscoveryService,
    private searchStatisticsService: SearchStatisticsService,
    private adminService: AdminService,
    private clientsService: ClientsService) {
      this.url = this.selectedEnvironment.url;
      this.populateSelectBoxes();
    }

    private populateSelectBoxes(): void {
      this.discoveryService.getDiscovery(this.url, undefined, this.apiKey)
        .observable.subscribe(result => {
          this.clients = result;
          this.selectedClient = result.Sources[0];
          this.cid = this.selectedClient.ConfigUid;
        });
      this.clientsService.getClients(this.url, undefined, this.apiKey)
        .observable.subscribe(result => {
          this.users = result;
          var admin = result.find(x => x.ApplicationName == 'admin');
          this.selectedUser = (admin === undefined) ? result[0] : admin;
          this.apiKey = this.selectedUser.ApiKey;
        });
    }

    public onCidChanged(event: any)
    {
      this.cid = this.selectedClient.ConfigUid;
    }

    public onApiKeyChanged(event: any)
    {
      this.apiKey = this.selectedUser.ApiKey;
    }

    public onEnvironmentSelectionChanged(event: any): void {
      this.url = this.selectedEnvironment.url;

      this.populateSelectBoxes();
    }

    public onUrlChanged(event: any): void {
      var matchUrl = this.environments.find(x => x.url == this.url);
      if (matchUrl === undefined) {
        this.selectedEnvironment = this.environments.find(x => x.name == 'Custom');
        this.selectedEnvironment.url = this.url;
      } else {
        this.selectedEnvironment = matchUrl;
      }

      this.populateSelectBoxes();
    }

    private processing(): void {
      this.results = "Processing...";
    }

    private convertXmlToJson(xml: string): string {
      //var convert = require('xml-js');
      return convertXml.xml2json(xml, { compact: true, spaces: 4, ignoreComment: true })
    }

    private outputResults(result: any): void{
      this.results = JSON.stringify(result, null, 4);
    }

    private outputXmlResults(result: any): void{
      //var format = require('xml-formatter');
      this.results = formatXml(result, { indentation: '      ' });
    }

    private outputJsonResults(result: any): void{
      this.results = JSON.stringify(result, null, 4);
    }

    private outputXmlAsJson(result: any): void {
      this.results = this.convertXmlToJson(result);
    }

    private commonCallback(apiCallback: Function, resultCallback: Function, params?: string) {
      this.processing();

      var observable: ObservableResponse;
      if (params) {
        observable = apiCallback(this.url, this.cid, this.apiKey, params);
      } else {
        observable = apiCallback(this.url, this.cid, this.apiKey);
      }
      this.apiRequest = observable.apiRequest;
      observable.observable.subscribe(result => { resultCallback(result); });
    }

    onDiscoveryClick (event: any): void {
      this.commonCallback(this.discoveryService.getDiscovery.bind(this), this.outputResults.bind(this));
    }

    onGetClientsClick (event: any): void {
      this.commonCallback(this.clientsService.getClients.bind(this), this.outputResults.bind(this));
    }

    onMetadataClick (event: any){
      this.commonCallback(this.adminService.getMetaData.bind(this), this.outputResults.bind(this));
    }

    onGetDataConfigClick (event: any){
      this.commonCallback(this.adminService.getConfiguration.bind(this), this.outputXmlResults.bind(this), 'DATA');
    }

    onGetEntityConfigClick (event: any){
      this.commonCallback(this.adminService.getConfiguration.bind(this), this.outputXmlResults.bind(this), 'ENTITY');
    }

    onGetSearchConfigClick (event: any){
      this.commonCallback(this.adminService.getConfiguration.bind(this), this.outputXmlAsJson.bind(this), 'SEARCH');
    }

    onConfigReloadClick (event: any){
      this.commonCallback(this.adminService.reloadConfig.bind(this), this.outputResults.bind(this));
    }

    onReloadSegmentsClick (event: any){
      this.commonCallback(this.adminService.reloadSegments.bind(this), this.outputResults.bind(this));
    }

    onReloadAllSegmentsClick (event: any){
      this.commonCallback(this.adminService.reloadAllSegments.bind(this), this.outputResults.bind(this));
    }

    onSearchStatisticsClick (event: any){
      this.commonCallback(this.searchStatisticsService.getSearchIndexStatistics.bind(this), this.outputResults.bind(this));
    }

    onRebuildSearchIndexClick (event: any){
      this.commonCallback(this.adminService.rebuildSearchIndex.bind(this), this.outputResults.bind(this));
    }

    onReindexSearchIndexClick (event: any){
      this.commonCallback(this.adminService.reindexSearchIndex.bind(this), this.outputResults.bind(this));
    }

    onSyncSearchIndexClick (event: any){
      this.commonCallback(this.adminService.syncSearchIndex.bind(this), this.outputResults.bind(this));
    }
}
