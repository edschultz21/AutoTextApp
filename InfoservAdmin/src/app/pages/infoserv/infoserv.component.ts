import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { DiscoveryResponse, DiscoverySource } from '../../services/discovery.service';
import { ObservableResponse } from '../../services/admin.service';
import { ClientsResponse } from '../../services/clients.service';
import { InfoservService } from '../../services/infoserv.service';
import { NbMenuService } from '@nebular/theme';
//import { CookieService } from 'angular2-cookie/core';

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
  private url: string;
  private cid: string = 'F10B84FB252746C69AE87D36FB923408';
  private apiKey: string = 'F1FDE34C823E4671BE1926F3F892DFB5';

  private results: string;
  private clients: DiscoveryResponse;
  private selectedClient: DiscoverySource;
  private users: ClientsResponse[];
  private selectedUser: ClientsResponse;
  private environments: Environment[] = [
    { name: 'Local', url: 'http://localhost:3901' },
    { name: 'Development', url: 'http://engweb03.show2000.com:3901' },
    { name: 'Integration', url: 'http://engweb03.show2000.com:3902' },
    { name: 'Release', url: 'http://engweb03.show2000.com:3903' },
    { name: 'Custom', url: '' },
  ];
  private selectedEnvironment: Environment = this.environments[0];
  private apiRequest: string = '';

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
        else if (self.infoservService[event.item.data]) {
          self.infoservService[event.item.data](self.commonCallback.bind(self));
        }
      });
    }

    public processing(): void {
      this.results = "Processing...";
    }

    private commonCallback(apiCallback: Function, resultCallback: Function, params?: string): void {
      this.processing();

      var observable: ObservableResponse;
      if (params) {
        observable = apiCallback(this.url, this.cid, this.apiKey, params);
      } else {
        observable = apiCallback(this.url, this.cid, this.apiKey);
      }
      this.apiRequest = observable.apiRequest;
      observable.observable.subscribe(
        result => { this.results = resultCallback(result); },
        error => { this.results = resultCallback(error); });
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
}
