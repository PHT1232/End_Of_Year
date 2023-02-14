using System.Threading.Tasks;
using Abp.Application.Services;
using Nguyen_Tan_Phat_Project.Authorization.Accounts.Dto;

namespace Nguyen_Tan_Phat_Project.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
