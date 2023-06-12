using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.ExcelExport.Dtos;
using Nguyen_Tan_Phat_Project.Module.ExcelExport;
using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.VariantTypes;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Export_Import_View)]
    public class ExportImportAppService : Nguyen_Tan_Phat_ProjectAppServiceBase, IExportImportAppService, ITransientDependency
    {
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;
        private readonly IRepository<Employee, string> _employeeRepository;
        private readonly IRepository<Structure, string> _structureRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;

        public ExportImportAppService(IRepository<Product, string> productRepository
            , IAppFolders appFolders
            , IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<Customer, string> customerRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<Structure, string> structureRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            )
        {
            _appFolders = appFolders;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _customerRepository = customerRepository;
            _exportImportCustomerRepository = exportImportCustomerRepository;
            _employeeRepository = employeeRepository;
            _structureRepository = structureRepository;
            _retailCustomerRepository = retailCustomerRepository;
        }

        public string GetRandomCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Add)]
        public async Task AddNewAsync(ExportImportInput input)
        {
            try
            {
                var exportImportDto = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == input.ExportImportCode);
                if (exportImportDto != null)
                {
                    throw new UserFriendlyException("Đơn này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                var exportImport = new ExportImport();
                exportImport = new ExportImport
                {
                    Id = input.ExportImportCode,
                    NameOfReceiver = input.Customer.CustomerName,
                    OrderCreator = input.OrderCreator,
                    DeliveryEmployee = input.EmployeeDelivery,
                    NameOfExport = input.NameOfExport,
                    StructureId = input.StructureId,
                    OrderStatus = 1,
                    OrderType = input.OrderType,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                    Discount = input.Discount,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _exportImportRepository.InsertAndGetIdAsync(exportImport);

                var customer = new ExportImportCustomer
                {
                    ExportImportCode = id,
                    CustomerCode = input.Customer.CustomerCode,
                    ReciveAddress = input.Customer.CustomerAdress,
                    Discount = input.Discount,
                    PhoneToCall = input.Customer.CustomerPhone,
                };

                await _exportImportCustomerRepository.InsertAsync(customer);

                foreach (var product in input.Products)
                {
                    var exportImportProduct = new ExportImportProduct
                    {
                        ExportImportCode = id,
                        StorageId = product.StorageId,
                        StorageInputId = product.StorageInputId,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                        Location = product.Location,
                    };
                    _exportImportProductRepository.Insert(exportImportProduct);
                }

                foreach (var productExport in input.Products)
                {
                    var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                    product.ProductQuantity -= productExport.Quantity;
                    _productStorageRepository.Update(product);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAllowAnonymous]
        public async Task UpdateOrderAsync(ExportImportInput input)
        {
            try
            {
                var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == input.ExportImportCode);
                if (exportImport.OrderType == 1 && input.OrderStatus == 2)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == input.ExportImportCode)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);

                    //foreach (var productExport in exportImportProduct)
                    //{
                    //    var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                    //    product.ProductQuantity -= productExport.Quantity;
                    //    _productStorageRepository.Update(product);
                    //}
                }

                else if (exportImport.OrderType == 2 && input.OrderStatus == 2)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == input.ExportImportCode)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);


                    foreach (var productExport in exportImportProduct)
                    {
                        var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = productExport.StorageInputId,
                                ProductId = productExport.ProductId,
                                ProductLocation = productExport.Location,
                                ProductQuantity = productExport.Quantity,
                            };
                            _productStorageRepository.Insert(productInput);
                        }
                        else
                        {
                            productInput.ProductQuantity += productExport.Quantity;
                            _productStorageRepository.Update(productInput);
                        }
                        productOutput.ProductQuantity -= productExport.Quantity;
                        _productStorageRepository.Update(productOutput);
                    }
                }

                else if (exportImport.OrderType == 1 || exportImport.OrderType == 2 && input.OrderStatus == 3)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == input.ExportImportCode)
                        .ToListAsync();

                    foreach (var productExport in exportImportProduct)
                    {
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                        product.ProductQuantity += productExport.Quantity;
                        _productStorageRepository.Update(product);
                    }

                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);
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
        public async Task<ContentResult> UpdateOrderQRAsync(ExportImportInput input)
        {
            try
            {
                var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == input.ExportImportCode);
                if (exportImport.OrderType == 1 && input.OrderStatus == 2)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == exportImport.Id)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);

                    foreach (var productExport in exportImportProduct)
                    {
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                        product.ProductQuantity -= productExport.Quantity;
                        _productStorageRepository.Update(product);
                    }
                }

                else if (exportImport.OrderType == 2 && input.OrderStatus == 2)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == input.ExportImportCode)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);


                    foreach (var productExport in exportImportProduct)
                    {
                        var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageId && e.ProductId == productExport.ProductId);
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = productExport.StorageInputId,
                                ProductId = productExport.ProductId,
                                ProductLocation = productExport.Location,
                                ProductQuantity = productExport.Quantity,
                            };
                            _productStorageRepository.Insert(productInput);
                        }
                        else
                        {
                            productInput.ProductQuantity += productExport.Quantity;
                            _productStorageRepository.Update(productInput);
                        }
                        productOutput.ProductQuantity -= productExport.Quantity;
                        _productStorageRepository.Update(productOutput);
                    }
                }

                else if (exportImport.OrderType == 3 && input.OrderStatus == 2)
                {
                    var exportImportProduct = await _exportImportProductRepository.GetAll()
                        .Where(e => e.ExportImportCode == input.ExportImportCode)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);


                    foreach (var productExport in exportImportProduct)
                    {
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == productExport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = productExport.StorageInputId,
                                ProductId = productExport.ProductId,
                                ProductLocation = productExport.Location,
                                ProductQuantity = productExport.Quantity,
                            };
                            _productStorageRepository.Insert(productInput);
                        }
                        else
                        {
                            productInput.ProductQuantity += productExport.Quantity;
                            _productStorageRepository.Update(productInput);
                        }
                    }
                }

                else if (exportImport.OrderType == 1 || exportImport.OrderType == 2 || exportImport.OrderType == 3 && input.OrderStatus == 3)
                {
                    exportImport.OrderStatus = input.OrderStatus;
                    await _exportImportRepository.UpdateAsync(exportImport);
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

        public async Task UpdateAsync(ExportImportInput input)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _exportImportProductRepository.DeleteAsync(e => e.ExportImportCode == id);
                await _exportImportRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<ListOfCustomer> GetCustomerListAsync(string structureId)
        {
            try
            {
                var customerList = await _customerRepository.GetAll()
                    .Where(e => e.StructureCode == structureId)
                    .Select(e => new CustomerListDto
                    {
                        Code = e.Id,
                        Name = e.CustomerName
                    }).ToListAsync();

                var customerLists = new List<CustomerListDto>();

                foreach (var customer in customerList)
                {
                    var retailCustomer = _retailCustomerRepository.FirstOrDefault(e => e.CustomerCode == customer.Code);
                    if (retailCustomer == null)
                    {
                        customerLists.Add(customer);
                    }
                }

                return new ListOfCustomer
                {
                    items = customerLists
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CustomerDto> GetCustomerAsync(string phoneNumber)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.Id == phoneNumber);
                if (customer == null)
                    throw new UserFriendlyException("Không có khách hàng này trong hệ thống");

                var customerDto = new CustomerDto
                {
                    CustomerPhone = customer.CustomerPhone,
                    CustomerAdress = customer.CustomerAddress,
                    CustomerCode = customer.Id,
                    CustomerName = customer.CustomerName,
                    Discount = customer.Discount,
                };
                return customerDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<LookUpTableList> GetCreatorAsync()
        {
            try
            {
                var creator = await _employeeRepository.GetAll().Select(e => new LookUpTable
                {
                    Code = e.Id,
                    Name = e.EmployeeName,
                }).ToListAsync();

                return new LookUpTableList
                {
                    items = creator
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ExportImportProductDto>> GetProductAsync(ProductPagedRequest input)
        {
            try
            {
                var exportImport = new List<ExportImportProductDto>();

                if (input.IsInsert)
                {
                    exportImport = await _productRepository.GetAll()
                    .Select(e => new ExportImportProductDto
                    {
                        ProductId = e.Id,
                        ProductName = e.ProductName,
                        Quantity = 0,
                        Price = e.Price,
                        Unit = e.Unit,
                        FinalPrice = 0,
                    }).PageBy(input).ToListAsync();
                }
                else if (!string.IsNullOrEmpty(input.StorageId))
                {
                    var storageStructureList = await _storageRepository.GetAll().Where(e => e.StructureId == input.StorageId).ToListAsync();
                    storageStructureList.Add(_storageRepository.FirstOrDefault(e => e.Id == input.StorageId));
                    var structureStorage = await _structureRepository.FirstOrDefaultAsync(e => e.Id == input.StorageId);
                    var productStorage = new List<ProductStorage>();
                    var productStorageList = await _productRepository.GetAll()
                        .ToListAsync();

                    if (structureStorage != null)
                    {
                        if (!string.IsNullOrEmpty(input.Keyword))
                        {
                            productStorage = await _productStorageRepository.GetAll()
                                .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.ProductId.Contains(input.Keyword))
                                .PageBy(input).ToListAsync();
                        } else
                        {
                            foreach (var productIn in productStorageList)
                            {
                                foreach (var storage in storageStructureList)
                                {
                                    var productProduct = _productStorageRepository.FirstOrDefault(e => e.StorageId == storage.Id && productIn.Id == e.ProductId);
                                    if (productProduct != null)
                                    {
                                        productStorage.Add(productProduct);
                                    }
                                }
                            }
                        }
                    } else
                    {
                         productStorage = await _productStorageRepository.GetAll()
                        .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.ProductId.Contains(input.Keyword))
                        .Where(e => e.StorageId.Contains(input.StorageId))
                        .PageBy(input).ToListAsync();
                    }

                    List<ExportImportProductDto> result = new List<ExportImportProductDto>();
                    foreach (var storageProduct in productStorage)
                    {
                        //var productFullQuantity = _productStorageRepository.GetAll().Where(e => e.ProductId == storageProduct.ProductId).Select(L => L.ProductQuantity).Sum();

                        var product = _productRepository.FirstOrDefault(e => e.Id == storageProduct.ProductId);

                        var productDto = new ExportImportProductDto
                        {
                            StorageId = storageProduct.StorageId,
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            Quantity = storageProduct.ProductQuantity,
                            Price = product.Price,
                            Unit = product.Unit,
                            FinalPrice = product.Price * storageProduct.ProductQuantity
                        };
                        result.Add(productDto);
                    }

                    exportImport = result;
                    //exportImport = await _productStorageRepository.GetAll().Include(e => e.Product)
                    //.Where(l => l.StorageId == input.StorageId)
                    //.Select(e => new ExportImportProductDto
                    //{
                    //    ProductId = e.ProductId,
                    //    ProductName = e.Product.ProductName,
                    //    Quantity = e.ProductQuantity,
                    //    Price = e.Product.Price,
                    //    Unit = e.Product.Unit,
                    //    FinalPrice = e.Product.Price * e.ProductQuantity
                    //}).PageBy(input).ToListAsync();
                }

                int totalCount = await _productStorageRepository.CountAsync();

                return new PagedResultDto<ExportImportProductDto>
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

        public async Task<LookUpTableList> GetStorageAsync(string id)
        {
            List<LookUpTable> items = new List<LookUpTable>();
            items = await _storageRepository.GetAll()
                .Where(e => e.StructureId == id)
                .OrderByDescending(e => e.CreationTime)
                .Select(e => new LookUpTable
                {
                    Code = e.Id,
                    Name = e.StorageName,
                }).ToListAsync();

            return new LookUpTableList
            {
                items = items
            };
        }

        public async Task<PagedResultDto<ExportImportGetAllDto>> GetAllAsync(ExportImportPagedResultInput input)
        {
            try
            {
                var exportImport = new List<ExportImportGetAllDto>();
                int totalCount = 0;
                if (input.Keyword != null)
                {
                    exportImport = await _exportImportRepository.GetAll()
                       .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id.Contains(input.Keyword))
                       .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                       .PageBy(input).Select(e => new ExportImportGetAllDto
                       {
                           ExportImportCode = e.Id,
                           NameOfReceiver = e.NameOfReceiver,
                           Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                           OrderStatus = e.OrderStatus,
                           OrderType = e.OrderType,
                           TotalPrice = e.TotalPrice,
                           NameOfExport = e.NameOfExport,
                           CreationTime = e.CreationTime,
                           Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                       }).ToListAsync();

                    totalCount = await _exportImportRepository.GetAll()
                       .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword)
                       .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                       .CountAsync();
                } else
                {
                    if (input.DateTime != null)
                    {
                        var firstDate = new DateTime();
                        var endDate = new DateTime();

                        firstDate = DateTime.Parse(input.DateTime[0]);
                        endDate = DateTime.Parse(input.DateTime[1]);

                        exportImport = await _exportImportRepository.GetAll()
                            .Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate && e.OrderStatus == input.OrderStatus)
                            .PageBy(input).Select(e => new ExportImportGetAllDto
                            {
                                ExportImportCode = e.Id,
                                NameOfReceiver = e.NameOfReceiver,
                                Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                                OrderStatus = e.OrderStatus,
                                OrderType = e.OrderType,
                                TotalPrice = e.TotalPrice,
                                NameOfExport = e.NameOfExport,
                                CreationTime = e.CreationTime,
                                Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                            }).ToListAsync();
                        totalCount = await _exportImportRepository.GetAll().Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate && e.OrderStatus == input.OrderStatus).CountAsync();
                    }
                    else
                    {
                        exportImport = await _exportImportRepository.GetAll()
                           .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                           .PageBy(input).Select(e => new ExportImportGetAllDto
                           {
                               ExportImportCode = e.Id,
                               NameOfReceiver = e.NameOfReceiver,
                               Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                               OrderStatus = e.OrderStatus,
                               OrderType = e.OrderType,
                               TotalPrice = e.TotalPrice,
                               NameOfExport = e.NameOfExport,
                               CreationTime = e.CreationTime,
                               Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                           }).ToListAsync();
                        totalCount = await _exportImportRepository.GetAll().WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus).CountAsync();
                    }
                }

                return new PagedResultDto<ExportImportGetAllDto>
                {
                    Items = exportImport,
                    TotalCount = totalCount,
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<ExportImportOutput> GetAsync(string id)
        {
            try
            {
                var exportImportCustomer = await _exportImportCustomerRepository.FirstOrDefaultAsync(e => e.ExportImportCode == id);
                var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == id);
                var customer = _customerRepository.FirstOrDefault(e => e.Id == exportImportCustomer.CustomerCode);
                var product = await _exportImportProductRepository.GetAll().Include(e => e.Product)
                    .Where(l => l.ExportImportCode == id)
                    .Select(e => new ExportImportProductDto
                    {
                        ProductId = e.ProductId,
                        ProductName = e.Product.ProductName,
                        StorageId = e.StorageId,
                        StorageInputId = e.StorageInputId,
                        Quantity = e.Quantity,
                        Price = e.Product.Price,
                        Unit = e.Product.Unit,
                        FinalPrice = e.Product.Price * e.Quantity
                    }).ToListAsync();

                var customerDto = new CustomerDto();

                if (customer != null)
                {
                    customerDto.CustomerCode = customer.Id;
                    customerDto.CustomerPhone = customer.CustomerPhone;
                    customerDto.CustomerAdress = customer.CustomerAddress;
                    customerDto.CustomerName = customer.CustomerName;
                }

                ExportImportOutput output = new ExportImportOutput
                {
                    ExportImportCode = exportImport.Id,
                    OrderCreator = _employeeRepository.GetAll().FirstOrDefault(e => e.Id == exportImport.OrderCreator).EmployeeName,
                    EmployeeDelivery = _employeeRepository.GetAll().FirstOrDefault(e => e.Id == exportImport.DeliveryEmployee).EmployeeName,
                    OrderStatus = exportImport.OrderStatus,
                    OrderType = exportImport.OrderType,
                    ReceiveAddress = exportImportCustomer.ReciveAddress,
                    Customer = customerDto,
                    Products = product,
                    NameOfExport = exportImport.NameOfExport,
                    Discount = exportImport.Discount,
                    TotalPrice = exportImport.TotalPrice,
                };
                return output;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public byte[] GetByteForExcelExport(BaoGiaObject baoGia)
        {
            try
            {
                var productList = new List<ExportImportProductDto>();

                foreach (var product in baoGia.Products)
                {
                    var fileNameImage = _productRepository.FirstOrDefault(e => e.Id == product.ProductId).ProductImage;

                    var filePath = Path.Combine(_appFolders.ProductUploadFolder + @"\", fileNameImage);
                    Byte[] buffer = System.IO.File.ReadAllBytes(filePath);
                    product.PictureImage = buffer;
                    productList.Add(product);
                }

                var customer = new CustomerDto();
                customer.CustomerName = baoGia.CustomerName;
                customer.CustomerPhone = baoGia.CustomerPhone;
                customer.CustomerAdress = baoGia.CustomerAddress;

                ExcelFileGenerator exf = new ExcelFileGenerator();
                byte[] temp = exf.GenerateBaoGia(productList, customer, baoGia.Date);
                return temp;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
