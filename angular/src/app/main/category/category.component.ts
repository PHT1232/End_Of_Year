import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { CategoryServiceProxy, GetAllCategoryDto, GetAllCategoryPagedResultDto } from '@shared/service-proxies/service-proxies';
import { throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

class PagedCategoryRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css'],
  animations: [appModuleAnimation()]
})
export class CategoryComponent extends PagedListingComponentBase<GetAllCategoryDto> {
  keyword = '';
  categories: GetAllCategoryDto[] = [];
  totalCount: number;

  constructor(
    injector: Injector,
    private _categoryService: CategoryServiceProxy
  ) { 
    super(injector);
  }

  list(request: PagedCategoryRequestDto, pageNumber: number, finishedCallback: Function): void {
    request.keyword = this.keyword;

    this._categoryService
      .getAll(request.keyword, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: GetAllCategoryPagedResultDto) => {
        this.categories = result.items;
        this.showPaging(result, pageNumber);
      });
  }
  delete(entity: GetAllCategoryDto): void {
    this.swal.fire({
      title: 'Bạn có chắc?',
      text: 'Danh mục sẽ bị xóa',
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
        this._categoryService.delete(entity.categoryCode).pipe(
          catchError(err => {
            return throwError(err);
          }))
          .subscribe({
            next: () => {
              abp.notify.success(this.l('Xóa thành công'));
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
}
