using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.ExportImportManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ExportImportManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Export_Import_View)]
    public class ExportImportAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Category, string> _categoryRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<Customer> _customerRepository;

        public ExportImportAppService(IRepository<Product, string> productRepository
            , IRepository<Category, string> categoryRepository
            , IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<SubCategory> subCategoryRepository
            , IRepository<Customer> customerRepository
            )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _subCategoryRepository = subCategoryRepository;
            _customerRepository = customerRepository;
        }


        public async Task<string> GetRandomCode()
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
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.CustomerPhone == input.Customer.CustomerPhone);
                if (customer == null)
                {
                    customer.CustomerName = input.Customer.CustomerName;
                    customer.CustomerPhone = input.Customer.CustomerPhone;
                    customer.CustomerAdress = input.Customer.CustomerAdress;
                    await _customerRepository.InsertAsync(customer);
                }

                DateTime creationTime = DateTime.Now;
                var exportImport = new ExportImport
                {
                    Id = input.ExportImportCode,
                    StorageId = input.StorageId,
                    NameOfReceiver = input.Customer.CustomerName,
                    ReceiveAddress = input.Customer.CustomerAdress,
                    OrderCreator = input.OrderCreator,
                    OrderStatus = 1,
                    OrderType = 1,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _exportImportRepository.InsertAndGetIdAsync(exportImport);

                foreach (var product in input.Products)
                {
                    var exportImportProduct = new ExportImportProduct
                    {
                        ExportImportCode = id,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                    };
                    _exportImportProductRepository.Insert(exportImportProduct);
                }
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Update)]
        public async Task UpdateAsync()
        {

        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _exportImportProductRepository.DeleteAsync(e => e.ExportImportCode == id);
                await _exportImportRepository.DeleteAsync(id);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Add)]
        public async Task AddCustomerAsync(CustomerDto customerDto)
        {
            try
            {
                var customer = new Customer
                {
                    CustomerName = customerDto.CustomerName,
                    CustomerPhone = customerDto.CustomerPhone,
                    CustomerAdress = customerDto.CustomerAdress,
                };
                await _customerRepository.InsertAsync(customer);
            } catch (Exception ex) 
            { 
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CustomerDto> GetCustomerAsync(string phoneNumber)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(e => e.CustomerPhone == phoneNumber);
                if (customer == null)
                    throw new UserFriendlyException("Không có khách hàng này trong hệ thống");

                var customerDto = new CustomerDto
                {
                    CustomerPhone = customer.CustomerPhone,
                    CustomerAdress = customer.CustomerAdress,
                    CustomerCode = customer.Id,
                    CustomerName = customer.CustomerName,
                };
                return customerDto;
            } catch (Exception ex) 
            { 
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<LookUpTable>> GetCreatorAsync()
        {
            try
            {
                var creator = await _userRepository.GetAll().Where(l => l.Id != 1 && l.Id != 2).Select(e => new LookUpTable
                {
                    Id = (int)e.Id,
                    Name = e.FullName,
                }).ToListAsync();

                return creator;
            } catch (Exception ex) 
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ExportImportProductDto>> GetProductAsync(ProductPagedRequest input)
        {
            try
            {
                var exportImport = await _productStorageRepository.GetAll().Include(e => e.Product)
                    .Where(l => l.StorageId == input.StorageId)
                    .Select(e => new ExportImportProductDto
                    {
                        ProductId = e.ProductId,
                        ProductName = e.Product.ProductName,
                        Quantity = e.ProductQuantity,
                        Price = e.Product.Price,
                        Unit = e.Product.Unit,
                        FinalPrice = (e.Product.Price * e.ProductQuantity)
                    }).PageBy(input).ToListAsync();

                int totalCount = await _productStorageRepository.CountAsync();

                return new PagedResultDto<ExportImportProductDto>
                {
                    Items = exportImport,
                    TotalCount = totalCount
                };
            } catch (Exception ex) 
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ExportImportGetAllDto>> GetAllAsync(ExportImportPagedResultInput input)
        {
            try
            {
                //var exportImport = new List<ExportImport>();
                //if (input.Storage == null || input.DateTime.IsNullOrEmpty())
                //{
                //    exportImport = await _exportImportRepository.GetAll()
                //         .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword || e.NameOfReceiver == input.Keyword)
                //         .PageBy(input)
                //         .ToListAsync();
                //} else if (input.Storage != null) 
                //{
                //    exportImport = await _exportImportRepository.GetAll()
                //        .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage)
                //        .PageBy(input)
                //        .ToListAsync();
                //} else if (input.DateTime.Length > 0)
                //{
                //    exportImport = await _exportImportRepository.GetAll()
                //        .Where(e => e.CreationTime >= input.DateTime[0] && e.LastModificationTime <= input.DateTime[1])
                //        .PageBy(input)
                //        .ToListAsync();
                //} else if (input.OrderType != 0)
                //{
                var exportImport = await _exportImportRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword || e.NameOfReceiver == input.Keyword)
                    .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage)
                    .WhereIf(input.DateTime != null, e => e.CreationTime >= input.DateTime[0] && e.LastModificationTime <= input.DateTime[1])
                    .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                    .PageBy(input).Select(e => new ExportImportGetAllDto
                    {
                        ExportImportCode = e.Id,
                        NameOfReceiver = e.NameOfReceiver,
                        Address = e.ReceiveAddress,
                        StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                        OrderStatus = e.OrderStatus,
                        OrderType = e.OrderType,
                        TotalPrice = e.TotalPrice,
                        CreationTime = e.CreationTime,
                        LastModifiedDate = e.LastModificationTime,
                        Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).FullName,
                    }).ToListAsync();
                //}
                int totalCount = await _exportImportRepository.CountAsync();
                //List<ExportImportGetAllDto> list = exportImport.Select(e => new ExportImportGetAllDto
                //{
                //    ExportImportCode = e.Id,
                //    NameOfReceiver = e.NameOfReceiver,
                //    Address = e.ReceiveAddress,
                //    StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                //    OrderStatus = e.OrderStatus,
                //    OrderType = e.OrderType,
                //    TotalPrice = e.TotalPrice,
                //    CreationTime = e.CreationTime,
                //    LastModifiedDate = e.LastModificationTime,
                //    Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == e.OrderCreator).FullName,
                //}).ToList();

                //foreach (var item in exportImport)
                //{
                //    ExportImportGetAll exportImportGetAll = new ExportImportGetAll();
                //    exportImportGetAll.ExportImportCode = item.Id;
                //    exportImportGetAll.NameOfReceiver = item.NameOfReceiver;
                //    //exportImportGetAll.Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == item.OrderCreator).FullName;
                //    exportImportGetAll.OrderStatus = item.OrderStatus;
                //    exportImportGetAll.OrderType = item.OrderType;
                //    exportImportGetAll.Address = item.ReceiveAddress;
                //    //exportImportGetAll.StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == item.StorageId).StorageName;
                //    exportImportGetAll.CreationTime = item.CreationTime;
                //    exportImportGetAll.LastModifiedDate = item.LastModificationTime;
                //    exportImportGetAll.totalPrice = item.TotalPrice;
                //    list.Add(exportImportGetAll);
                //}

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

    }
}
