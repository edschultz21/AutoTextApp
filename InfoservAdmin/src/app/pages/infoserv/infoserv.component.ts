import { Component } from '@angular/core';
import { DiscoveryResponse, DiscoverySource } from '../../services/discovery.service';
import { ObservableResponse } from '../../services/admin.service';
import { ClientsResponse } from '../../services/clients.service';
import { InfoservService } from '../../services/infoserv.service';
import { NbMenuService } from '@nebular/theme';


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
    private infoservService: InfoservService,
    private menuService: NbMenuService) {
      this.url = this.selectedEnvironment.url;
      this.populateSelectBoxes();

      this.menuService.onItemClick().subscribe((event: {tag: string, item: any}) => {
        if (this.infoservService[event.item.fragment]) {
          this.infoservService[event.item.fragment](this.commonCallback.bind(this));
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
      observable.observable.subscribe(result => { this.results = resultCallback(result); });
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
}
