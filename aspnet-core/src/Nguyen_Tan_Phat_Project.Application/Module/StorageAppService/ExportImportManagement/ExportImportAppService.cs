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
using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Export_Import_View)]
    public class ExportImportAppService : Nguyen_Tan_Phat_ProjectAppServiceBase, IExportImportAppService, ITransientDependency
    {
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public ExportImportAppService(IRepository<Product, string> productRepository
            , IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<Customer, string> customerRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<Employee, string> employeeRepository
            )
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _customerRepository = customerRepository;
            _exportImportCustomerRepository = exportImportCustomerRepository;
            _employeeRepository = employeeRepository;
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
                    StorageId = input.StorageId,
                    NameOfReceiver = input.Customer.CustomerName,
                    OrderCreator = input.OrderCreator,
                    DeliveryEmployee = input.EmployeeDelivery,
                    NameOfExport = input.NameOfExport,
                    OrderStatus = 1,
                    OrderType = input.OrderType,
                    StorageInputId = input.StorageInputId,
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
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                        Location = product.Location,
                    };
                    _exportImportProductRepository.Insert(exportImportProduct);
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

                    foreach (var productExport in exportImportProduct)
                    {
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
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
                        var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = exportImport.StorageInputId,
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
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = exportImport.StorageInputId,
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
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
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
                        var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = exportImport.StorageInputId,
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
                        var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
                        if (productInput == null)
                        {
                            productInput = new ProductStorage
                            {
                                StorageId = exportImport.StorageInputId,
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

                var html = "<html><body><h1>HOÀN THÀNH ĐƠN THÀNH CÔNG</h1></body></html>";

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

        public async Task<ListOfCustomer> GetCustomerListAsync()
        {
            try
            {
                var customerList = await _customerRepository.GetAll().Select(e => new CustomerListDto
                {
                    Code = e.Id,
                    Name = e.CustomerName
                }).ToListAsync();

                return new ListOfCustomer
                {
                    items = customerList
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
                else
                {
                    exportImport = await _productStorageRepository.GetAll().Include(e => e.Product)
                    .Where(l => l.StorageId == input.StorageId)
                    .Select(e => new ExportImportProductDto
                    {
                        ProductId = e.ProductId,
                        ProductName = e.Product.ProductName,
                        Quantity = e.ProductQuantity,
                        Price = e.Product.Price,
                        Unit = e.Product.Unit,
                        FinalPrice = e.Product.Price * e.ProductQuantity
                    }).PageBy(input).ToListAsync();
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

        public async Task<PagedResultDto<ExportImportGetAllDto>> GetAllAsync(ExportImportPagedResultInput input)
        {
            try
            {
                var exportImport = new List<ExportImportGetAllDto>();

                if (input.Keyword != null)
                {
                    exportImport = await _exportImportRepository.GetAll()
                       .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword)
                       .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                       .PageBy(input).Select(e => new ExportImportGetAllDto
                       {
                           ExportImportCode = e.Id,
                           NameOfReceiver = e.NameOfReceiver,
                           Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                           StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                           OrderStatus = e.OrderStatus,
                           OrderType = e.OrderType,
                           TotalPrice = e.TotalPrice,
                           NameOfExport = e.NameOfExport,
                           CreationTime = e.CreationTime,
                           Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                       }).ToListAsync();
                } else
                {
                    if (input.DateTime != null)
                    {
                        var firstDate = new DateTime();
                        var endDate = new DateTime();

                        firstDate = DateTime.Parse(input.DateTime[0]);
                        endDate = DateTime.Parse(input.DateTime[1]);

                        exportImport = await _exportImportRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage || e.StorageInputId == input.Storage)
                            .Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate && e.OrderStatus == input.OrderStatus)
                            .PageBy(input).Select(e => new ExportImportGetAllDto
                            {
                                ExportImportCode = e.Id,
                                NameOfReceiver = e.NameOfReceiver,
                                Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                                StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                                OrderStatus = e.OrderStatus,
                                OrderType = e.OrderType,
                                TotalPrice = e.TotalPrice,
                                NameOfExport = e.NameOfExport,
                                CreationTime = e.CreationTime,
                                Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                            }).ToListAsync();
                    }
                    else
                    {
                        exportImport = await _exportImportRepository.GetAll()
                           .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage || e.StorageInputId == input.Storage)
                           .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                           .PageBy(input).Select(e => new ExportImportGetAllDto
                           {
                               ExportImportCode = e.Id,
                               NameOfReceiver = e.NameOfReceiver,
                               Address = _exportImportCustomerRepository.GetAll().FirstOrDefault(c => c.ExportImportCode == e.Id).ReciveAddress,
                               StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                               OrderStatus = e.OrderStatus,
                               OrderType = e.OrderType,
                               TotalPrice = e.TotalPrice,
                               NameOfExport = e.NameOfExport,
                               CreationTime = e.CreationTime,
                               Username = _employeeRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).EmployeeName,
                           }).ToListAsync();
                    }
                }

                int totalCount = await _exportImportRepository.CountAsync();

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
                    StorageId = _storageRepository.GetAll().FirstOrDefault(p => p.Id == exportImport.StorageId).StorageName,
                    StorageInputId = exportImport.StorageInputId,
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
    }
}
