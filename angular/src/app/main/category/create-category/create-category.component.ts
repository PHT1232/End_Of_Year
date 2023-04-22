import { Component, OnInit, Output, EventEmitter, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CategoryInput, CategoryServiceProxy, PermissionDto } from '@shared/service-proxies/service-proxies';
import { FormArray, FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponent } from '@app/app.component';

@Component({
  selector: 'app-create-category',
  templateUrl: './create-category.component.html',
  styleUrls: ['./create-category.component.css'],
  animations: [appModuleAnimation()]
})
export class CreateCategoryComponent extends AppComponentBase implements OnInit {
  saving = false;
  category = new CategoryInput();
  permissions: PermissionDto[] = [];
  checkedPermissionMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;
  subCategoryFormArray = new FormArray([]);
  isExist: boolean[] = [];
  errorMessage = 'Không được trùng với tên hoặc mã danh mục';
  isTrue = true;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _router: Router,
    private _categoryService: CategoryServiceProxy,
    private appMain: AppComponent
  ) { 
    super(injector);
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;

    const categoryAdd = new CategoryInput();
    categoryAdd.categoryCode = this.category.categoryCode;
    categoryAdd.categoryName = this.category.categoryName;
    categoryAdd.description = this.category.description;
    categoryAdd.subCategories = [];
    this.subCategoryFormArray.controls.forEach(element => {
      categoryAdd.subCategories.push(element.value.toString());
    });
    
    this._categoryService.create(categoryAdd).subscribe(
      () => {
        this.appMain.showSuccessMessage('Thêm mới thành công', 'Thêm mới danh mục thành công');
        this.onSave.emit();
        this._router.navigate(['app/category']);
      },
      () => {
        this.saving = false;
      }
    )
  }

  Cancel(): void {
    this._router.navigate(['app/category']);
  }

  AddItem(): void {
    this.subCategoryFormArray.push(new FormControl(''));
  }

  RemoveItem(index: number) {
    this.subCategoryFormArray.removeAt(index);
  }

  onChange(subcategoryName: string, index: number) {
    var isForeach = false;
    // if (this.subCategoryFormArray.length > 1) {
      this.subCategoryFormArray.controls.forEach((element, indexElement) => {
        if (element.value.trim() === subcategoryName.trim() && indexElement !== index || subcategoryName.trim() === this.category.categoryName || subcategoryName.trim() === this.category.categoryCode) {
            isForeach = true;
            this.isExist[index] = true;
            // this.isTrue = true;
        }
      });
      if (!isForeach) {
        this.isExist[index] = false;
      }
    // }
  }

  checkFormValid(): boolean {
    if (this.category.categoryCode === undefined
      || this.category.categoryName === undefined
      || this.category.categoryCode === ''
      || this.category.categoryName === '') {
          return true;
    }

    if (this.subCategoryFormArray.length >= 0) {
      this.subCategoryFormArray.controls.forEach((element, index) => {
        if (element.value === undefined || element.value === '' || element.value === this.category.categoryName || element.value === this.category.categoryCode) {
          this.isTrue = true;
          return;
        }
      });
    }

    this.isExist.forEach(element => {
      if (element) {
        console.log("is true")
        this.isTrue = true;
      }
    });

    if (this.isTrue) {
      this.isTrue = false;
      return true;
    }
  }
}
