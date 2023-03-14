using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Product_View)]
    public class ProductAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Category, string> _categoryRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;

        public ProductAppService(IRepository<Product, string> productRepository
            , IRepository<Category, string> categoryRepository
            , IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<SubCategory> subCategoryRepository
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
        }

        [AbpAuthorize(PermissionNames.Page_System_Product_Add)]
        public async Task AddNewAsync(ProductInputDto input)
        {
            try
            {
                var productDto = await _productRepository.FirstOrDefaultAsync(e => e.Id == input.ProductCode || e.ProductName == input.ProductName);
                if (productDto != null)
                {
                    throw new UserFriendlyException("Sản phẩm này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                var product = new Product
                {
                    Id = input.ProductCode,
                    ProductName = input.ProductName,
                    ProductDescription = input.ProductDescription,
                    ProductDetail = input.ProductDetail,
                    Price = input.Price,
                    CategoryId = input.CategoryId,
                    Unit = input.Unit,
                    SubCategoryId = input.SubCategoryId,
                    LastModificationTime = creationTime,
                };
                var listOfStorageProduct = input.storages.Select(e => new ProductStorage
                {
                    ProductId = input.ProductCode,
                    StorageId = e.StorageCode,
                    ProductQuantity = e.ProductQuantity,
                    ProductLocation = e.ProductLocation,
                    Description = e.Description
                }).ToList();
                await _productRepository.InsertAsync(product);
                foreach (var storage in listOfStorageProduct)
                {
                    _productStorageRepository.Insert(storage);
                }
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Product_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                var productExportImport = await _exportImportProductRepository.FirstOrDefaultAsync(pd => pd.ProductId == id);
                if (productExportImport != null)
                {
                    var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == productExportImport.ExportImportCode);
                    if (exportImport != null)
                    {
                        if (exportImport.OrderStatus.Equals("Inprocess"))
                        {
                            throw new UserFriendlyException("Không thể xóa sản phẩm đang trong quá trình xuất/nhập kho");
                        }
                    }
                }

                await _productRepository.DeleteAsync(e => e.Id == id);
                await _productStorageRepository.DeleteAsync(e => e.ProductId == id);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Product_Update)]
        public async Task UpdateAsync(ProductInputDto input)
        {
            try
            {
                var checkProductDto = await _productRepository.FirstOrDefaultAsync(e => e.ProductName == input.ProductName);
                if (checkProductDto != null)
                    throw new UserFriendlyException("Đã tồn tại sản phẩm với tên này");

                var productDto = await _productRepository.FirstOrDefaultAsync(e => e.Id == input.ProductCode);
                if (productDto == null)
                    throw new UserFriendlyException($"Không thể tìm sản phẩm với mã {input.ProductCode}");

                productDto.ProductName = input.ProductName;
                productDto.ProductDetail = input.ProductDetail;
                productDto.ProductDescription = input.ProductDescription;
                productDto.Price = input.Price;
                productDto.CategoryId = input.CategoryId;
                productDto.SubCategoryId = input.SubCategoryId;
                productDto.Unit = input.Unit;

                await _productRepository.UpdateAsync(productDto);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ProductGetAllDto>> GetAllAsync(ProductPagedResultInput input)
        {
            try
            {
                if (input.StorageCode == null)
                {
                    var storageRepo = await this._storageRepository.GetAll().OrderByDescending(e => e.CreationTime).ToArrayAsync();
                    input.StorageCode = storageRepo[0].Id; 
                }
                var storageProducts = await _productStorageRepository.GetAll()
                        .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.StorageId.Contains(input.Keyword))
                        .WhereIf(!string.IsNullOrEmpty(input.StorageCode), e => e.StorageId.Contains(input.StorageCode))
                        .PageBy(input).ToListAsync();

                //if (!string.IsNullOrEmpty(input.Keyword))
                //{
                //    var storageProducts = await _productStorageRepository.GetAll()
                //        .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.StorageId.Contains(input.Keyword))
                //        .WhereIf(!string.IsNullOrEmpty(input.StorageCode), e => e.StorageId.Contains(input.StorageCode))
                //        .PageBy(input).ToListAsync();
                //}
                if (!storageProducts.IsNullOrEmpty())
                {
                    int totalCount = _productStorageRepository.GetAll().Where(e => e.StorageId.Contains(input.Keyword)).Count();
                    List<ProductGetAllDto> result = new List<ProductGetAllDto>();
                    foreach (var storageProduct in storageProducts)
                    {
                        var product = await _productRepository.FirstOrDefaultAsync(e => e.Id == storageProduct.ProductId);
                        var productDto = new ProductGetAllDto
                        {
                            ProductCode = product.Id,
                            ProductName = product.ProductName,
                            CategoryName = product.CategoryId,
                            Price = product.Price,
                            Unit = product.Unit,
                            CreationTime = product.CreationTime,
                            LastDateModified = product.LastModificationTime,
                            Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == product.CreatorUserId || l.Id == product.LastModifierUserId).Name
                        };
                        result.Add(productDto);
                    }

                    List<ProductGetAllDto> resultList = new List<ProductGetAllDto>();
                    resultList = result.Distinct().ToList();

                    return new PagedResultDto<ProductGetAllDto>
                    {
                        Items = resultList,
                        TotalCount = totalCount
                    };
                }

                var productDto1 = await _productRepository.GetAll()

                //else
                //{
                //    int totalCount = _productRepository.GetAll()
                //    .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.CategoryId.Contains(input.Keyword))
                //    .WhereIf(!string.IsNullOrEmpty(input.CategoryCode), e => e.CategoryId.Contains(input.CategoryCode))
                //    .Count();

                //    var productDto = await _productRepository.GetAll()
                //    .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.CategoryId.Contains(input.Keyword)).Select(e => new ProductGetAllDto
                //    {
                //        ProductCode = e.Id,
                //        ProductName = e.ProductName,
                //        CategoryName = e.CategoryId,
                //        Price = e.Price,
                //        Unit = e.Unit,
                //        CreationTime = e.CreationTime,
                //        LastDateModified = e.LastModificationTime,
                //        Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == e.CreatorUserId || l.Id == e.LastModifierUserId).Name
                //    }).PageBy(input).Distinct().ToListAsync();

                //    return new PagedResultDto<ProductGetAllDto>
                //    {
                //        Items = productDto,
                //        TotalCount = totalCount
                //    };
                //}
                //var  _categoryRepository.FirstOrDefault(m => m.Id == e.CategoryId).CategoryName;
                //var productGetAll = await _productRepository.GetAll().Select(e => new ProductGetAllDto
                //{
                //    ProductCode = e.Id,
                //    ProductName = e.ProductName,
                //    CategoryName = e.CategoryId,
                //    Price = e.Price,
                //    Unit = e.Unit,
                //    CreationTime = e.CreationTime,
                //    LastDateModified = e.LastModificationTime,
                //    Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == e.CreatorUserId || l.Id == e.LastModifierUserId).Name
                //}).PageBy(input).ToListAsync();

                return new PagedResultDto<ProductGetAllDto>
                {
                    Items = new List<ProductGetAllDto>(),
                    TotalCount = 0
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<CategoryProduct>> GetCategoryProductAsync()
        {
            var categoryDto = await _categoryRepository.GetAll()
                .Select(e => new CategoryProduct
                {
                    CategoryId = e.Id,
                    CategoryName = e.CategoryName,
                }).ToListAsync();

            return categoryDto;
        }

        public async Task<List<StorageProductDetail>> GetStorageProductAsync()
        {
            var storageDto = await _storageRepository.GetAll()
                .Select(e => new StorageProductDetail
                {
                    StorageCode = e.Id,
                    StorageName = e.StorageName,
                }).ToListAsync();

            return storageDto;
        }

        public async Task<List<SubcategoryProduct>> GetSubcategoryProductAsync(string categoryId)
        {
            var subCategoryDto = await _subCategoryRepository.GetAll()
                .Where(e => e.CategoryId == categoryId)
                .Select(e => new SubcategoryProduct
                {
                    SubcategoryId = e.Id,
                    SubcategoryName = e.SubCategoryName
                }).ToListAsync();

            return subCategoryDto;
        }

        public async Task<ProductOutputDto> GetAsync(string id)
        {
            try
            {
                var product = await _productRepository.FirstOrDefaultAsync(id);
                if (product == null)
                    throw new UserFriendlyException($"Không thể tìm thấy sản phẩm mã {id}");

                var storageProduct = await _productStorageRepository.FirstOrDefaultAsync(e => e.ProductId == id);
                var StorageProductDetail = await _storageRepository.GetAll()
                    .Where(e => e.Id == storageProduct.StorageId)
                    .Select(e => new StorageProductDetail
                    {
                        StorageCode = e.Id,
                        StorageName = e.StorageName,
                        Quantity = storageProduct.ProductQuantity
                    })
                    .ToListAsync();

                var productDto = new ProductOutputDto
                {
                    ProductCode = product.Id,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Unit = product.Unit,
                    ProductDescription = product.ProductDescription,
                    ProductDetail = product.ProductDetail,
                    CategoryId = product.CategoryId,
                    SubCategoryId = product.SubCategoryId,
                    storages = StorageProductDetail,
                };

                return productDto;
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
