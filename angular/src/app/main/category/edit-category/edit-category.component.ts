import { Component, OnInit, Output, EventEmitter, Injector } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponent } from '@app/app.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { CategoryInput, CategoryOutputDto, CategoryServiceProxy, PermissionDto } from '@shared/service-proxies/service-proxies';
import { result } from 'lodash-es';

@Component({
  selector: 'app-edit-category',
  templateUrl: './edit-category.component.html',
  styleUrls: ['./edit-category.component.css'],
  animations: [appModuleAnimation()]
})
export class EditCategoryComponent extends AppComponentBase implements OnInit {
  saving = false;
  id: number;
  category = new CategoryInput();
  permissions: PermissionDto[] = [];
  checkedPermissionMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;
  subCategoryFormArray = new FormArray([]);

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _router: Router,
    private router: ActivatedRoute,
    private _categoryService: CategoryServiceProxy,
    private appMain: AppComponent
  ) { 
    super(injector);
  }

  ngOnInit(): void {
    this.router.params.subscribe(params => {
      this.id = params['id']
    });
    this._categoryService.getCategoryForEdit(this.id.toString())
    .subscribe((result: CategoryOutputDto) => {
      this.category.categoryCode = result.categoryCode;
      this.category.categoryName = result.categoryName;
      this.category.description = result.description;
      result.subCategories.forEach(element => {
          this.subCategoryFormArray.push(new FormControl(element));
      });
    }) 
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
    
    this._categoryService.update(categoryAdd).subscribe(
      () => {
        this.appMain.showSuccessMessage('Cập nhật thành công', 'Câp nhật danh mục thành công');
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
}
