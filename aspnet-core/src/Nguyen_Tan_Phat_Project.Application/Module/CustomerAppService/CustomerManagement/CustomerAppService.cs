using Abp.Authorization;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.CustomerAppService.CustomerManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Nguyen_Tan_Phat_Project.Module.CustomerAppService.CustomerManagement
{
    public class CustomerAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
<<<<<<< Updated upstream
        private IRepository<Customer, string> _customerRepository;

        public CustomerAppService(IRepository<Customer, string> customerRepository)
=======
        private IRepository<OrgCustomer, string> _customerRepository;
        private IRepository<BankAccount> _bankRepository;

        public CustomerAppService(IRepository<OrgCustomer, string> customerRepository
            , IRepository<BankAccount> bankRepository
            )
>>>>>>> Stashed changes
        {
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Customer_Add)]
        public async Task AddNewAsync(CustomerInputDto input)
        {
            try
            {
<<<<<<< Updated upstream
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.CustomerCode);
                if (customer == null)
=======
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.OrgCustomerCode);
                if (customer != null)
>>>>>>> Stashed changes
                {
                    throw new UserFriendlyException("Đã tồn tại khách hàng này");
                }

                DateTime creationTime = DateTime.Now;
                await _customerRepository.InsertAsync(new Customer
                {
                    Id = input.CustomerCode,
                    TaxIdentification = input.TaxIdentification,
                    CustomerName = input.CustomerName,
                    CustomerEmail = input.CustomerEmail,
                    CustomerPhone = input.CustomerPhone,
                    CustomerAddress = input.CustomerAddress,
                    CustomerWebsite = input.CustomerWebsite,
                    CustomerDescription = input.CustomerDescription,
                    BankAccount = input.BankAccount,
                });
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Customer_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _customerRepository.HardDeleteAsync(e => e.Id == id);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Customer_Update)]
        public async Task UpdateAsync(CustomerInputDto input)
        {
            try
            {
                var customerUpdateCheck = await _customerRepository.FirstOrDefaultAsync(x => x.CustomerName == input.CustomerName && x.Id != input.CustomerCode);
                if (customerUpdateCheck != null)
                    throw new UserFriendlyException("Đã có khách hàng với tên này");

                var customerDto = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.CustomerCode);
                if (customerDto != null)
                    throw new UserFriendlyException("Không thể tìm thấy khách hàng này");

                customerDto.CustomerName = input.CustomerName;
                customerDto.TaxIdentification = input.TaxIdentification;
                customerDto.CustomerEmail = input.CustomerEmail;
                customerDto.CustomerPhone = input.CustomerPhone;
                customerDto.CustomerAddress = input.CustomerAddress;
                customerDto.CustomerWebsite = input.CustomerWebsite;
                customerDto.CustomerDescription = input.CustomerDescription;
                customerDto.BankAccount = input.BankAccount;

                await _customerRepository.UpdateAsync(customerDto);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<CustomerGetAllDto>> GetAllAsync(CustomerPagedResultInput input)
        {
            try
            {
                var query = await _customerRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.KeyWord), e => e.CustomerName == input.KeyWord || e.Id == input.KeyWord)
                    .Select(e => new CustomerGetAllDto
                    {
                        CustomerCode = e.Id,
                        CustomerName = e.CustomerName,
                        CustomerAddress = e.CustomerAddress,
                        CustomerWebsite = e.CustomerWebsite,
                        CustomerPhone = e.CustomerPhone,
                        CustomerBankAccount = e.BankAccount,
                    }).PageBy(input).ToListAsync();

                int totalCount = _customerRepository.Count();

                return new PagedResultDto<CustomerGetAllDto> 
                {
                    Items = query, 
                    TotalCount = totalCount 
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CustomerOutputDto> GetAsync(string id)
        {
            var query = await _customerRepository.GetAsync(id);
            if (query == null)
            {
                throw new UserFriendlyException("Không thể tìm thấy nhân viên với mã này");
            }


            var customerOutput = new CustomerOutputDto
            {
<<<<<<< Updated upstream
                CustomerCode = query.Id,
                CustomerName = query.CustomerName,
                CustomerAddress = query.CustomerAddress,
                CustomerWebsite = query.CustomerWebsite,
                CustomerPhone = query.CustomerPhone,
                CustomerDescription = query.CustomerDescription,
                CustomerEmail = query.CustomerEmail,
                BankAccount = query.BankAccount,
=======
                OrgCustomerCode = query.Id,
                OrgCustomerName = query.OrgCustomerName,
                OrgCustomerAddress = query.OrgCustomerAddress,
                OrgCustomerWebsite = query.OrgCustomerWebsite,
                OrgCustomerPhone = query.OrgCustomerPhone,
                OrgCustomerDescription = query.OrgCustomerDescription,
                OrgCustomerEmail = query.OrgCustomerEmail,
                BankAccount = _bankRepository.GetAll().FirstOrDefault(b => b.BankId == query.BankId),
>>>>>>> Stashed changes
                TaxIdentification = query.TaxIdentification,
            };

            return customerOutput;
        }
    }
}
