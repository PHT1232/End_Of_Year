import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { CategoryInput, CategoryServiceProxy, PermissionDto, ProductServiceProxy } from '@shared/service-proxies/service-proxies';
import { CategoryProduct, CategoryProductList } from '@shared/service-proxies/dtos/products/CategoryProduct';
import { ProductGetAllDto } from '@shared/service-proxies/dtos/products/ProductGetAllDto';
import { ProductInputDto } from '@shared/service-proxies/dtos/products/ProductInputDto';
import { ProductOutputDto } from '@shared/service-proxies/dtos/products/ProductOutputDto';
import { ProductStorageDto } from '@shared/service-proxies/dtos/products/ProductStorageDto';
import { StorageProductDetail, StorageProductDetailList } from '@shared/service-proxies/dtos/products/StorageProductDetail';
import { ProductGetAllPagedResultDto } from '@shared/service-proxies/dtos/products/ProductGetAllPagedResultDto';
import { SubcategoryProduct, SubcategoryProductList } from '@shared/service-proxies/dtos/products/SubcategoryProduct';
import { throwError } from 'rxjs';
import { catchError, delay, finalize } from 'rxjs/operators';
import { MessageService } from 'primeng/api';
import { AppComponent } from '@app/app.component';

class PagedProductRequestDto extends PagedRequestDto {
  keyword: string;
  storageCode: string;
  categoryCode: string;
  subcategoryCode: number;
}

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
  animations: [appModuleAnimation()]
})
export class ProductComponent extends PagedListingComponentBase<ProductGetAllDto> {
  keyword = '';
  storageCode = '1';
  categoryCode = '0';
  subcategoryCode = 0;
  productList: ProductGetAllDto[] = [];
  getStorage: StorageProductDetailList = new StorageProductDetailList();
  getCategory: CategoryProductList = new CategoryProductList();
  getSubcategorycode: SubcategoryProductList = new SubcategoryProductList();
  totalCount: number;
  first: number = 0;
  rows: number = 6;
  selectedProducts: ProductGetAllDto[] = [];

  constructor(
    injector: Injector,
    private _productService: ProductServiceProxy,
    private appMain: AppComponent
  ) { 
    super(injector);
      this._productService.getStorageProduct().subscribe(val => {
        this.getStorage = val;
      });
  
      this._productService.getCategoryProduct().subscribe(val => {
          this.getCategory = val;
      });
    
    // console.log(this.getStorage.items[0].storageCode)
    if (this.getStorage.items === undefined) {
      this.storageCode = '0';
      // this.getStorage.items = [];
      // this.getStorage.items[0] = new StorageProductDetail();
    }
  }

  list(request: PagedProductRequestDto, pageNumber: number, finishedCallback: Function): void {
    request.keyword = this.keyword;
    request.storageCode = this.storageCode;
    request.categoryCode = this.categoryCode;
    setTimeout(() => { 
      request.subcategoryCode = this.subcategoryCode; 
      
      console.log(request.keyword);
      console.log(request.storageCode);
      console.log(request.categoryCode);
      console.log(request.subcategoryCode);
      
      this._productService
      .getAll(request.keyword, request.storageCode, request.categoryCode, request.subcategoryCode, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: ProductGetAllPagedResultDto) => {
        this.productList = result.items;
        this.showPaging(result, pageNumber);
      });
    },500);
  }
  delete(entity: ProductGetAllDto): void {
    this.swal.fire({
      title: 'Bạn có chắc?',
      text: 'Sản phẩm sẽ bị xóa',
      showCancelButton: true,
      confirmButtonColor: this.confirmButtonColor,
      cancelButtonColor: this.cancelButtonColor,
      cancelButtonText: 'Hủy',
      confirmButtonText: 'Xóa',
      reverseButtons: this.ReverseButtons,
      icon: 'warning',
    })
    .then((result) => {
      if (result.value) {
        this._productService.delete(entity.productCode).pipe(
          catchError(err => {
            return throwError(err);
          }))
          .subscribe({
            next: () => {
              this.appMain.showSuccessMessage('Xóa thành công', 'Xóa thành công sản phẩm thành công')
              this.refresh();
            },
            error: (error) => {
              console.log(error);
              if (error.error && error.error.message) {
                this.notify.error(error.error.message);
              }
            },
            complete() {
              
            },
          })
      }
    });
  }

  checkOnDelete() {
    // console.log("Selected storage: " + this.selectedStorages.storageCode)
    this.swal.fire({
      title: 'Bạn có chắc?',
      text: this.selectedProducts.length + ' sản phẩm sẽ bị xóa',
      showCancelButton: true,
      confirmButtonColor: this.confirmButtonColor,
      cancelButtonColor: this.cancelButtonColor,
      cancelButtonText: 'Hủy',
      confirmButtonText: 'Xóa',
      reverseButtons: this.ReverseButtons,
      icon: 'warning',
    })
    .then((result) => {
      if (result.value) {
        var numOfDeleted = 0;
        var numToDelete = this.selectedProducts.length;
        this.selectedProducts.forEach(element => {
          this._productService.delete(element.productCode).pipe(
            catchError(err => {
              return throwError(err);
            }))
            .subscribe({
              next: () => {
                // this.message.add({ severity: 'success', summary: 'Xóa thành công', detail: 'Xóa thành công kho thành công'})
                numOfDeleted += 1;
                // abp.notify.success(this.l('Xóa thành công'));
                this.refresh();
              },
              error: (error) => {
                console.log(error);
                if (error.error && error.error.message) {
                  this.notify.error(error.error.message);
                }
              },
              complete() {
                
              },
            })
          });
          this.appMain.showSuccessMessage('Xóa thành công', 'Xóa thành công ' + numOfDeleted + '/' + numToDelete + ' kho thành công')
        }
      });
  }

  getSubcategory() {
    if (this.categoryCode !== '0') {
      this._productService.getSubcategoryProduct(this.categoryCode).subscribe(val => {
          this.getSubcategorycode = val
          this.subcategoryCode = 0;
      });
    } 
  }
}
