using Abp.Authorization;
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
            test.CreateChildPermission(PermissionNames.Page_System_Test_Add, L("Add"));
            test.CreateChildPermission(PermissionNames.Page_System_Test_Delete, L("Delete"));
        
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, Nguyen_Tan_Phat_ProjectConsts.LocalizationSourceName);
        }
    }
}
