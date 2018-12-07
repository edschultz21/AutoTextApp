import { NgModule } from '@angular/core';

import { PagesComponent } from './pages.component';
import { InfoservModule } from './infoserv/infoserv.module';
import { HelpModule } from './help/help.module';
import { PagesRoutingModule } from './pages-routing.module';
import { ThemeModule } from '../@theme/theme.module';

const PAGES_COMPONENTS = [
  PagesComponent,
];

@NgModule({
  imports: [
    PagesRoutingModule,
    ThemeModule,
    InfoservModule,
    HelpModule,
  ],
  declarations: [
    ...PAGES_COMPONENTS,
  ],
})
export class PagesModule {
}
