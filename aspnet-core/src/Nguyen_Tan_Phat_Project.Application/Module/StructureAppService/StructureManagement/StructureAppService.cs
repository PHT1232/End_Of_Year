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
using Nguyen_Tan_Phat_Project.Module.StructureAppService.StructureManagement.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.StructureManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Structure_View)]
    public class StructureAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private IRepository<Structure, string> _structureRepository;
        private IRepository<Employee, string> _employeeRepository;
        private IRepository<User, long> _userRepository;

        public StructureAppService(
            IRepository<Structure, string> structureRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<User, long> userRepository) 
        { 
            _structureRepository = structureRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Structure_Add)]
        public async Task AddNewAsync(StructureInputDto input)
        {
            try
            {
                var structureDto = await _structureRepository.FirstOrDefaultAsync(e => e.Id == input.UnitCode || e.UnitName == input.UnitName);
                if (structureDto != null)
                {
                    throw new UserFriendlyException("Đơn vị này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                var structure = new Structure
                {
                    Id = input.UnitCode,
                    UnitName = input.UnitName,
                    Address = input.Address,
                    LevelOfUnit = input.LevelOfUnit,
                    UnitOf = input.UnitOf,
                    BusinessRN = input.BusinessRN,
                    IssuedDate = input.IssuedDate,
                    IssuedPlace = input.IssuedPlace,
                    LastModificationTime = creationTime,
                };

                await _structureRepository.InsertAsync(structure);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Structure_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                var isEmployeeInUnit = await _employeeRepository.FirstOrDefaultAsync(e => e.WorkUnit.Id == id);
                if (isEmployeeInUnit != null)
                {
                     throw new UserFriendlyException("Không thể xóa đơn vị đang có nhân viên làm việc");
                }

                await _structureRepository.HardDeleteAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[AbpAuthorize(PermissionNames.Page_System_Structure_Delete)]
        //public async Task<string> DeleteMultipleAsync(string[] ids)
        //{
        //    try
        //    {
        //        int numOfDeleted = 0;
        //        foreach (var id in ids)
        //        {
        //            var isEmployeeInUnit = await _employeeRepository.FirstOrDefaultAsync(e => e.WorkUnit.Id == id);
        //            if (isEmployeeInUnit != null)
        //            {
        //                var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == productExportImport.ExportImportCode);
        //                if (exportImport != null)
        //                {
        //                    throw new UserFriendlyException("Không thể xóa sản phẩm trong Đơn");
        //                }
        //            }

        //            await _productRepository.HardDeleteAsync(e => e.Id == id);
        //            await _productStorageRepository.HardDeleteAsync(e => e.ProductId == id);
        //            numOfDeleted++;
        //        }

        //        if (numOfDeleted == 0)
        //        {
        //            throw new UserFriendlyException("Không thể xóa sản phẩm trong Đơn");
        //        }

        //        return "Xóa thành công " + numOfDeleted + "/" + ids.Length + " sản phẩm thành công";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException(ex.Message);
        //    }
        //}

        [AbpAuthorize(PermissionNames.Page_System_Structure_Update)]
        public async Task UpdateAsync(StructureInputDto input)
        {
            try
            {
                var checkStructureDto = await _structureRepository.FirstOrDefaultAsync(e => e.UnitName == input.UnitName && e.Id != input.UnitCode);
                if (checkStructureDto != null)
                    throw new UserFriendlyException("Đã tồn tại đơn vị với tên này");

                var structureDto = await _structureRepository.FirstOrDefaultAsync(e => e.Id == input.UnitCode);
                if (structureDto == null)
                    throw new UserFriendlyException($"Không thể tìm nhân viên với mã {input.UnitCode}");

                structureDto.UnitName = input.UnitName;
                structureDto.Address = input.Address;
                structureDto.LevelOfUnit = input.LevelOfUnit;
                structureDto.UnitOf = input.UnitOf;
                structureDto.BusinessRN = input.BusinessRN;
                structureDto.IssuedDate = input.IssuedDate;
                structureDto.IssuedPlace = input.IssuedPlace;

                await _structureRepository.UpdateAsync(structureDto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<StructureSelectListDto> GetStructureSelectAsync()
        {
            try
            {
                var list = await _structureRepository.GetAll()
                    .Select(e => new StructureSelectDto
                    {
                        UnitCode = e.Id,
                        UnitName = e.UnitName
                    }).ToListAsync();

                return new StructureSelectListDto
                {
                    items = list
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<StructureGetAllDto>> GetAllAsync(StructurePagedResultInput input)
        {
            try
            {
                var query = await _structureRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id.Contains(input.Keyword) || e.UnitName.Contains(input.Keyword))
                .Select(e => new StructureGetAllDto
                {
                    UnitCode = e.Id,
                    UnitName = e.UnitName,
                    Address = e.Address,
                    LevelOfUnit = e.LevelOfUnit,
                    CreationTime = e.CreationTime,
                    LastDateModified = e.LastModificationTime,
                    username = _userRepository.GetAll().FirstOrDefault(l => l.Id == e.CreatorUserId || l.Id == e.LastModifierUserId).Name,
                }).OrderByDescending(e => e.CreationTime).PageBy(input).ToListAsync();

                int totalCount = _structureRepository.Count();

                return new PagedResultDto<StructureGetAllDto>
                {
                    Items = query,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<StructureOutputDto> GetAsync(string id)
        {
            var query = await _structureRepository.GetAsync(id);
            var structureDto = new StructureOutputDto
            {
                UnitCode = query.Id,
                UnitName = query.UnitName,
                Address = query.Address,
                LevelOfUnit = query.LevelOfUnit,
                UnitOf = query.UnitOf,
                BusinessRN = query.BusinessRN,
                IssuedDate = query.IssuedDate,
                IssuedPlace = query.IssuedPlace,
            };
            return structureDto;
        }
    }
}
