﻿using Abp.Authorization;
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
        private IRepository<OrgCustomer, string> _customerRepository;

        public CustomerAppService(IRepository<OrgCustomer, string> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Customer_Add)]
        public async Task AddNewAsync(CustomerInputDto input)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.OrgCustomerCode);
                if (customer == null)
                {
                    throw new UserFriendlyException("Đã tồn tại khách hàng này");
                }

                DateTime creationTime = DateTime.Now;
                await _customerRepository.InsertAsync(new OrgCustomer
                {
                    Id = input.OrgCustomerCode,
                    TaxIdentification = input.TaxIdentification,
                    OrgCustomerName = input.OrgCustomerName,
                    OrgCustomerEmail = input.OrgCustomerEmail,
                    OrgCustomerPhone = input.OrgCustomerPhone,
                    OrgCustomerAddress = input.OrgCustomerAddress,
                    OrgCustomerWebsite = input.OrgCustomerWebsite,
                    OrgCustomerDescription = input.OrgCustomerDescription,
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
                var customerUpdateCheck = await _customerRepository.FirstOrDefaultAsync(x => x.OrgCustomerName == input.OrgCustomerName && x.Id != input.OrgCustomerCode);
                if (customerUpdateCheck != null)
                    throw new UserFriendlyException("Đã có khách hàng với tên này");

                var customerDto = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.OrgCustomerCode);
                if (customerDto != null)
                    throw new UserFriendlyException("Không thể tìm thấy khách hàng này");

                customerDto.OrgCustomerName = input.OrgCustomerName;
                customerDto.TaxIdentification = input.TaxIdentification;
                customerDto.OrgCustomerEmail = input.OrgCustomerEmail;
                customerDto.OrgCustomerPhone = input.OrgCustomerPhone;
                customerDto.OrgCustomerAddress = input.OrgCustomerAddress;
                customerDto.OrgCustomerWebsite = input.OrgCustomerWebsite;
                customerDto.OrgCustomerDescription = input.OrgCustomerDescription;
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
                    .WhereIf(!string.IsNullOrEmpty(input.KeyWord), e => e.OrgCustomerName == input.KeyWord || e.Id == input.KeyWord)
                    .Select(e => new CustomerGetAllDto
                    {
                        OrgCustomerCode = e.Id,
                        OrgCustomerName = e.OrgCustomerName,
                        OrgCustomerAddress = e.OrgCustomerAddress,
                        OrgCustomerWebsite = e.OrgCustomerWebsite,
                        OrgCustomerPhone = e.OrgCustomerPhone,
                        OrgCustomerBankAccount = e.BankAccount,
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
                OrgCustomerCode = query.Id,
                OrgCustomerName = query.OrgCustomerName,
                OrgCustomerAddress = query.OrgCustomerAddress,
                OrgCustomerWebsite = query.OrgCustomerWebsite,
                OrgCustomerPhone = query.OrgCustomerPhone,
                OrgCustomerDescription = query.OrgCustomerDescription,
                OrgCustomerEmail = query.OrgCustomerEmail,
                BankAccount = query.BankAccount,
                TaxIdentification = query.TaxIdentification,
            };

            return customerOutput;
        }
    }
}
