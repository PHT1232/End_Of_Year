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
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Modules;

namespace Nguyen_Tan_Phat_Project.Module
{
    [AbpAuthorize(PermissionNames.Page_System_Test_View)]
    public class TestAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Test> _testRepository;

        public TestAppService(IRepository<User, long> userRepository, IRepository<Test> testRepository)
        {
            _userRepository = userRepository;
            _testRepository = testRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Test_Add)]
        public async Task AddNewAsync(TestInput input)
        {
            try
            {
                var testDto = await _testRepository.FirstOrDefaultAsync(e => e.TestVarible == input.testVarible);

                if (testDto != null)
                {
                    throw new UserFriendlyException("This test variable already exist!");
                }

                await _testRepository.InsertAsync(new Test
                {
                    TestVarible = input.testVarible,
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Test_Delete)]
        public async Task DeleteAsync(int id)
        {
            try
            {
                await _testRepository.DeleteAsync(e => e.TestVarible == id);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<TestDto>> GetAllAsync(TestPagedResultInput input)
        {
            if (input == null) throw new ArgumentNullException();

            try
            {
                var test = await _testRepository.GetAll().Select(e => new TestDto
                {
                    testVarible = e.TestVarible,
                }).PageBy(input).ToListAsync();
                return new PagedResultDto<TestDto>
                {
                    Items = test,
                    TotalCount = _testRepository.Count()
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
