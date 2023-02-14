using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    public abstract class Nguyen_Tan_Phat_ProjectControllerBase: AbpController
    {
        protected Nguyen_Tan_Phat_ProjectControllerBase()
        {
            LocalizationSourceName = Nguyen_Tan_Phat_ProjectConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
