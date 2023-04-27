using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.InventoryManagement.ProductManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.StorageManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
                //var listOfStorageProduct = input.storages.Select(e => new ProductStorage
                //{
                //    ProductId = input.ProductCode,
                //    StorageId = e.StorageCode,
                //    ProductQuantity = e.Quantity,
                //    ProductLocation = e.ProductLocation,
                //    //Description = e.Description
                //}).ToList();
                await _productRepository.InsertAsync(product);
                //foreach (var storage in listOfStorageProduct)
                //{
                //    _productStorageRepository.Insert(storage);
                //}
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
                        throw new UserFriendlyException("Không thể xóa sản phẩm trong Đơn");
                    }
                }

                await _productRepository.HardDeleteAsync(e => e.Id == id);
                await _productStorageRepository.HardDeleteAsync(e => e.ProductId == id);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Product_Delete)]
        public async Task<string> DeleteMultipleAsync(string[] ids)
        {
            try
            {
                int numOfDeleted = 0;
                foreach (var id in ids)
                {
                    var productExportImport = await _exportImportProductRepository.FirstOrDefaultAsync(pd => pd.ProductId == id);
                    if (productExportImport != null)
                    {
                        var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == productExportImport.ExportImportCode);
                        if (exportImport != null)
                        {
                            throw new UserFriendlyException("Không thể xóa sản phẩm trong Đơn");
                        }
                    }

                    await _productRepository.HardDeleteAsync(e => e.Id == id);
                    await _productStorageRepository.HardDeleteAsync(e => e.ProductId == id);
                    numOfDeleted++;
                }

                if (numOfDeleted == 0)
                {
                    throw new UserFriendlyException("Không thể xóa sản phẩm trong Đơn");
                }

                return "Xóa thành công " + numOfDeleted + "/" + ids.Length + " sản phẩm thành công";
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Product_Update)]
        public async Task UpdateAsync(ProductInputDto input)
        {
            try
            {
                var checkProductDto = await _productRepository.FirstOrDefaultAsync(e => e.ProductName == input.ProductName && e.Id != input.ProductCode);
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

                //var listOfStorageProduct = input.storages.Select(e => new ProductStorage
                //{
                //    Id = e.StorageProductId,
                //    ProductId = input.ProductCode,
                //    StorageId = e.StorageCode,
                //    ProductQuantity = e.Quantity,
                //    ProductLocation = e.ProductLocation,
                //    //Description = e.Description
                //}).ToList();
                //foreach (var storage in listOfStorageProduct)
                //{
                //    var storageProduct = _productStorageRepository.FirstOrDefault(e => e.Id == storage.Id);
                //    if (storageProduct == null)
                //    {
                //        storageProduct = new ProductStorage();
                //        storageProduct.ProductId = storage.ProductId;
                //    }
                //    storageProduct.ProductQuantity = storage.ProductQuantity;
                //    storageProduct.StorageId = storage.StorageId;
                //    storageProduct.ProductLocation = storage.ProductLocation;
                //    _productStorageRepository.InsertOrUpdate(storageProduct);
                //}
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
                    if (storageRepo.IsNullOrEmpty())
                    {
                        input.StorageCode = null;
                    }
                    else
                    {
                        input.StorageCode = storageRepo[storageRepo.Length - 1].Id;
                    }
                }

                if (!string.IsNullOrEmpty(input.Keyword))
                {
                    var product = await _productRepository
                        .GetAll()
                        .Where(e => e.Id.Contains(input.Keyword) || e.ProductName.Contains(input.Keyword))
                        .PageBy(input).ToListAsync();

                    List<ProductGetAllDto> result = new List<ProductGetAllDto>();
                    foreach (var storageProduct in product)
                    {
                        var productFullQuantity = _productStorageRepository.GetAll().Where(e => e.ProductId == storageProduct.Id).Select(L => L.ProductQuantity).Sum();
                        var productDto = new ProductGetAllDto
                        {
                            ProductCode = storageProduct.Id,
                            ProductName = storageProduct.ProductName,
                            CategoryName = _categoryRepository.GetAll().FirstOrDefault(i => i.Id == storageProduct.CategoryId).CategoryName,
                            Price = storageProduct.Price,
                            Unit = storageProduct.Unit,
                            Quantity = productFullQuantity,
                            CreationTime = storageProduct.CreationTime,
                            LastDateModified = storageProduct.LastModificationTime,
                            Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == storageProduct.CreatorUserId || l.Id == storageProduct.LastModifierUserId).Name
                        };
                        result.Add(productDto);
                    }
                    var totalCount = await _productRepository.CountAsync();
                    return new PagedResultDto<ProductGetAllDto>
                    {
                        Items = result,
                        TotalCount = totalCount
                    };
                }   
                
                if (input.StorageCode.Equals("0"))
                {
                    var product = await _productRepository
                        .GetAll()
                        .PageBy(input).ToListAsync();

                    List<ProductGetAllDto> result = new List<ProductGetAllDto>();
                    foreach (var storageProduct in product)
                    {
                        var productFullQuantity = _productStorageRepository.GetAll().Where(e => e.ProductId == storageProduct.Id).Select(L => L.ProductQuantity).Sum();
                        var productDto = new ProductGetAllDto
                        {
                            ProductCode = storageProduct.Id,
                            ProductName = storageProduct.ProductName,
                            CategoryName = _categoryRepository.GetAll().FirstOrDefault(i => i.Id == storageProduct.CategoryId).CategoryName,
                            Price = storageProduct.Price,
                            Unit = storageProduct.Unit,
                            Quantity = productFullQuantity,
                            CreationTime = storageProduct.CreationTime,
                            LastDateModified = storageProduct.LastModificationTime,
                            Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == storageProduct.CreatorUserId || l.Id == storageProduct.LastModifierUserId).Name
                        };
                        result.Add(productDto);
                    }
                    var totalCount = await _productRepository.CountAsync();
                    return new PagedResultDto<ProductGetAllDto>
                    {
                        Items = result,
                        TotalCount = totalCount
                    };
                }

                var storageProducts = new List<ProductStorage>();
                    storageProducts = await _productStorageRepository.GetAll()
                        .WhereIf(!string.IsNullOrEmpty(input.StorageCode), e => e.StorageId.Contains(input.StorageCode))
                        .PageBy(input).ToListAsync();

                if (!storageProducts.IsNullOrEmpty())
                {
                    int totalCount = _productStorageRepository.GetAll().Where(e => e.StorageId.Contains(input.StorageCode)).Count();
                    List<ProductGetAllDto> result = new List<ProductGetAllDto>();
                    foreach (var storageProduct in storageProducts)
                    {
                        var product = _productRepository.GetAll()
                        .WhereIf(input.CategoryCode != "0" && input.SubCategoryCode != 0, e => e.CategoryId == input.CategoryCode && e.SubCategoryId == input.SubCategoryCode)
                        .WhereIf(input.CategoryCode != "0" && input.SubCategoryCode == 0, e => e.CategoryId == input.CategoryCode)
                        .WhereIf(input.CategoryCode == "0" && input.SubCategoryCode == 0, e => e.Id == storageProduct.ProductId)
                        .FirstOrDefault(x => storageProduct.ProductId.Equals(x.Id));

                        if (product != null)
                        {
                            var productDto = new ProductGetAllDto
                            {
                                ProductCode = product.Id,
                                ProductName = product.ProductName,
                                CategoryName = _categoryRepository.GetAll().FirstOrDefault(i => i.Id == product.CategoryId).CategoryName,
                                Price = product.Price,
                                Unit = product.Unit,
                                Quantity = storageProduct.ProductQuantity,
                                CreationTime = product.CreationTime,
                                LastDateModified = product.LastModificationTime,
                                Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == product.CreatorUserId || l.Id == product.LastModifierUserId).Name
                            };
                            result.Add(productDto);
                        }
                    }
                    return new PagedResultDto<ProductGetAllDto>
                    {
                        Items = result,
                        TotalCount = totalCount
                    };
                }

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

        public async Task<CategoryProductList> GetCategoryProductAsync()
        {
            List<CategoryProduct> items = new List<CategoryProduct>();
            items = await _categoryRepository.GetAll()
                .Select(e => new CategoryProduct
                {
                    CategoryId = e.Id,
                    CategoryName = e.CategoryName,
                }).ToListAsync();

            return new CategoryProductList 
            { 
                items = items
            };
        }

        public async Task<StorageProductDetailList> GetStorageProductAsync()
        {
            List<StorageProductDetail> items = new List<StorageProductDetail>();
            items = await _storageRepository.GetAll().OrderByDescending(e => e.CreationTime)
                .Select(e => new StorageProductDetail
                {
                    StorageCode = e.Id,
                    StorageName = e.StorageName,
                }).ToListAsync();

            return new StorageProductDetailList
            {
                items = items
            };
        }

        public async Task<SubcategoryProductList> GetSubcategoryProductAsync(string categoryId)
        {
            List<SubcategoryProduct> items = new List<SubcategoryProduct>();
            items = await _subCategoryRepository.GetAll()
                .Where(e => e.CategoryId == categoryId)
                .Select(e => new SubcategoryProduct
                {
                    SubcategoryId = e.Id,
                    SubcategoryName = e.SubCategoryName
                }).ToListAsync();

            return new SubcategoryProductList
            {
                items = items
            };
        }

        public async Task<ProductOutputDto> GetAsync(string id)
        {
            try
            {
                var product = await _productRepository.FirstOrDefaultAsync(id);
                if (product == null)
                    throw new UserFriendlyException($"Không thể tìm thấy sản phẩm mã {id}");

                var storageProduct = await _productStorageRepository.GetAll().Where(e => e.ProductId == id).ToListAsync();
                
                List<StorageProductDetail> storageProductDetails = new List<StorageProductDetail>();
                foreach ( var item in storageProduct)
                {
                    var storage = _storageRepository.FirstOrDefault(e => e.Id == item.StorageId);
                    var storageProductDto = new StorageProductDetail();
                    storageProductDto.StorageProductId = item.Id;
                    storageProductDto.StorageCode = item.StorageId;
                    storageProductDto.ProductLocation = item.ProductLocation;
                    storageProductDto.StorageName = storage.StorageName;
                    storageProductDto.Quantity = item.ProductQuantity;
                    storageProductDetails.Add(storageProductDto);
                }

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
                    storages = storageProductDetails,
                };

                return productDto;
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
