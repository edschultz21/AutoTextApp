import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { DiscoveryResponse, DiscoverySource } from '../../services/discovery.service';
import { ObservableResponse } from '../../services/admin.service';
import { ClientsResponse } from '../../services/clients.service';
import { InfoservService } from '../../services/infoserv.service';
import { NbMenuService } from '@nebular/theme';
import * as TypesIS from '../../pages/infoserv/infoserv.types';
//import { CookieService } from 'angular2-cookie/core';

// Testing
// APIKey - F1FDE34C823E4671BE1926F3F892DFB5



@Component({
  selector: 'infoserv',
  styleUrls: ['./infoserv.component.scss'],
  templateUrl: './infoserv.component.html',
})
export class InfoservComponent {

  public displayResultEnum = TypesIS.DisplayResultEnum;
  public resultToDisplay: TypesIS.DisplayResultEnum = TypesIS.DisplayResultEnum.Default;

  public url: string;
  private cid: string = 'F10B84FB252746C69AE87D36FB923408';
  public apiKey: string = 'F1FDE34C823E4671BE1926F3F892DFB5';

  public stringResults: string;
  public dataDebugResults: string;
  public segmentSearchResults: TypesIS.SegmentSearchResult[];
  public detailsResults: TypesIS.DetailsResult[];
  public dataResults: TypesIS.DataResult[];
  public clients: DiscoveryResponse;
  public selectedClient: DiscoverySource;
  public users: ClientsResponse[];
  public selectedUser: ClientsResponse;
  public environments: TypesIS.Environment[] = [
    { name: 'Local', url: 'http://localhost:3901' },
    { name: 'Development', url: 'http://engweb03.show2000.com:3901' },
    { name: 'Integration', url: 'http://engweb03.show2000.com:3902' },
    { name: 'Release', url: 'http://engweb03.show2000.com:3903' },
    { name: 'Custom', url: '' },
  ];
  public selectedEnvironment: TypesIS.Environment = this.environments[0];
  public apiRequest: string = '';
  // public apiBodySegmentSearch = 'query=real';
  // public apiBodyDetail: string = '{ "Query": "GET o.* FROM Office o WHERE o.SegmentKey IN (\'1372832\', \'1372835\', \'1372840\')"}';
  // public apiBodyDataMultipleMetrics: string = '{"Id":null,"Queries":[{"Id":null,"Filters":[{"Segments":"1273128","Filters":null,"TimeSeries":null}],"Metrics":[{"Id": "TotalVolume","Metric": "ttv","MetricModifiers": null,"Filters": null},{"Id": "MetricRank","Metric": "ttv","MetricModifiers": ["rank"],"Filters": null}],"Segments":{"List":[{"Id":"zipcode","Segments":"zipcode","RemoveEmptySegments":true}],"Order":{"Limit":{"Skip":0,"Take":5}}},"TimeSeries":{"PeriodType":"month","PeriodCalculation":"none","PeriodList":"LP-11..LP"},"Options":null}],"Fragments":null}';
  // public apiBodyDataOptionsIncludeContext: string = '{"Id":null,"Queries":[{"Id":null,"Filters":[{"Segments":"1273128","Filters":null,"TimeSeries":null}],"Metrics":[{"Id":"TotalActiveListings","Metric":"cac","MetricModifiers":null,"Filters":null}],"Segments":{"List":[{"Id":"zipcode","Segments":"zipcode","RemoveEmptySegments":true}],"Order":{"Limit":{"Skip":0,"Take":5}}},"TimeSeries":{"PeriodType":"month","PeriodCalculation":"none","PeriodList":"LP-11..LP"},"Options":{"ExperimentalFlags": "include_context"}}],"Fragments":null}';
  // public apiBodyDataOptionsIncludeQuery: string = '{"Id":null,"Queries":[{"Id":null,"Filters":[{"Segments":"1273128","Filters":null,"TimeSeries":null}],"Metrics":[{"Id":"TotalActiveListings","Metric":"cac","MetricModifiers":null,"Filters":null}],"Segments":{"List":[{"Id":"zipcode","Segments":"zipcode","RemoveEmptySegments":true}],"Order":{"Limit":{"Skip":0,"Take":5}}},"TimeSeries":{"PeriodType":"month","PeriodCalculation":"none","PeriodList":"LP-11..LP"},"Options":{"ExperimentalFlags": "include_query"}}],"Fragments":null}';
  // public apiBodyDataOptionsIncludeOptimizations: string = '{"Id":null,"Queries":[{"Id":null,"Filters":[{"Segments":"1273128","Filters":null,"TimeSeries":null}],"Metrics":[{"Id":"TotalActiveListings","Metric":"cac","MetricModifiers":null,"Filters":null}],"Segments":{"List":[{"Id":"zipcode","Segments":"zipcode","RemoveEmptySegments":true}],"Order":{"Limit":{"Skip":0,"Take":5}}},"TimeSeries":{"PeriodType":"month","PeriodCalculation":"none","PeriodList":"LP-11..LP"},"Options":{"ExperimentalFlags": "include_optimizations"}}],"Fragments":null}';
  public apiBody: string = '';
  public collapsedRows: boolean[] = [true, false, true];

