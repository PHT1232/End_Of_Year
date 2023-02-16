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
        public async Task<PagedResultDto<TestDto>> GetAllAsync(TestPagedResultInput input)
        {
            if (input == null) throw new ArgumentNullException();
            
            if (input.DbContext == 1)
            {
                var test = await _testRepository.GetAll().Select(e => new TestDto
                {
                    testVarible = input.testVarible,
                    username = _userRepository.FirstOrDefault(v => v.UserName.Equals("admin"))
                }).PageBy(input).ToListAsync();
                return new PagedResultDto<TestDto>
                {
                    Items = test,
                    TotalCount = test.Count
                };
            }

            var testHoChiMinh = await _testHoChiMinhRepository.GetAll().Select(e => new TestDto
            {
                testVarible = input.testVarible,
                username = _userRepository.FirstOrDefault(v => v.UserName.Equals("admin"))
            }).PageBy(input).ToListAsync();
            return new PagedResultDto<TestDto>
            {
                Items = testHoChiMinh,
                TotalCount = testHoChiMinh.Count
            };
        }
    }
}
