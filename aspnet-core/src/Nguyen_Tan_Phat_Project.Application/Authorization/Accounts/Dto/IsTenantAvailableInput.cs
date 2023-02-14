using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace Nguyen_Tan_Phat_Project.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}
