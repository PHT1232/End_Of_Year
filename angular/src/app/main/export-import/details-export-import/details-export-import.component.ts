import { Component, OnInit, Output, EventEmitter, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { 
  CategoryInput, 
  CategoryServiceProxy, 
  CustomerDto, 
  ExportImportInput, 
  ExportImportOutputDto, 
  ExportImportPagedResult, 
  ExportImportProductDto, 
  ExportImportService, 
  PermissionDto, 
  ProductServiceProxy, 
} from '@shared/service-proxies/service-proxies';
import { LookUpTableList } from '@shared/service-proxies/dtos/LookUpTable';
import { StorageProductDetailList } from '@shared/service-proxies/dtos/products/StorageProductDetail';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

class PagedProductRequestDto extends PagedRequestDto {
  storageCode: string;
  isInsert: boolean;
}

class InitialProductQuantity {
  id: string;
  quantity: number;
}

@Component({
  selector: 'app-details-export-import',
  templateUrl: './details-export-import.component.html',
  styleUrls: ['./details-export-import.component.css'],
  animations: [appModuleAnimation()]
})
export class DetailsExportImportComponent extends AppComponentBase implements OnInit {

  saving = false;
  isCollapsed = false;
  exportImport = new ExportImportOutputDto();
  storageCode = '';
  keyword: string;
  getStorage: StorageProductDetailList = new StorageProductDetailList();
  orderType = 1;
  products: ExportImportProductDto[] = [];
  user: LookUpTableList = new LookUpTableList();
  customer = new CustomerDto();
  request: PagedProductRequestDto;
  pageSize = 5;
  pageNumber = 1;
  totalPages = 1;
  skipCount = (1 - 1) * this.pageSize;
  isTrue = true;
  quantityCheck: boolean[] = [];
  totalItems: number;
  initialProductQuantity: InitialProductQuantity[] = [];
  id: string;

  @Output() onsave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _router: Router,
    private _exportImport: ExportImportService,
    private router: ActivatedRoute,
    private _productService: ProductServiceProxy
  ) {
    super(injector);
    this._productService.getStorageProduct().subscribe(val => {
      this.getStorage = val;
      this.storageCode = val[val.items.length - 1].storageCode;
    });

    this.router.params.subscribe(params => {
      this.id = params['id']
    });

    setTimeout(() => {
      this._exportImport.get(this.id).subscribe((result: ExportImportOutputDto) => {
        this.exportImport.exportImportCode = result.exportImportCode;
        this.exportImport.orderCreator = result.orderCreator;
        this.exportImport.orderStatus = result.orderStatus;
        this.exportImport.orderType = result.orderType;
        this.exportImport.receiveAddress = result.receiveAddress;
        this.customer = result.customer;
        this.products = result.products;
        this.exportImport.storageId = result.storageId;
        this.exportImport.storageInputId = result.storageInputId;
        this.exportImport.nameOfExport = result.nameOfExport;
        this.exportImport.totalPrice = result.totalPrice;
      });
    }, 300);

    
    this._exportImport.getUser()
    .subscribe(val => {
      this.user = val;
    });
  }

  ngOnInit(): void {
  }

  public showPaging(result: ExportImportPagedResult, pageNumber: number): void {
    this.totalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;

    this.totalItems = result.totalCount;
    this.pageNumber = pageNumber;
}

  Cancel(): void {
    this._router.navigate(['app/exportimport']);
  }

  getProductPage() {

  }
}
