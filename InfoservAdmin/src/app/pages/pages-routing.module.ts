import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

import { PagesComponent } from './pages.component';
import { InfoservComponent } from './infoserv/infoserv.component';
import { HelpComponent } from './help/help.component';

const routes: Routes = [{
  path: '',
  component: PagesComponent,
  children: [{
    path: 'infoserv',
    component: InfoservComponent,
  }, {
    path: 'help',
    component: HelpComponent,
  }, {
    path: '',
    redirectTo: 'infoserv',
    pathMatch: 'full',
  }],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule {
}
