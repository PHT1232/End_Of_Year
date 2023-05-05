using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nguyen_Tan_Phat_Project.Authorization;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Entities;
using Abp.Application.Services.Dto;
using Abp.UI;
using Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Employee_View)]
    public class EmployeeAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private IRepository<Employee, string> _employeeRepository;
        private IRepository<Structure, string> _structureRepository;
        private IRepository<BankAccount> _bankRepository;
        private IRepository<CMND> _cmndRepository;
    
        public EmployeeAppService(IRepository<Employee, string> employeeRepository
            , IRepository<Structure, string> structureRepository
            , IRepository<BankAccount> bankRepository
            , IRepository<CMND> cmndRepository)
        {
            _employeeRepository = employeeRepository;
            _structureRepository = structureRepository;
            _bankRepository = bankRepository;
            _cmndRepository = cmndRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Employee_Add)]
        public async Task AddNewAsync(EmployeeInputDto input)
        {
            try
            {
                var employeeDto = await _employeeRepository.FirstOrDefaultAsync(e => e.Id == input.EmployeeCode || e.EmployeeName == input.EmployeeName);
                if (employeeDto != null)
                {
                    throw new UserFriendlyException("Nhân viên này đã tồn tại");
                }

                var employee = new Employee
                {
                    Id = input.EmployeeCode,
                    EmployeeName = input.EmployeeName,
                    EmployeeGender = input.EmployeeGender,
                    EmployeeDateOfBirth = input.EmployeeDateOfBirth,
                    JobTitle = input.JobTitle,
                    WorkUnit = _structureRepository.Get(input.WorkUnit),
                    TaxIdentification = input.TaxIdentification,
                    EmployeeSalary = input.EmployeeSalary,
                    SalaryFactor = input.SalaryFactor,
                    TypeOfContract = input.TypeOfContract,
                };
                await _employeeRepository.InsertAsync(employee);

                input.EmployeeCMND.Employee = employee;
                await _cmndRepository.InsertAsync(input.EmployeeCMND);

                input.employeeBankAccount.EmployeeId = employee.Id;
                _bankRepository.Insert(input.employeeBankAccount);
               
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Employee_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _cmndRepository.DeleteAsync(e => e.Employee.Id == id);
                await _bankRepository.DeleteAsync(e => e.Employee.Id == id);
                await _employeeRepository.HardDeleteAsync(e => e.Id == id);
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

        [AbpAuthorize(PermissionNames.Page_System_Employee_Update)]
        public async Task UpdateAsync(EmployeeInputDto input)
        {
            try
            {
                var checkEmployeeDto = await _employeeRepository.FirstOrDefaultAsync(e => e.EmployeeName == input.EmployeeName && e.Id != input.EmployeeCode);
                if (checkEmployeeDto != null)
                    throw new UserFriendlyException("Đã tồn tại nhân viên với tên này");

                var employeeDto = await _employeeRepository.FirstOrDefaultAsync(e => e.Id == input.EmployeeCode);
                if (employeeDto == null)
                    throw new UserFriendlyException($"Không thể tìm nhân viên với mã {input.EmployeeCode}");

                employeeDto.EmployeeName = input.EmployeeName;
                employeeDto.EmployeeGender = input.EmployeeGender;
                employeeDto.EmployeeDateOfBirth = input.EmployeeDateOfBirth;
                employeeDto.JobTitle = input.JobTitle;
                employeeDto.WorkUnit = _structureRepository.Get(input.WorkUnit);
                employeeDto.TaxIdentification = input.TaxIdentification;
                employeeDto.EmployeeSalary = input.EmployeeSalary;
                employeeDto.SalaryFactor = input.SalaryFactor;
                employeeDto.TypeOfContract = input.TypeOfContract;

                await _employeeRepository.UpdateAsync(employeeDto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<EmployeeGetAllDto>> GetAllAsync(EmployeePagedResultInput input)
        {
            try
            {
                var query = await _employeeRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id.Contains(input.Keyword) || e.EmployeeName.Contains(input.Keyword))
                .WhereIf(!string.IsNullOrEmpty(input.UnitCode), e => e.WorkUnit.Id.Contains(input.UnitCode))
                .Select(e => new EmployeeGetAllDto
                {
                    EmployeeCode = e.Id,
                    EmployeeName = e.EmployeeName,
                    JobTitle = e.JobTitle,
                    WorkUnit = e.WorkUnit.UnitName,
                    TaxIdentification = e.TaxIdentification,
                    AccountId = _bankRepository.GetAll().FirstOrDefault(b => b.EmployeeId == e.Id).BankId,
                    AccountName = _bankRepository.GetAll().FirstOrDefault(b => b.EmployeeId == e.Id).BankName,
                }).PageBy(input).ToListAsync();

                int totalCount = _employeeRepository.Count();

                return new PagedResultDto<EmployeeGetAllDto>
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

        public async Task<UnitEmployeeList> GetWorkUnitAsync()
        {
            var query = await _structureRepository.GetAll()
                .Select(e => new UnitEmployee
                {
                    UnitCode = e.Id,
                    UnitName = e.UnitName,
                }).ToListAsync();

            UnitEmployeeList result = new UnitEmployeeList();
            result.items = query;

            return result;
        }

        public async Task<EmployeeSelectForAccountList> GetEmployeeSelect()
        {
            var query = await _employeeRepository.GetAll()
                .Select(e => new EmployeeSelectForAccount
                {
                    Code = e.Id,
                    Name = e.EmployeeName,
                }).ToListAsync();

            EmployeeSelectForAccountList result = new EmployeeSelectForAccountList();
            result.items = query;

            return result;
        }

        public async Task<EmployeeOutputDto> GetAsync(string id)
        {
            var query = await _employeeRepository.GetAsync(id);
            var employeeDto = new EmployeeOutputDto
            {
                EmployeeCode = query.Id,
                EmployeeName = query.EmployeeName,
                EmployeeGender = query.EmployeeGender,
                EmployeeDateOfBirth = query.EmployeeDateOfBirth,
                JobTitle = query.JobTitle,
                WorkUnit = query.WorkUnit,
                TaxIdentification = query.TaxIdentification,
                EmployeeSalary = query.EmployeeSalary,
                SalaryFactor = query.SalaryFactor,
                TypeOfContract = query.TypeOfContract,
                EmployeeBankAccount = _bankRepository.GetAll().FirstOrDefault(b => b.EmployeeId == query.Id),
                EmployeeCMND = _cmndRepository.GetAll().FirstOrDefault(b => b.EmployeeId == query.Id)
            };
            return employeeDto;
        }
    }
}
