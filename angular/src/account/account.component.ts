import {
  Component,
  OnInit,
  ViewEncapsulation,
  Injector,
  Renderer2
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  templateUrl: './account.component.html',
  encapsulation: ViewEncapsulation.None,
  styles: [`
  :host ::ng-deep .pi-eye,
  :host ::ng-deep .pi-eye-slash {
      transform:scale(1.6);
      margin-right: 1rem;
      color: var(--primary-color) !important;
  }
`]
})
export class AccountComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private renderer: Renderer2) {
    super(injector);
  }

  showTenantChange(): boolean {
    return abp.multiTenancy.isEnabled;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, 'login-page');
  }
}
