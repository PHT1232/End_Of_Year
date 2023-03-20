using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.ExportImportManagement.Dto;
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
                DateTime creationTime = DateTime.Now;
                var exportImport = new ExportImport
                {
                    Id = input.ExportImportCode,
                    StorageId = input.StorageId,
                    NameOfReceiver = input.NameOfReceiver,
                    ReceiveAddress = input.ReceiveAddress,
                    OrderCreator = input.OrderCreator,
                    OrderStatus = 1,
                    LastModificationTime = creationTime,
                };
                string id = await _exportImportRepository.InsertAndGetIdAsync(exportImportDto);
                
                foreach (var product in input.Products)
                {
                    var exportImportProduct = new ExportImportProduct
                    {
                        ExportImportCode = id,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                        TotalPrice = product.TotalPrice,
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
        public async Task AddCustomer(CustomerDto customerDto)
        {
            try
            {
                var customer = new Customer
                {
                    CustomerName = customerDto.CustomerName,
                    CustomerPhone = customerDto.CustomerPhone,
                };
                await _customerRepository.InsertAsync(customer);
            } catch (Exception ex) 
            { 
                throw new UserFriendlyException(ex.Message);
            }
        }

        //public async Task<PagedResultDto<ExportImportGetAll>> GetAllAsync(ExportImportPagedResultInput input)
        //{
        //    try
        //    {
        //        var exportImport = new List<ExportImport>();
        //        if (input.Storage == null || input.DateTime.IsNullOrEmpty())
        //        {
        //            exportImport = await _exportImportRepository.GetAll()
        //                 .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword || e.NameOfReceiver == input.Keyword)
        //                 .ToListAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException(ex.Message);
        //    }
        //}
    }
}
