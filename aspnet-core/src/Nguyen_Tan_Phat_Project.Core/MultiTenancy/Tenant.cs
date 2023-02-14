using Abp.MultiTenancy;
using Nguyen_Tan_Phat_Project.Authorization.Users;

namespace Nguyen_Tan_Phat_Project.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
