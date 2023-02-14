using Abp.Application.Services;
using Nguyen_Tan_Phat_Project.MultiTenancy.Dto;

namespace Nguyen_Tan_Phat_Project.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

