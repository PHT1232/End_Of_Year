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
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<BankAccount> _bankRepository;
        private readonly IRepository<Structure, string> _structureRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;

        public CustomerAppService(IRepository<Customer, string> customerRepository
            , IRepository<BankAccount> bankRepository
            , IRepository<Structure, string> structureRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            )
        {
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _structureRepository = structureRepository;
            _retailCustomerRepository = retailCustomerRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Customer_Add)]
        public async Task AddNewAsync(CustomerInputDto input)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.CustomerCode);
                if (customer != null)
                {
                    throw new UserFriendlyException("Đã tồn tại khách hàng này");
                }

                DateTime creationTime = DateTime.Now;
                
                if (input.BankAccount.BankId == null)
                {
                    customer = new Customer
                    {
                        Id = input.CustomerCode,
                        TaxIdentification = input.TaxIdentification,
                        CustomerName = input.CustomerName,
                        CustomerEmail = input.CustomerEmail,
                        CustomerPhone = input.CustomerPhone,
                        CustomerAddress = input.CustomerAddress,
                        CustomerWebsite = input.CustomerWebsite,
                        StructureCode = input.StructureCode,
                        CustomerDescription = input.CustomerDescription,
                        Discount = input.Discount
                    };
                } else
                {
                    customer = new Customer
                    {
                        Id = input.CustomerCode,
                        TaxIdentification = input.TaxIdentification,
                        CustomerName = input.CustomerName,
                        CustomerEmail = input.CustomerEmail,
                        CustomerPhone = input.CustomerPhone,
                        CustomerAddress = input.CustomerAddress,
                        StructureCode = input.StructureCode,
                        CustomerWebsite = input.CustomerWebsite,
                        CustomerDescription = input.CustomerDescription,
                        BankAccount = input.BankAccount,
                        Discount = input.Discount
                    };
                }

                await _customerRepository.InsertAsync(customer);

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
                if (customerDto == null)
                    throw new UserFriendlyException("Không thể tìm thấy khách hàng này");

                customerDto.CustomerName = input.CustomerName;
                customerDto.TaxIdentification = input.TaxIdentification;
                customerDto.CustomerEmail = input.CustomerEmail;
                customerDto.CustomerPhone = input.CustomerPhone;
                customerDto.CustomerAddress = input.CustomerAddress;
                customerDto.CustomerWebsite = input.CustomerWebsite;
                customerDto.StructureCode = input.StructureCode;
                customerDto.CustomerDescription = input.CustomerDescription;
                customerDto.Discount = input.Discount;

                var customerBankAccount = await _bankRepository.FirstOrDefaultAsync(e => e.BankId == input.BankAccount.BankId);
                if (customerBankAccount == null || customerBankAccount.BankId != input.BankAccount.BankId)
                {
                    await _bankRepository.InsertAsync(input.BankAccount);
                    customerDto.BankId = input.BankAccount.BankId;
                    await _customerRepository.UpdateAsync(customerDto);
                }

                //await _customerRepository.UpdateAsync(customerDto);
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
                        CustomerBankId = e.BankAccount.BankId,
                        StructureName = _structureRepository.GetAll().FirstOrDefault(x => x.Id == e.StructureCode).UnitName,
                        CustomerBankName = e.BankAccount.BankName,
                        Discount = e.Discount,
                    }).ToListAsync();

                
                List<CustomerGetAllDto> result = new List<CustomerGetAllDto>();
                foreach (var item in query)
                {
                    var retailCustomer = _retailCustomerRepository.FirstOrDefault(e => e.CustomerCode == item.CustomerCode);
                    if (!input.isRetail)
                    {
                        if (retailCustomer == null)
                        {
                            result.Add(item);
                        }
                    } else
                    {
                        if (retailCustomer != null)
                        {
                            result.Add(item);
                        }
                    }
                }

                int totalCount = _customerRepository.Count();

                return new PagedResultDto<CustomerGetAllDto> 
                {
                    Items = result, 
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
                throw new UserFriendlyException("Không thể tìm thấy Khách hàng với mã này");
            }

            var customerOutput = new CustomerOutputDto
            {
                CustomerCode = query.Id,
                CustomerName = query.CustomerName,
                TaxIdentification = query.TaxIdentification,
                CustomerAddress = query.CustomerAddress,
                CustomerWebsite = query.CustomerWebsite,
                CustomerPhone = query.CustomerPhone,
                CustomerDescription = query.CustomerDescription,
                CustomerEmail = query.CustomerEmail,
                StructureCode = query.StructureCode,
                BankAccount = _bankRepository.GetAll().FirstOrDefault(e => e.BankId == query.BankId),
                Discount = query.Discount,
            };

            return customerOutput;
        }
    }
}
