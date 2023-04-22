import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { AppComponent } from '@app/app.component';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  selector: 'header-left-navbar',
  templateUrl: './header-left-navbar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderLeftNavbarComponent implements OnInit {
  sidebarExpanded: boolean;

  constructor(private _layoutStore: LayoutStoreService,
    private appMain: AppComponent) {}

  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
    this.appMain.toggleSidebarDisplay();
  }
}
