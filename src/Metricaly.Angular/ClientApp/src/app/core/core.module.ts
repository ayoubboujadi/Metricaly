import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CopyToClipboardDirective } from './shared/directives/copy-to-clipboard.directive';



@NgModule({
  declarations: [CopyToClipboardDirective],
  imports: [
    CommonModule
  ],
  exports: [CopyToClipboardDirective]
})
export class CoreModule { }
