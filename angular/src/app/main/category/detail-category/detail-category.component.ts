import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { CategoryInput, CategoryOutputDto, CategoryServiceProxy, PermissionDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-detail-category',
  templateUrl: './detail-category.component.html',
  styleUrls: ['./detail-category.component.css'],
  animations: [appModuleAnimation()]
})
export class DetailCategoryComponent extends AppComponentBase implements OnInit {
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
    private _categoryService: CategoryServiceProxy
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

  Cancel(): void {
    this._router.navigate(['app/category']);
  }

}
