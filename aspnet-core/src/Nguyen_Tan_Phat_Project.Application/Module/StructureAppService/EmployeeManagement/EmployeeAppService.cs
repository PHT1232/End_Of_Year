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
using Nguyen_Tan_Phat_Project.Authorization.Users;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Employee_View)]
    public class EmployeeAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private IRepository<Employee, string> _employeeRepository;
        private IRepository<Structure, string> _structureRepository;
        private IRepository<User, long> _userReposistory;
        private IRepository<BankAccount> _bankRepository;
        private IRepository<CMND> _cmndRepository;
    
        public EmployeeAppService(IRepository<Employee, string> employeeRepository
            , IRepository<Structure, string> structureRepository
            , IRepository<BankAccount> bankRepository
            , IRepository<User, long> userRepository
            , IRepository<CMND> cmndRepository)
        {
            _employeeRepository = employeeRepository;
            _structureRepository = structureRepository;
            _bankRepository = bankRepository;
            _userReposistory = userRepository;
            _cmndRepository = cmndRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Employee_Add)]
        public async Task AddNewAsync(EmployeeInputDto input)
        {
            try
            {
                var employee = await _employeeRepository.FirstOrDefaultAsync(e => e.Id == input.EmployeeCode || e.EmployeeName == input.EmployeeName);
                if (employee != null)
                {
                    throw new UserFriendlyException("Nhân viên này đã tồn tại");
                }

                if (input.employeeBankAccount.BankId == null)
                {
                    employee = new Employee
                    {
                        Id = input.EmployeeCode,
                        EmployeeName = input.EmployeeName,
                        EmployeeGender = input.EmployeeGender,
                        EmployeeDateOfBirth = input.EmployeeDateOfBirth,
                        JobTitle = input.JobTitle,
                        WorkUnitId = input.WorkUnit,
                        TaxIdentification = input.TaxIdentification,
                        phoneNumber = input.PhoneNumber,
                        EmployeeSalary = input.EmployeeSalary,
                        SalaryFactor = input.SalaryFactor,
                        TypeOfContract = input.TypeOfContract,
                    };
                } else
                {
                    employee = new Employee
                    {
                        Id = input.EmployeeCode,
                        EmployeeName = input.EmployeeName,
                        EmployeeGender = input.EmployeeGender,
                        EmployeeDateOfBirth = input.EmployeeDateOfBirth,
                        JobTitle = input.JobTitle,
                        WorkUnitId = input.WorkUnit,
                        TaxIdentification = input.TaxIdentification,
                        phoneNumber = input.PhoneNumber,
                        EmployeeSalary = input.EmployeeSalary,
                        SalaryFactor = input.SalaryFactor,
                        TypeOfContract = input.TypeOfContract,
                        BankAccount = input.employeeBankAccount
                    };
                }

                await _employeeRepository.InsertAsync(employee);

                input.EmployeeCMND.Employee = employee;
                await _cmndRepository.InsertAsync(input.EmployeeCMND);

                //if (input.employeeBankAccount.BankId != null)
                //{
                //    input.employeeBankAccount.EmployeeId = employee.Id;
                //    _bankRepository.Insert(input.employeeBankAccount);
                //}
               
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
                var employeeAccount = await _userReposistory.FirstOrDefaultAsync(e => e.UserName == id);
                if (employeeAccount != null)
                {
                    throw new UserFriendlyException("Nhân viên có tài khoản không thể bị xóa");
                }
                var employee = await _employeeRepository.GetAsync(id);
                await _cmndRepository.DeleteAsync(e => e.Employee.Id == id);
                await _bankRepository.DeleteAsync(e => e.BankId == employee.Id);
                await _employeeRepository.HardDeleteAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Structure_Delete)]
        public async Task<string> DeleteMultipleAsync(string[] ids)
        {
            try
            {
                var employeeDontHaveAccount = ids.Where(e => _userReposistory.FirstOrDefault(x => x.UserName == e) == null).ToArray();
                foreach (string id in employeeDontHaveAccount)
                {
                    var bankAccountToDelete = _employeeRepository.GetAll().Where(e => e.Id == id).FirstOrDefault().BankId;
                    await _bankRepository.DeleteAsync(e => e.BankId == bankAccountToDelete);
                }
                await _cmndRepository.DeleteAsync(e => employeeDontHaveAccount.Contains(e.Employee.Id));
                await _employeeRepository.HardDeleteAsync(e => employeeDontHaveAccount.Contains(e.Id));

                return "Xóa thành công " + employeeDontHaveAccount.Count() + "/" + ids.Length + " nhân viên";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

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
                employeeDto.phoneNumber = input.PhoneNumber;
                employeeDto.TypeOfContract = input.TypeOfContract;

                var employeeBankAccount = await _bankRepository.FirstOrDefaultAsync(e => e.BankId == input.employeeBankAccount.BankId);
                if (employeeBankAccount == null || employeeBankAccount.BankId != input.employeeBankAccount.BankId)
                {
                    await _bankRepository.InsertAsync(input.employeeBankAccount);
                    employeeDto.BankId = input.employeeBankAccount.BankId;
                    await _employeeRepository.UpdateAsync(employeeDto);
                }

                //_cmndRepository.Update(input.EmployeeCMND);
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
                    EmployeePhone = e.phoneNumber,
                    AccountId = e.BankAccount.BankId,
                    AccountName = e.BankAccount.BankName,
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
                EmployeePhone = query.phoneNumber,
                EmployeeSalary = query.EmployeeSalary,
                SalaryFactor = query.SalaryFactor,
                TypeOfContract = query.TypeOfContract,
                EmployeeBankAccount = _bankRepository.GetAll().FirstOrDefault(b => b.BankId == query.BankId),
                EmployeeCMND = _cmndRepository.GetAll().FirstOrDefault(b => b.EmployeeId == query.Id)
            };
            return employeeDto;
        }
    }
}
