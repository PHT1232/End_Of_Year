using Abp.Authorization;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module
{
    public class TestAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;

        public TestAppService(IRepository<User, long> userRepository)
        {
            _userRepository = userRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Test)]
        public async Task AddNewAsync(int id)
        {

        }
    }
}
