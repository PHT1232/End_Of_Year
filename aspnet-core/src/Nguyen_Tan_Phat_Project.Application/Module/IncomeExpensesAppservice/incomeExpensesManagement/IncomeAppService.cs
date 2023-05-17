using Abp.Authorization;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Revenue_View)]
    public class IncomeAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
    }
}
