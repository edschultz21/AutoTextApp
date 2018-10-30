import { NgModule } from '@angular/core';

import { ThemeModule } from '../../@theme/theme.module';
import { InfoservComponent } from './infoserv.component';

@NgModule({
  imports: [
    ThemeModule,
  ],
  declarations: [
    InfoservComponent,
  ],
})
export class InfoservModule { }
