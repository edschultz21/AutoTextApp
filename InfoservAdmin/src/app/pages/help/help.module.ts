import { NgModule } from '@angular/core';

import { ThemeModule } from '../../@theme/theme.module';
import { HelpComponent } from './help.component';

@NgModule({
  imports: [
    ThemeModule,
  ],
  declarations: [
    HelpComponent,
  ],
})
export class HelpModule { }