  constructor(
    private router: Router,
    private infoservService: InfoservService,
    private menuService: NbMenuService
    //private cookieService: CookieService
    ) {
      this.url = this.selectedEnvironment.url;
      this.populateSelectBoxes();

      this.apiKey = localStorage.getItem('InfoservApiKey');

      var self = this;
      this.menuService.onItemClick().subscribe((event: {tag: string, item: any}) => {
        // Given my limited understanding of nebular and angular, can't think of a
        // better way to do this. Basically, if we switch away from this page (eg, to a
        // help page) and click on one of the infoserv items, we want to be able
        // do display that page and then execute the command to display the output.
        // Adding a "link" to the menu item will bring up the page but will not
        // fire this onItemClick event. We could setup each item to have its very
        // own link, but that seems like supreme overkill. Hence, we check to see
        // if we are on the correct page, and if not, display it.
        var parts = self.router.url.split('/');
        var path = parts[parts.length - 1];
        if (path != 'infoserv') {
          this.router.navigate(['infoserv']);
        }
        else {
          parts = event.item.data.split(',');
          var callback = parts[0];

          // There must be a better (and safer) way to do this.
          this.resultToDisplay = TypesIS.DisplayResultEnum.Default;
          if (parts.length === 2) {
            if (parts[1] === 'Details') {
              this.resultToDisplay = TypesIS.DisplayResultEnum.Details;
            } else if (parts[1] === 'Data') {
              this.resultToDisplay = TypesIS.DisplayResultEnum.Data;
            } else if (parts[1] === 'SegmentSearch') {
              this.resultToDisplay = TypesIS.DisplayResultEnum.SegmentSearch;
            }
          }
          if (self.infoservService[callback]) {
            self.infoservService[callback](self.commonCallback.bind(self));
          }
        }
      });
    }

    public processing(): void {
      this.stringResults = "Processing...";
    }

    private commonCallback(apiCallback: Function, resultCallback: Function, params?: string): void {
      this.processing();

      var observable: ObservableResponse;
      if (params) {
        if (params === 'POST') {
          observable = apiCallback(this.url, this.cid, this.apiKey, this.apiBody);
        }
        else {
          this.apiBody = '';
        observable = apiCallback(this.url, this.cid, this.apiKey, params);
        }
      } else {
        this.apiBody = '';
        observable = apiCallback(this.url, this.cid, this.apiKey);
      }
      this.apiRequest = observable.apiRequest;
      observable.observable.subscribe(
        result => {
          var callbackResults = resultCallback(result);
          this.stringResults = callbackResults.stringResult;
          this.detailsResults = callbackResults.rawResults.Payload;
          this.dataResults = callbackResults.rawResults.Payload;
          if (this.dataResults) {
            var dataDebug = this.dataResults.find(x => x.Type === 'DEBUG');
            if (dataDebug) {
              // Wish I knew a better way to do this.
              var debugResults = (dataDebug as {} as TypesIS.DataDebugResult).Debug;
              this.dataDebugResults = debugResults;
              try {
                // Should pretty print the results if it is JSON output.
                // Not sure the best (proper) way to do this, but rebuilding the object
                // and then stringifying it does the trick.
                this.dataDebugResults = JSON.stringify(JSON.parse(debugResults), null, '    ');
              }
              catch (ex) {
                // Outupt was not in JSON format so leave it alone.
              }
            }
          }
          this.segmentSearchResults = callbackResults.rawResults;
        },
        error => { this.stringResults = resultCallback(error.stringResult); }
      );
    }

    private populateSelectBoxes(): void {
      this.infoservService.getDiscoveryWrapper(this.url, this.apiKey)
        .observable.subscribe(result => {
          this.clients = result;
          this.selectedClient = result.Sources[0];
          this.cid = this.selectedClient.ConfigUid;
        });
      this.infoservService.getClientsWrapper(this.url, this.apiKey)
        .observable.subscribe(result => {
          this.users = result;
          var admin = result.find(x => x.ApplicationName == 'admin');
          this.selectedUser = (admin === undefined) ? result[0] : admin;
          //this.apiKey = this.selectedUser.ApiKey;
        });
    }

    public onCidChanged(event: any)
    {
      this.cid = this.selectedClient.ConfigUid;
    }

    public onApiKeySaveClick (event: any): void {
      localStorage.setItem('InfoservApiKey', this.apiKey);
    }

    public onApiKeyDeleteClick (event: any): void {
      localStorage.removeItem('InfoservApiKey');
      this.apiKey = '';
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

    public onSegmentTableRowSelected (index: any, displayid: any) {
      this.collapsedRows[index] = !this.collapsedRows[index];
    }
}
