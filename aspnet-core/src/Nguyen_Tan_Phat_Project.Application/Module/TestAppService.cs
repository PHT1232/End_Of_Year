using Abp.Authorization;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.TestManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;

namespace Nguyen_Tan_Phat_Project.Module
{
    public class TestAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<TestHoChiMinh> _testHoChiMinhRepository;

        public TestAppService(IRepository<User, long> userRepository, IRepository<Test> testRepository, IRepository<TestHoChiMinh> testHoChiMinhRepository)
        {
            _userRepository = userRepository;
            _testRepository = testRepository;
            _testHoChiMinhRepository = testHoChiMinhRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Test)]
        public async Task AddNewAsync(TestInput input)
        {
            if (input == null) throw new ArgumentNullException();
            var checkExist = await _testRepository.FirstOrDefaultAsync(e => e.TestVarible == input.testVarible);
            var checkExistForHoChiMinh = await _testHoChiMinhRepository.FirstOrDefaultAsync(e => e.TestVarible == input.testVarible);

            if (checkExist != null && checkExistForHoChiMinh != null)
                throw new UserFriendlyException("This varible already existed");

            if (checkExist == null && checkExistForHoChiMinh == null)
            {
                try
                {
                    await _testRepository.InsertAsync(new Test
                    {
                        TestVarible = input.testVarible,
                    });

                    await _testHoChiMinhRepository.InsertAsync(new TestHoChiMinh
                    {
                        TestVarible = input.testVarible,
                    });
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            }

            if (checkExist == null && checkExistForHoChiMinh != null)
            {
                try
                {
                    await _testRepository.InsertAsync(new Test
                    {
                        TestVarible = input.testVarible,
                    });
                } catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            } else
            {
                try
                {
                    await _testHoChiMinhRepository.InsertAsync(new TestHoChiMinh
                    {
                        TestVarible = input.testVarible,
                    });
                } catch (Exception ex) 
                {
                throw new UserFriendlyException(ex.Message);
                }
            }
        }  
        
        [AbpAuthorize(PermissionNames.Page_System_Test_Add)]
        public async Task GetAsync(TestPagedResultInput input)
        {

        }
    }
}
