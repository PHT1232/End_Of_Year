using System.Threading.Tasks;
using Abp.Application.Services;
using Nguyen_Tan_Phat_Project.Sessions.Dto;

namespace Nguyen_Tan_Phat_Project.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
