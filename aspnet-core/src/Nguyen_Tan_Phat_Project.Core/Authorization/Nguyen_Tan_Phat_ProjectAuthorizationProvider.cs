﻿using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Nguyen_Tan_Phat_Project.Authorization
{
    public class Nguyen_Tan_Phat_ProjectAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            var system = context.CreatePermission(PermissionNames.Pages_System, L("System"));
            var test = system.CreateChildPermission(PermissionNames.Page_System_Test, L("Tests"));
            test.CreateChildPermission(PermissionNames.Page_System_Test_View, L("View"));
            test.CreateChildPermission(PermissionNames.Page_System_Test_Add, L("Add"));
            test.CreateChildPermission(PermissionNames.Page_System_Test_Delete, L("Delete"));

            var storage = system.CreateChildPermission(PermissionNames.Page_System_Storage, L("Storage"));
            storage.CreateChildPermission(PermissionNames.Page_System_Storage_Update, L("Update"));
            storage.CreateChildPermission(PermissionNames.Page_System_Storage_View, L("View"));
            storage.CreateChildPermission(PermissionNames.Page_System_Storage_Add, L("Add"));
            storage.CreateChildPermission(PermissionNames.Page_System_Storage_Delete, L("Delete"));

            var category = system.CreateChildPermission(PermissionNames.Page_System_Category, L("Category"));
            category.CreateChildPermission(PermissionNames.Page_System_Category_Update, L("Update"));
            category.CreateChildPermission(PermissionNames.Page_System_Category_View, L("View"));
            category.CreateChildPermission(PermissionNames.Page_System_Category_Add, L("Add"));
            category.CreateChildPermission(PermissionNames.Page_System_Category_Delete, L("Delete"));

            var product = system.CreateChildPermission(PermissionNames.Page_System_Product, L("Product"));
            product.CreateChildPermission(PermissionNames.Page_System_Product_Update, L("Update"));
            product.CreateChildPermission(PermissionNames.Page_System_Product_View, L("View"));
            product.CreateChildPermission(PermissionNames.Page_System_Product_Add, L("Add"));
            product.CreateChildPermission(PermissionNames.Page_System_Product_Delete, L("Delete"));

            var exportImport = system.CreateChildPermission(PermissionNames.Page_System_Export_Import, L("ExportImport"));
            exportImport.CreateChildPermission(PermissionNames.Page_System_Export_Import_Update, L("Update"));
            exportImport.CreateChildPermission(PermissionNames.Page_System_Export_Import_View, L("View"));
            exportImport.CreateChildPermission(PermissionNames.Page_System_Export_Import_Add, L("Add"));
            exportImport.CreateChildPermission(PermissionNames.Page_System_Export_Import_Delete, L("Delete"));

            var structure = system.CreateChildPermission(PermissionNames.Page_System_Structure, L("Structure"));
            structure.CreateChildPermission(PermissionNames.Page_System_Structure_Update, L("Update"));
            structure.CreateChildPermission(PermissionNames.Page_System_Structure_View, L("View"));
            structure.CreateChildPermission(PermissionNames.Page_System_Structure_Add, L("Add"));
            structure.CreateChildPermission(PermissionNames.Page_System_Structure_Delete, L("Delete"));          
            
            var employee = system.CreateChildPermission(PermissionNames.Page_System_Employee, L("Employee"));
            employee.CreateChildPermission(PermissionNames.Page_System_Employee_Update, L("Update"));
            employee.CreateChildPermission(PermissionNames.Page_System_Employee_View, L("View"));
            employee.CreateChildPermission(PermissionNames.Page_System_Employee_Add, L("Add"));
            employee.CreateChildPermission(PermissionNames.Page_System_Employee_Delete, L("Delete"));

            var customer = system.CreateChildPermission(PermissionNames.Page_System_Customer, L("Customer"));
            customer.CreateChildPermission(PermissionNames.Page_System_Customer_Update, L("Update"));
            customer.CreateChildPermission(PermissionNames.Page_System_Customer_View, L("View"));
            customer.CreateChildPermission(PermissionNames.Page_System_Customer_Add, L("Add"));
            customer.CreateChildPermission(PermissionNames.Page_System_Customer_Delete, L("Delete"));

            var revenue = system.CreateChildPermission(PermissionNames.Page_System_Revenue, L("Revenue"));
            revenue.CreateChildPermission(PermissionNames.Page_System_Revenue_Update, L("Update"));
            revenue.CreateChildPermission(PermissionNames.Page_System_Revenue_View, L("View"));
            revenue.CreateChildPermission(PermissionNames.Page_System_Revenue_Add, L("Add"));
            revenue.CreateChildPermission(PermissionNames.Page_System_Revenue_Delete, L("Delete"));

            var summary = system.CreateChildPermission(PermissionNames.Page_System_Summary, L("Summary"));
            summary.CreateChildPermission(PermissionNames.Page_System_Summary_Update, L("Update"));
            summary.CreateChildPermission(PermissionNames.Page_System_Summary_View, L("View"));
            summary.CreateChildPermission(PermissionNames.Page_System_Summary_Add, L("Add"));
            summary.CreateChildPermission(PermissionNames.Page_System_Summary_Delete, L("Delete"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, Nguyen_Tan_Phat_ProjectConsts.LocalizationSourceName);
        }
    }
}