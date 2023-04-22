import { Component, Injector, OnInit } from '@angular/core';
import { NavigationEnd, PRIMARY_OUTLET, Router, RouterEvent } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';
import { BehaviorSubject, filter } from 'rxjs';

@Component({
  selector: 'app-sidebarnew',
  templateUrl: './sidebarnew.component.html',
  styleUrls: ['./sidebarnew.component.css']
})
export class SidebarnewComponent extends AppComponentBase implements OnInit {
  position: string = 'left';
  menuItems: MenuItem[];
  menuItemsMap: { [key: number]: MenuItem } = {};
    activatedMenuItems: MenuItem[] = [];
    routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
    homeRoute = '/app/home';

    constructor(injector: Injector, private router: Router) {
      super(injector);
      this.router.events.subscribe(this.routerEvents);
  }

  ngOnInit(): void {
      this.menuItems = this.getMenuItems();
      this.patchMenuItems(this.menuItems);
      this.routerEvents
          .pipe(filter((event) => event instanceof NavigationEnd))
          .subscribe((event) => {
              const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
              const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
                  .children[PRIMARY_OUTLET];
              if (primaryUrlSegmentGroup) {
                  this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
              }
          });
  }

  getMenuItems(): MenuItem[] {
    return [
        // new MenuItem(
        //     this.l('Test'),
        //     '/app/test',
        //     '',
        //     'Pages.System.Test'
        // ),
        new MenuItem(
            this.l('Kho'),
            '/app/storage',
            '',
            'Pages.System.Storage.View'
        ),
        new MenuItem(
            this.l('Danh mục'),
            '/app/category',
            '',
            'Pages.System.Category.View'
        ),
        new MenuItem(
            this.l('Sản phẩm'),
            '/app/product',
            '',
            'Pages.System.Product.View'
        ),
        new MenuItem(
            this.l('Xuất nhập kho'),
            '/app/exportimport',
            '',
            'Pages.System.ExportImport.View'
        ),
        new MenuItem(
            this.l('Vai trò'),
            '/app/roles',
            '',
            'Pages.Users'
        ),
        new MenuItem(
            this.l('Người dùng'),
            '/app/users',
            '',
            'Pages.Users'
        ),
    ];
}

patchMenuItems(items: MenuItem[], parentId?: number): void {
    items.forEach((item: MenuItem, index: number) => {
        item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
        if (parentId) {
            item.parentId = parentId;
        }
        if (parentId || item.children) {
            this.menuItemsMap[item.id] = item;
        }
        if (item.children) {
            this.patchMenuItems(item.children, item.id);
        }
    });
}

activateMenuItems(url: string): void {
    this.deactivateMenuItems(this.menuItems);
    this.activatedMenuItems = [];
    const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
    foundedItems.forEach((item) => {
        this.activateMenuItem(item);
    });
}

deactivateMenuItems(items: MenuItem[]): void {
    items.forEach((item: MenuItem) => {
        item.isActive = false;
        item.isCollapsed = true;
        if (item.children) {
            this.deactivateMenuItems(item.children);
        }
    });
}

findMenuItemsByUrl(
    url: string,
    items: MenuItem[],
    foundedItems: MenuItem[] = []
): MenuItem[] {
    items.forEach((item: MenuItem) => {
        if (item.route === url) {
            foundedItems.push(item);
        } else if (item.children) {
            this.findMenuItemsByUrl(url, item.children, foundedItems);
        }
    });
    return foundedItems;
}

activateMenuItem(item: MenuItem): void {
    item.isActive = true;
    if (item.children) {
        item.isCollapsed = false;
    }
    this.activatedMenuItems.push(item);
    if (item.parentId) {
        this.activateMenuItem(this.menuItemsMap[item.parentId]);
    }
}

isMenuItemVisible(item: MenuItem): boolean {
    if (!item.permissionName) {
        return true;
    }
    return this.permission.isGranted(item.permissionName);
}
}
