using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement
{
    public class RetailAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Retail, string> _retailRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;
        private readonly IRepository<RetailProduct> _retailProductRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public RetailAppService(IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Product, string> productRepository
            , IRepository<Retail, string> retailRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            , IRepository<RetailProduct> retailProductRepository
            , IRepository<Customer, string> customerRepository
            )
        {
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
            _retailRepository = retailRepository;
            _retailProductRepository = retailProductRepository;
            _retailCustomerRepository = retailCustomerRepository;
            _customerRepository = customerRepository;
        }

        public async Task AddNewAsync(RetailInputDto input)
        {
            try
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == input.RetailCode);
                if (retail != null)
                {
                    throw new UserFriendlyException("Đơn này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                retail = new Retail
                {
                    Id = input.RetailCode,
                    StructureId = input.StructureId,
                    NameOfReceiver = input.Customer.CustomerName,
                    OrderCreator = input.OrderCreator,
                    PaymentMethod = input.PaymentMethod,
                    DeliveryEmployee = input.DeliveryEmployee,
                    OrderStatus = 1,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                    Discount = input.Discount,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _retailRepository.InsertAndGetIdAsync(retail);

                var customerRetail = new RetailCustomer
                {
                    RetailCode = id,
                    CustomerCode = input.Customer.CustomerCode,
                    ReciveAddress = input.Customer.ReveciveAddress,
                    Discount = input.Discount,
                    PhoneToCall = input.Customer.PhoneToCall,
                };

                var customerCheck = await _customerRepository.FirstOrDefaultAsync(e => e.Id == input.Customer.CustomerCode);
                if (customerCheck == null)
                {
                    var customer = new Customer
                    {
                        Id = input.Customer.CustomerCode,
                        CustomerName = input.Customer.CustomerName,
                        StructureCode = input.StructureId,
                        CustomerAddress = input.Customer.ReveciveAddress,
                        CustomerPhone = input.Customer.PhoneToCall,
                    };

                    await _customerRepository.InsertAsync(customer);
                }

                await _retailCustomerRepository.InsertAsync(customerRetail);

                foreach (var product in input.Products)
                {
                    var retailProduct = new RetailProduct
                    {
                        RetailId = id,
                        StorageId = product.StorageId,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                    };
                    _retailProductRepository.Insert(retailProduct);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<RetailProductDto>> GetProductAsync(ProductPagedRequest input)
        {
            try
            {
                var exportImport = new List<RetailProductDto>();


                var productStorage = await _productStorageRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.ProductId.Contains(input.Keyword))
                    .Where(e => e.StorageId == input.StorageId)
                    .PageBy(input).ToListAsync();

                List<RetailProductDto> result = new List<RetailProductDto>();
                foreach (var storageProduct in productStorage)
                {
                    var productFullQuantity = _productStorageRepository.GetAll().Where(e => e.ProductId == storageProduct.ProductId).Select(L => L.ProductQuantity).Sum();

                    var product = _productRepository.FirstOrDefault(e => e.Id == storageProduct.ProductId);

                    var productDto = new RetailProductDto
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Quantity = productFullQuantity,
                        Price = product.Price,
                        Unit = product.Unit,
                        FinalPrice = product.Price * productFullQuantity
                    };
                    result.Add(productDto);
                }

                exportImport = result;

                int totalCount = await _productStorageRepository.CountAsync();

                return new PagedResultDto<RetailProductDto>
                {
                    Items = exportImport,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<RetailOutputDto> GetAsync(string id)
        {
            try 
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == id);
                var retailCustomer = await _retailCustomerRepository.FirstOrDefaultAsync(e => e.RetailCode == id);
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id.Equals(retailCustomer.CustomerCode));
                if (retail == null)
                {
                    throw new UserFriendlyException("Không thể tìm đơn này");
                }

                var product = await _retailProductRepository.GetAll().Include(e => e.Product)
                    .Where(l => l.RetailId == id)
                    .Select(l => new RetailProductDto
                    {
                        RetailId = l.RetailId,
                        ProductId = l.ProductId,
                        StorageId = l.StorageId,
                        ProductName = l.Product.ProductName,
                        Quantity = l.Quantity,
                        Price = l.Price,
                        Unit = l.Product.Unit,
                        FinalPrice = l.FinalPrice,
                    }).ToListAsync();

                var customerDto = new CustomerDto();

                if (customer != null)
                {
                    customerDto.CustomerCode = customer.Id;
                    customerDto.CustomerName = customer.CustomerName;
                    customerDto.CustomerPhone = customer.CustomerPhone;
                }

                RetailOutputDto outputDto = new RetailOutputDto
                {
                    RetailCode = retail.Id,
                    OrderCreator = _employeeRepository.GetAll().FirstOrDefault(e => e.Id == retail.OrderCreator).EmployeeName,
                    OrderStatus = retail.OrderStatus,
                    Customer = customerDto,
                    Products = product,
                    PaymentMethod = retail.PaymentMethod,
                    TotalPrice = retail.TotalPrice,
                };

                return outputDto;
            } catch (Exception ex) {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<RetailGetAllDto>> GetAllAsync(RetailPagedResultInput input)
        {
            try
            {
                var retail = new List<RetailGetAllDto>();
                int totalCount = 0;
                if (input.Keyword != null)
                {
                    retail = await _retailRepository.GetAll()
                       .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id.Contains(input.Keyword))
                       .WhereIf(!string.IsNullOrEmpty(input.Structure), e => e.StructureId.Contains(input.Structure))
                       .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                       .Where(e => e.IsDelivered == input.isDelived)
                       .PageBy(input).Select(e => new RetailGetAllDto
                       {
                           RetailCode = e.Id,
                           NameOfReceiver = e.NameOfReceiver,
                           Address = _retailCustomerRepository.GetAll().FirstOrDefault(c => c.RetailCode == e.Id).ReciveAddress,
                           OrderStatus = e.OrderStatus,
                           PaymentMethod = e.PaymentMethod,
                           TotalPrice = e.TotalPrice,
                           StructureId = e.StructureId,
                           IsDelivered = e.IsDelivered,
                           CreationTime = e.CreationTime,
                           OrderCreator = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                       }).ToListAsync();

                    totalCount = await _retailRepository.GetAll()
                       .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword)
                       .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                       .CountAsync();
                }
                else
                {
                    if (input.DateTime != null)
                    {
                        var firstDate = new DateTime();
                        var endDate = new DateTime();

                        firstDate = DateTime.Parse(input.DateTime[0]);
                        endDate = DateTime.Parse(input.DateTime[1]);

                        retail = await _retailRepository.GetAll()
                            .Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate && e.OrderStatus == input.OrderStatus)
                            .WhereIf(!string.IsNullOrEmpty(input.Structure), e => e.StructureId.Contains(input.Structure))
                            .PageBy(input).Select(e => new RetailGetAllDto
                            {
                                RetailCode = e.Id,
                                NameOfReceiver = e.NameOfReceiver,
                                Address = _retailCustomerRepository.GetAll().FirstOrDefault(c => c.RetailCode == e.Id).ReciveAddress,
                                OrderStatus = e.OrderStatus,
                                PaymentMethod = e.PaymentMethod,
                                TotalPrice = e.TotalPrice,
                                StructureId = e.StructureId,
                                IsDelivered = e.IsDelivered,
                                CreationTime = e.CreationTime,
                                OrderCreator = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                            }).ToListAsync();
                        totalCount = await _retailRepository.GetAll().Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate && e.OrderStatus == input.OrderStatus).CountAsync();
                    }
                    else
                    {
                        retail = await _retailRepository.GetAll()
                           .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                           .WhereIf(!string.IsNullOrEmpty(input.Structure), e => e.StructureId.Contains(input.Structure))
                           .PageBy(input).Select(e => new RetailGetAllDto
                           {
                               RetailCode = e.Id,
                               NameOfReceiver = e.NameOfReceiver,
                               Address = _retailCustomerRepository.GetAll().FirstOrDefault(c => c.RetailCode == e.Id).ReciveAddress,
                               OrderStatus = e.OrderStatus,
                               PaymentMethod = e.PaymentMethod,
                               TotalPrice = e.TotalPrice,
                               StructureId = e.StructureId,
                               IsDelivered = e.IsDelivered,
                               CreationTime = e.CreationTime,
                               OrderCreator = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                           }).ToListAsync();
                        totalCount = await _retailRepository.GetAll().WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus).CountAsync();
                    }
                }

                return new PagedResultDto<RetailGetAllDto>
                {
                    Items = retail,
                    TotalCount = totalCount,
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAllowAnonymous]
        public async Task UpdateOrderAsync(RetailInputDto input)
        {
            try
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == input.RetailCode);

                if (input.OrderStatus == 2)
                {
                    var retailProduct = await _retailProductRepository.GetAll()
                        .Where(e => e.RetailId == input.RetailCode)
                        .ToListAsync();
                    
                    retail.OrderStatus = input.OrderStatus;
                    await _retailRepository.UpdateAsync(retail);

                    if (!input.IsDelivered)
                    {
                        foreach (var productExport in retailProduct)
                        {
                            var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == retail.StructureId && e.ProductId == productExport.ProductId);
                            if (product != null)
                            {
                                product.ProductQuantity -= productExport.Quantity;
                                _productStorageRepository.Update(product);
                            }
                        }
                    }
                    else
                    {

                        foreach (var productExport in retailProduct)
                        {
                            var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                            if (product != null)
                            {
                                product.ProductQuantity -= productExport.Quantity;
                                _productStorageRepository.Update(product);
                            }
                        }
                    }
                }
                else if (input.OrderStatus == 3)
                {
                    retail.OrderStatus = input.OrderStatus;
                    await _retailRepository.UpdateAsync(retail);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAllowAnonymous]
        [DisableCors]
        [DisableAuditing]
        [HttpGet]
        public async Task<ContentResult> UpdateOrderQRAsync(RetailInputDto input)
        {
            try
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == input.RetailCode);
                if (input.OrderStatus == 2)
                {
                    var retailProduct = await _retailProductRepository.GetAll()
                        .Where(e => e.RetailId == retail.Id)
                        .ToListAsync();
                    retail.OrderStatus = input.OrderStatus;
                    await _retailRepository.UpdateAsync(retail);

                    foreach (var productExport in retailProduct)
                    {
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == retail.StructureId && e.ProductId == productExport.ProductId);
                        if (product != null)
                        {
                            product.ProductQuantity -= productExport.Quantity;
                            _productStorageRepository.Update(product);
                        }
                    }
                }

                else if (input.OrderStatus == 3)
                {
                    retail.OrderStatus = input.OrderStatus;
                    await _retailRepository.UpdateAsync(retail);
                }

                var html = "<html>" +
                    "<head>" +
                    "<meta charset=\"utf-8\" />" +
                    "<style>" +
                    ".success-checkmark {\r\n        width: 80px;\r\n        height: 115px;\r\n        margin: 0 auto;\r\n\r\n        .check-icon {\r\n          width: 80px;\r\n          height: 80px;\r\n          position: relative;\r\n          border-radius: 50%;\r\n          box-sizing: content-box;\r\n          border: 4px solid #4caf50;\r\n\r\n          &::before {\r\n            top: 3px;\r\n            left: -2px;\r\n            width: 30px;\r\n            transform-origin: 100% 50%;\r\n            border-radius: 100px 0 0 100px;\r\n          }\r\n\r\n          &::after {\r\n            top: 0;\r\n            left: 30px;\r\n            width: 60px;\r\n            transform-origin: 0 50%;\r\n            border-radius: 0 100px 100px 0;\r\n            animation: rotate-circle 4.25s ease-in;\r\n          }\r\n\r\n          &::before,\r\n          &::after {\r\n            content: \"\";\r\n            height: 100px;\r\n            position: absolute;\r\n            background: #ffffff;\r\n            transform: rotate(-45deg);\r\n          }\r\n\r\n          .icon-line {\r\n            height: 5px;\r\n            background-color: #4caf50;\r\n            display: block;\r\n            border-radius: 2px;\r\n            position: absolute;\r\n            z-index: 10;\r\n\r\n            &.line-tip {\r\n              top: 46px;\r\n              left: 14px;\r\n              width: 25px;\r\n              transform: rotate(45deg);\r\n              animation: icon-line-tip 0.75s;\r\n            }\r\n\r\n            &.line-long {\r\n              top: 38px;\r\n              right: 8px;\r\n              width: 47px;\r\n              transform: rotate(-45deg);\r\n              animation: icon-line-long 0.75s;\r\n            }\r\n          }\r\n\r\n          .icon-circle {\r\n            top: -4px;\r\n            left: -4px;\r\n            z-index: 10;\r\n            width: 80px;\r\n            height: 80px;\r\n            border-radius: 50%;\r\n            position: absolute;\r\n            box-sizing: content-box;\r\n            border: 4px solid rgba(76, 175, 80, 0.5);\r\n          }\r\n\r\n          .icon-fix {\r\n            top: 8px;\r\n            width: 5px;\r\n            left: 26px;\r\n            z-index: 1;\r\n            height: 85px;\r\n            position: absolute;\r\n            transform: rotate(-45deg);\r\n            background-color: #ffffff;\r\n          }\r\n        }\r\n      }\r\n\r\n      @keyframes rotate-circle {\r\n        0% {\r\n          transform: rotate(-45deg);\r\n        }\r\n        5% {\r\n          transform: rotate(-45deg);\r\n        }\r\n        12% {\r\n          transform: rotate(-405deg);\r\n        }\r\n        100% {\r\n          transform: rotate(-405deg);\r\n        }\r\n      }\r\n\r\n      @keyframes icon-line-tip {\r\n        0% {\r\n          width: 0;\r\n          left: 1px;\r\n          top: 19px;\r\n        }\r\n        54% {\r\n          width: 0;\r\n          left: 1px;\r\n          top: 19px;\r\n        }\r\n        70% {\r\n          width: 50px;\r\n          left: -8px;\r\n          top: 37px;\r\n        }\r\n        84% {\r\n          width: 17px;\r\n          left: 21px;\r\n          top: 48px;\r\n        }\r\n        100% {\r\n          width: 25px;\r\n          left: 14px;\r\n          top: 45px;\r\n        }\r\n      }\r\n\r\n      @keyframes icon-line-long {\r\n        0% {\r\n          width: 0;\r\n          right: 46px;\r\n          top: 54px;\r\n        }\r\n        65% {\r\n          width: 0;\r\n          right: 46px;\r\n          top: 54px;\r\n        }\r\n        84% {\r\n          width: 55px;\r\n          right: 0px;\r\n          top: 35px;\r\n        }\r\n        100% {\r\n          width: 47px;\r\n          right: 8px;\r\n          top: 38px;\r\n        }\r\n      }" +
                    "</style>" +
                    "</head>" +
                    "<body>" +
                    "<div class=\"success-checkmark\" style=\"margin-top: 50px\">" +
                    "<div class=\"check-icon\">" +
                    "<span class=\"icon-line line-tip\"></span>" +
                    "<span class=\"icon-line line-long\"></span>" +
                    "<div class=\"icon-circle\"></div>" +
                    "<div class=\"icon-fix\"></div>" +
                    "</div>" +
                    "</div>" +
                    "<center>" +
                    "<h2>Hoàn thành đơn</h2>" +
                    "</center>" +
                    "</body>" +
                    "</html>";

                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html"
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<RetailCustomerDto> GetCustomerAsync(string phoneNumber)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.CustomerPhone == phoneNumber);
                if (customer == null)
                    throw new UserFriendlyException("Không có khách hàng này trong hệ thống");

                var customerDto = new RetailCustomerDto
                {
                    PhoneToCall = customer.CustomerPhone,
                    ReveciveAddress = customer.CustomerAddress,
                    CustomerCode = customer.Id,
                    CustomerName = customer.CustomerName,
                };
                return customerDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
