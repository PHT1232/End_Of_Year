﻿using Abp.Application.Services.Dto;
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
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.StorageManagement.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.StorageManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Storage_View)]
    public class StorageAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Structure, string> _structureRepository;

        public StorageAppService(
            IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Product, string> productRepository
            , IRepository<Structure, string> structureRepository
            )
        {
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
            _structureRepository = structureRepository;
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
                    StructureId = input.StructureId,
                    Address = input.Address,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                });

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Storage_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                var checkIfStorageHaveProduct = await _productStorageRepository.FirstOrDefaultAsync(e => e.StorageId == id);
                if (checkIfStorageHaveProduct != null)
                {
                    throw new UserFriendlyException("Không thể xóa kho đang có sản phẩm");
                }
                await _storageRepository.HardDeleteAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Storage_Delete)]
        public async Task<string> DeleteMultipleAsync(string[] ids)
        {
            try
            {
                var storagesDontHaveProduct = ids.Where(e => _productStorageRepository.FirstOrDefault(x => e == x.StorageId) == null).ToArray();

                await _storageRepository.HardDeleteAsync(e => storagesDontHaveProduct.Contains(e.Id));

                return "Xóa thành công " + storagesDontHaveProduct.Count() + "/" + ids.Length + " kho";
            }
            catch (Exception ex)
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

                storageDto.StructureId = input.StructureId;
                storageDto.StorageName = input.StorageName;
                storageDto.Address = input.Address;
                storageDto.Description = input.Description;
                await _storageRepository.UpdateAsync(storageDto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ListStorageGetAllDto>> GetAllAsync(StoragePagedResultInput input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = GlobalFunction.RegexFormat(input.Keyword);

            try
            {
                //var query = await _storageRepository.GetAll()
                //.WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.StorageName.Contains(input.Keyword) || e.Id.Contains(input.Keyword))
                //.Select(e => new ListStorageGetAllDto
                //{
                //    StorageCode = e.Id,
                //    StorageName = e.StorageName,
                //    Unit = _structureRepository.GetAll().FirstOrDefault(x => x.Id == e.StructureId).UnitName,
                //    Address = e.Address,
                //    CreationTime = e.CreationTime,
                //    LastDateModified = (DateTime)e.LastModificationTime,
                //    Username = _userRepository.GetAll().FirstOrDefault(x => x.Id == e.CreatorUserId || x.Id == e.LastModifierUserId).Name,
                //}).OrderByDescending(e => e.CreationTime).PageBy(input).ToListAsync();
                
                

                var structures = await _structureRepository.GetAll()
                    .ToListAsync();

                List<ListStorageGetAllDto> result = new List<ListStorageGetAllDto>();
                foreach (var structure in structures)
                {
                    ListStorageGetAllDto dto = new ListStorageGetAllDto();
                    var storages = await _storageRepository.GetAll()
                        .Where(e => e.StructureId == structure.Id)
                        .Select(e => new StorageGetAllDto
                        {
                            StorageCode = e.Id,
                            StorageName = e.StorageName,
                            Unit = _structureRepository.GetAll().FirstOrDefault(x => x.Id == e.StructureId).UnitName,
                            Address = e.Address,
                            CreationTime = e.CreationTime,
                            LastDateModified = (DateTime)e.LastModificationTime,
                            ProductQuantity = _productStorageRepository.GetAll().Where(x => x.StorageId.Equals(e.Id)).Select(x => x.ProductQuantity).Sum(),
                            Username = _userRepository.GetAll().FirstOrDefault(x => x.Id == e.CreatorUserId || x.Id == e.LastModifierUserId).Name,
                        })
                        .ToListAsync();

                    dto.StorageCode = structure.Id;
                    dto.ProductQuantity = storages.Select(e => e.ProductQuantity).Sum();
                    dto.StorageName = structure.UnitName;
                    dto.Address = structure.Address;
                    dto.Children = storages;

                    result.Add(dto);
                }

                int totalCount = _storageRepository.Count();

                return new PagedResultDto<ListStorageGetAllDto>
                {
                    Items = result,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
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

            var storageOutput = new StorageOutputDto()
            {
                StorageCode = storage.Id,
                StorageName = storage.StorageName,
                Address = storage.Address,
                Description = storage.Description,
            };

            return storageOutput;
        }

        public async Task<StorageOutputDto> GetProductAsync(string id)
        {
            var productStorage = await _productStorageRepository.GetAll()
                .Where(x => x.StorageId == id).ToListAsync();

            var productStorageId = productStorage.Select(x => x.ProductId).ToList();

            var productList = new List<StorageProductDto>();

            foreach (var productId in productStorageId)
            {
                var product = _productRepository.FirstOrDefault(e => e.Id == productId);
                var productStorageDto = new StorageProductDto()
                {
                    ProductCode = product.Id,
                    ProductName = product.ProductName,
                    Quantity = productStorage.FirstOrDefault(x => x.ProductId == product.Id && x.StorageId == id).ProductQuantity,
                    Unit = product.Unit,
                    Location = productStorage.FirstOrDefault(x => x.ProductId == product.Id && x.StorageId == id).ProductLocation,
                };
                productList.Add(productStorageDto);
            }

            var storageOutput = new StorageOutputDto()
            {
                products = productList,
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
