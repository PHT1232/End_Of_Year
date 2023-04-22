import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoleServiceProxy,
  RoleDto,
  PermissionDto,
  CreateRoleDto,
  PermissionDtoListResultDto
} from '@shared/service-proxies/service-proxies';
import { forEach as _forEach, map as _map } from 'lodash-es';

@Component({
  templateUrl: 'create-role-dialog.component.html'
})
export class CreateRoleDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  role = new RoleDto();
  permissions: PermissionDto[] = [];
  checkedPermissionsMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;

  public permissions1 = [
    { label: 'Quản lý người dùng', 
      data: 'Pages.Users', 
      children: 
      [
      {
          label: 'Thêm mới',
          data: 'Pages.Users.Activation',
          checked: false,
      }], 
      expanded: true },
      { label: 'Quản lý kho', 
      data: 'Pages.System.Storage', 
      children: 
      [{
          label: 'Xem danh sách',
          data: 'Pages.System.Storage.View',
          checked: true,
      },
      {
          label: 'Thêm mới',
          data: 'Pages.System.Storage.Add',
          checked: true,
      },
      {
          label: 'Cập nhật',
          data: 'Pages.System.Storage.Update',
          checked: true,
      },
      {
          label: 'Xóa',
          data: 'Pages.System.Storage.Delete',
          checked: true,
      }], 
      expanded: true },
      { label: 'Quản lý danh mục', 
      data: 'Pages.System.Category', 
      children: 
      [{
          label: 'Xem danh sách',
          data: 'Pages.System.Category.View',
          checked: true,
      },
      {
          label: 'Thêm mới',
          data: 'Pages.System.Category.Add',
          checked: true,
      },
      {
          label: 'Cập nhật',
          data: 'Pages.System.Category.Update',
          checked: true,
      },
      {
          label: 'Xóa',
          data: 'Pages.System.Category.Delete',
          checked: true,
      }], 
      expanded: true },
      { label: 'Quản lý sản phẩm', 
      data: 'Pages.System.Product', 
      children: 
      [{
          label: 'Xem danh sách',
          data: 'Pages.System.Product.View',
          checked: true,
      },
      {
          label: 'Thêm mới',
          data: 'Pages.System.Product.Add',
          checked: true,
      },
      {
          label: 'Cập nhật',
          data: 'Pages.System.Product.Update',
          checked: true,
      },
      {
          label: 'Xóa',
          data: 'Pages.System.Product.Delete',
          checked: true,
      }], 
      expanded: true },
      { label: 'Quản lý xuất nhập', 
      data: 'Pages.System.ExportImport', 
      children: 
      [{
          label: 'Xem danh sách',
          data: 'Pages.System.ExportImport.View',
          checked: true,
      },
      {
          label: 'Thêm mới',
          data: 'Pages.System.ExportImport.Add',
          checked: true,
      },
      {
          label: 'Cập nhật',
          data: 'Pages.System.ExportImport.Update',
          checked: true,
      },
      {
          label: 'Xóa',
          data: 'Pages.System.ExportImport.Delete',
          checked: true,
      }], 
      expanded: true },
  ];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _roleService: RoleServiceProxy,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._roleService
      .getAllPermissions()
      .subscribe((result: PermissionDtoListResultDto) => {
        this.permissions = result.items;
        this.setInitialPermissionsStatus();
      });
  }

  setInitialPermissionsStatus(): void {
    _map(this.permissions, (item) => {
      this.checkedPermissionsMap[item.name] = this.isPermissionChecked(
        item.name
      );
    });
  }

  isPermissionChecked(permissionName: string): boolean {
    // just return default permission checked status
    // it's better to use a setting
    if (permissionName === 'Pages.Users') {
      return false;
    }
    return this.defaultPermissionCheckedStatus;
  }

  onPermissionChange(permission: string, $event) {
    this.checkedPermissionsMap[permission] = $event.target.checked;
  }

  onFatherPermissionChange(permission: string, $event) {
    this.permissions1.forEach(element => {
      if (element.data === permission) {
        element.children.forEach(elementChild => {
            this.checkedPermissionsMap[permission] = $event.target.checked;
            this.checkedPermissionsMap[elementChild.data] = $event.target.checked;
            elementChild.checked = $event.target.checked;
        });
      }
    });
  }

  getCheckedPermissions(): string[] {
    const permissions: string[] = [];
    _forEach(this.checkedPermissionsMap, function (value, key) {
      if (value) {
        permissions.push(key);
      }
    });
    return permissions;
  }

  save(): void {
    this.saving = true;

    const role = new CreateRoleDto();
    role.init(this.role);
    role.grantedPermissions = this.getCheckedPermissions();

    this._roleService
      .create(role)
      .subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
  }
}
