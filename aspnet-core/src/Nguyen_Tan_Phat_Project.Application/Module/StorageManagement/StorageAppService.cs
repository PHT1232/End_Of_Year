using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Global;
using Nguyen_Tan_Phat_Project.Module.StorageManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Storage_View)]
    public class StorageAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Product, string> _productRepository;

        public StorageAppService(
            IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Product, string> productRepository
            )
        {
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Storage_Add)]
        public async Task AddNewAsync(StorageInput input)
        {
            try
            {
                var storageDto = await _storageRepository.FirstOrDefaultAsync(e => e.Id == input.StorageCode || e.StorageName == input.StorageName);
                if (storageDto != null)
                {
                    throw new UserFriendlyException("Đã tồn tại kho này");
                }
                DateTime creationTime = DateTime.Now;
                await _storageRepository.InsertAsync(new Storage
                {
                    Id = input.StorageCode,
                    StorageName = input.StorageName,
                    Address = input.Address,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                });

            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Storage_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                var storageDto = await _storageRepository.FirstOrDefaultAsync(e => e.Id == id);
                var checkIfStorageHaveProduct = await _productStorageRepository.FirstOrDefaultAsync(e => e.StorageId == id);
                if (checkIfStorageHaveProduct != null)
                {
                    throw new UserFriendlyException("Không thể xóa kho đang có sản phẩm");
                }
                await _storageRepository.DeleteAsync(storageDto);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Storage_Update)]
        public async Task UpdateAsync(StorageInput input)
        {
            try
            {
                var storageUpdateCheck = await _storageRepository.FirstOrDefaultAsync(x => x.StorageName == input.StorageName && x.Id != input.StorageCode);
                if (storageUpdateCheck != null)
                    throw new UserFriendlyException("Đã có kho với tên này");

                var storageDto = await _storageRepository.FirstOrDefaultAsync(e => e.Id == input.StorageCode);
                if (storageDto == null)
                    throw new UserFriendlyException("Kho này không tồn tại");

                storageDto.StorageName = input.StorageName;
                storageDto.Address = input.Address;
                storageDto.Description = input.Description;
                await _storageRepository.UpdateAsync(storageDto);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<StorageGetAllDto>> GetAllAsync(StoragePagedResultInput input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = GlobalFunction.RegexFormat(input.Keyword);
            
            try
            {
                var query = await _storageRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.StorageName.Contains(input.Keyword) || e.Id.Contains(input.Keyword))
                .Select(e => new StorageGetAllDto
                {
                    StorageCode = e.Id,
                    StorageName = e.StorageName,
                    Address = e.Address,
                    CreationTime = e.CreationTime,
                    LastDateModified = (DateTime)e.LastModificationTime,
                    Username = _userRepository.GetAll().FirstOrDefault(x => x.Id == e.CreatorUserId || x.Id == e.LastModifierUserId).Name,
                }).OrderByDescending(e => e.CreationTime).PageBy(input).ToListAsync();

                int totalCount = _storageRepository.Count();

                return new PagedResultDto<StorageGetAllDto>
                {
                    Items = query,
                    TotalCount = totalCount
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<StorageOutputDto> GetAsync(string id)
        {
            var storage = await _storageRepository.FirstOrDefaultAsync(e => e.Id == id);
            if (storage == null)
            {
                throw new UserFriendlyException($"Không thể tìm thấy kho với mã: {id}");
            }

            var productStorage = await _productStorageRepository.GetAll()
                .Where(x => x.StorageId == id)
                .Select(e => e.ProductId).ToListAsync();

            var productList = await _productRepository.GetAll()
                .Where(e => productStorage.Contains(e.Id)).ToListAsync();

            List<StorageProductDto> storageOutputDtos = new List<StorageProductDto>();
            foreach (var product in productList)
            {
                var productStorage1 = _productStorageRepository.FirstOrDefault(e => e.ProductId == product.Id);
                var storageProduct = new StorageProductDto
                {
                    ProductCode = product.Id,
                    ProductName = product.ProductName,
                    Quantity = productStorage1.ProductQuantity,
                    Unit = product.Unit,
                    Location = productStorage1.ProductLocation,
                };
                storageOutputDtos.Add(storageProduct);
            }

            var storageOutput = new StorageOutputDto()
            {
                StorageCode = storage.Id,
                StorageName = storage.StorageName,
                Address = storage.Address,
                Description = storage.Description,
                products = storageOutputDtos,
            };

            return storageOutput;
        }
        
        public async Task<StorageForUpdate> GetUpdateAsync(string id)
        {
            var storage = await _storageRepository.FirstOrDefaultAsync(e => e.Id == id);
            if (storage == null)
            {
                throw new UserFriendlyException($"Không thể tìm thấy kho với mã: {id}");
            }

            var storageOutput = new StorageForUpdate()
            {
                StorageCode = storage.Id,
                StorageName = storage.StorageName,
                Address = storage.Address,
                Description = storage.Description,
            };

            return storageOutput;
        }
    }
}
