import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { StorageComponent } from './main/storage/storage.component';
import { CreateStorageComponent } from './main/storage/create-storage/create-storage.component';
import { EditStorageComponent } from './main/storage/edit-storage/edit-storage.component';
import { CategoryComponent } from './main/category/category.component';
import { CreateCategoryComponent } from './main/category/create-category/create-category.component';
import { EditCategoryComponent } from './main/category/edit-category/edit-category.component';
import { DetailCategoryComponent } from './main/category/detail-category/detail-category.component';
import { ProductComponent } from './main/product/product.component';
import { CreateProductComponent } from './main/product/create-product/create-product.component';
import { EditProductComponent } from './main/product/edit-product/edit-product.component';
import { DetailProductComponent } from './main/product/detail-product/detail-product.component';
import { DetailStorageComponent } from './main/storage/detail-storage/detail-storage.component';
import { ExportImportComponent } from './main/export-import/export-import.component';
import { CreateExportImportComponent } from './main/export-import/create-export-import/create-export-import.component';
import { EditExportImportComponent } from './main/export-import/edit-export-import/edit-export-import.component';
import { DetailsExportImportComponent } from './main/export-import/details-export-import/details-export-import.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' }, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent, canActivate: [AppRouteGuard] },
                    { path: 'update-password', component: ChangePasswordComponent, canActivate: [AppRouteGuard] },
                    { path: 'storage', component: StorageComponent, data: { permission: 'Pages.System.Storage.View'} , canActivate: [AppRouteGuard] },
                    { path: 'storage/create', component: CreateStorageComponent, data: { permission: 'Pages.System.Storage.Add'} , canActivate: [AppRouteGuard] },
                    { path: 'storage/edit/:id', component: EditStorageComponent, data: { permission: 'Pages.System.Storage.Update'} , canActivate: [AppRouteGuard] },
                    { path: 'storage/detail/:id', component: DetailStorageComponent, data: { permission: 'Pages.System.Storage.View'} , canActivate: [AppRouteGuard] },
                    { path: 'category', component: CategoryComponent, data: { permission: 'Pages.System.Category.View'} , canActivate: [AppRouteGuard] },
                    { path: 'category/create', component: CreateCategoryComponent, data: { permission: 'Pages.System.Category.Add'} , canActivate: [AppRouteGuard] },
                    { path: 'category/edit/:id', component: EditCategoryComponent, data: { permission: 'Pages.System.Category.Update'} , canActivate: [AppRouteGuard] },
                    { path: 'category/detail/:id', component: DetailCategoryComponent, data: { permission: 'Pages.System.Category.View'} , canActivate: [AppRouteGuard] },
                    { path: 'product', component: ProductComponent, data: { permission: 'Pages.System.Product.View'} , canActivate: [AppRouteGuard] },
                    { path: 'product/create', component: CreateProductComponent, data: { permission: 'Pages.System.Product.Add'} , canActivate: [AppRouteGuard] },
                    { path: 'product/edit/:id', component: EditProductComponent, data: { permission: 'Pages.System.Product.Update'} , canActivate: [AppRouteGuard] },
                    { path: 'product/detail/:id', component: DetailProductComponent, data: { permission: 'Pages.System.Product.View'} , canActivate: [AppRouteGuard] },
                    { path: 'exportimport', component: ExportImportComponent, data: { permission: 'Pages.System.ExportImport.View'} , canActivate: [AppRouteGuard] },
                    { path: 'exportimport/create', component: CreateExportImportComponent, data: { permission: 'Pages.System.ExportImport.Add'} , canActivate: [AppRouteGuard] },
                    { path: 'exportimport/edit/:id', component: EditExportImportComponent, data: { permission: 'Pages.System.ExportImport.Update'} , canActivate: [AppRouteGuard] },
                    { path: 'exportimport/detail/:id', component: DetailsExportImportComponent, data: { permission: 'Pages.System.ExportImport.View'} , canActivate: [AppRouteGuard] }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
