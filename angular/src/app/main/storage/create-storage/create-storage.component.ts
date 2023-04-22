import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { PermissionDto, StorageInput, StorageServiceProxy } from '@shared/service-proxies/service-proxies';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AppComponent } from '@app/app.component';

@Component({
  selector: 'app-create-storage',
  templateUrl: './create-storage.component.html',
  styleUrls: ['./create-storage.component.css'],
  animations: [appModuleAnimation()]
})
export class CreateStorageComponent extends AppComponentBase implements OnInit {
  saving = false;
  storage = new StorageInput();
  permissions: PermissionDto[] = [];
  checkedPermissionMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _router: Router,
    private _storageService: StorageServiceProxy,
    private appMain: AppComponent
  ) { 
    super(injector);
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;
    const storageAdd = new StorageInput();
    storageAdd.init(this.storage);
    
    this._storageService.createStorage(storageAdd).subscribe(
      () => {
        // this.notify.info(this.l('Thêm mới thành công'));
        // this.message.add({ severity: 'success', summary: 'Thêm mới thành công', detail: 'Thêm mới kho thành công'});
        this.appMain.showSuccessMessage('Thêm mới thành công', 'Thêm mới kho thành công')
        this.onSave.emit();
        this._router.navigate(['app/storage']);
      },
      () => {
        this.saving = false;
      }
    )
  }

  Cancel(): void {
    this._router.navigate(['app/storage']);
  }

}
