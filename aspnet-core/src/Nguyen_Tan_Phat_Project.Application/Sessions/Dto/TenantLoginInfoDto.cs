using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nguyen_Tan_Phat_Project.MultiTenancy;

namespace Nguyen_Tan_Phat_Project.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
