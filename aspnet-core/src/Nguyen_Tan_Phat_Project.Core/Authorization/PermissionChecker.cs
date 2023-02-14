using Abp.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;

namespace Nguyen_Tan_Phat_Project.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
