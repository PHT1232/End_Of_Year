using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Dtos;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Revenue_View)]
    public class ExpensesAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Expenses, string> _expensesRepository;
        private readonly IRepository<ProductExpenses> _productExpensesReposistory;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public ExpensesAppService(IRepository<Expenses, string> expensesRepository
            , IRepository<ProductExpenses> productExpensesReposistory
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<Product, string> productRepository
            )
        {
            _expensesRepository = expensesRepository;
            _productExpensesReposistory = productExpensesReposistory;
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
        }

        [AbpAuthorize(PermissionNames.Page_System_Revenue_Add)]
        public async Task AddNewAsync(ExpensesInputDto input)
        {
            try
            {
                var expenses = await _expensesRepository.FirstOrDefaultAsync(e => e.Id == input.ExpensesCode);
                if (expenses != null)
                {
                    throw new UserFriendlyException("Đơn này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                expenses = new Expenses
                {
                    Id = input.ExpensesCode,
                    StorageId = input.StorageId,
                    ProductProvider = input.ProductProvider,
                    OrderCreator = input.EmployeeCode,
                    OrderStatus = 1,
                    Description = input.Description,
                    PaymentDate = input.PaymentDate,
                    Discount = input.Discount,
                    LastModificationTime = creationTime,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _expensesRepository.InsertAndGetIdAsync(expenses);

                foreach (var product in input.Products)
                {
                    var productExpenses = new ProductExpenses
                    {
                        ExpensesId = id,
                        ProductCode = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                        Location = product.Location,
                    };
                    _productExpensesReposistory.Insert(productExpenses);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Export_Import_Update)]
        public async Task UpdateOrderAsync(ExpensesInputDto input)
        {
            try
            {
                var exportImport = await _expensesRepository.FirstOrDefaultAsync(e => e.Id == input.ExpensesCode);
                if (input.OrderStatus == 2)
                {
                    var expensesProduct = await _productExpensesReposistory.GetAll()
                        .Where(e => e.ExpensesId == input.ExpensesCode)
                        .ToListAsync();
                    exportImport.OrderStatus = input.OrderStatus;
                    await _expensesRepository.UpdateAsync(exportImport);

                    foreach (var productEx in expensesProduct)
                    {
                        var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productEx.ProductCode);
                        if (product == null)
                        {
                            product = new ProductStorage
                            {
                                StorageId = exportImport.StorageId,
                                ProductId = productEx.ProductCode,
                                ProductLocation = productEx.Location,
                                ProductQuantity = productEx.Quantity,
                            };
                            _productStorageRepository.Insert(product);
                        }
                        else
                        {
                            product.ProductQuantity += productEx.Quantity;
                            _productStorageRepository.Update(product);
                        }
                    }
                }

                else if (input.OrderStatus == 3)
                {
                    exportImport.OrderStatus = input.OrderStatus;
                    await _expensesRepository.UpdateAsync(exportImport);
                }
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

        public async Task<PagedResultDto<ExpensesProductDto>> GetProductAsync(ProductPagedRequest input)
        {
            try
            {
                var exportImport = await _productRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id.Contains(input.Keyword) || e.ProductName.Contains(input.Keyword))
                    .Select(e => new ExpensesProductDto
                    {
                        ProductId = e.Id,
                        ProductName = e.ProductName,
                        Quantity = 0,
                        Price = e.Price,
                        Unit = e.Unit,
                        FinalPrice = 0,
                    }).PageBy(input).ToListAsync();

                int totalCount = await _productRepository.CountAsync();

                return new PagedResultDto<ExpensesProductDto>
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

        public async Task<PagedResultDto<ExpensesGetAllDto>> GetAllAsync(ExpensesPagedResultInput input)
        {
            try
            {
                var expensesList = new List<ExpensesGetAllDto>();
                if (input.Keyword != null)
                {
                    expensesList = await _expensesRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.Id == input.Keyword)
                            .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                            .PageBy(input).Select(e => new ExpensesGetAllDto
                            {
                                ExpensesCode = e.Id,
                                ProductProvider = e.ProductProvider,
                                StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                                OrderStatus = e.OrderStatus,
                                Description = e.Description,
                                EmployeeCode = _employeeRepository.GetAll().FirstOrDefault(p => p.Id == e.OrderCreator).EmployeeName,
                                TotalPrice = e.TotalPrice,
                                PaymentDate = e.PaymentDate,
                                TotalPriceAfterDiscount = e.TotalPrice - (e.TotalPrice * (e.Discount / 100)),
                                CreationTime = e.CreationTime,
                            }).ToListAsync();
                } else
                {
                    if (input.DateTime != null)
                    {
                        var firstDate = DateTime.Parse(input.DateTime[0]);
                        var endDate = DateTime.Parse(input.DateTime[1]);

                        expensesList = await _expensesRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage)
                            .Where(e => e.CreationTime >= firstDate && e.CreationTime <= endDate)
                            .PageBy(input).Select(e => new ExpensesGetAllDto
                            {
                                ExpensesCode = e.Id,
                                ProductProvider = e.ProductProvider,
                                StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                                OrderStatus = e.OrderStatus,
                                Description = e.Description,
                                TotalPrice = e.TotalPrice,
                                EmployeeCode = _employeeRepository.GetAll().FirstOrDefault(p => p.Id == e.OrderCreator).EmployeeName,
                                PaymentDate = e.PaymentDate,
                                TotalPriceAfterDiscount = e.TotalPrice - (e.TotalPrice * (e.Discount / 100)),
                                CreationTime = e.CreationTime,
                            }).ToListAsync();
                    } else
                    {
                        expensesList = await _expensesRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Storage), e => e.StorageId == input.Storage)
                            .WhereIf(input.OrderStatus != 0, e => e.OrderStatus == input.OrderStatus)
                            .PageBy(input).Select(e => new ExpensesGetAllDto
                            {
                                ExpensesCode = e.Id,
                                ProductProvider = e.ProductProvider,
                                StorageName = _storageRepository.GetAll().FirstOrDefault(p => p.Id == e.StorageId).StorageName,
                                OrderStatus = e.OrderStatus,
                                Description = e.Description,
                                TotalPrice = e.TotalPrice,
                                PaymentDate = e.PaymentDate,
                                EmployeeCode = _employeeRepository.GetAll().FirstOrDefault(p => p.Id == e.OrderCreator).EmployeeName,
                                TotalPriceAfterDiscount = e.TotalPrice - (e.TotalPrice * (e.Discount / 100)),
                                CreationTime = e.CreationTime,
                            }).ToListAsync();
                    }
                }

                int totalCount = await _expensesRepository.CountAsync();

                return new PagedResultDto<ExpensesGetAllDto>
                {
                    Items = expensesList,
                    TotalCount = totalCount
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<ExpensesInputDto> GetAsync(string id)
        {
            try
            {
                var expense = await _expensesRepository.FirstOrDefaultAsync(e => e.Id == id);
                if (expense == null)
                {
                    throw new UserFriendlyException("Không thể tìm thấy đơn với mã này");
                }

                var productList = _productExpensesReposistory.GetAll()
                    .Where(e => e.ExpensesId == expense.Id)
                    .Select(e => new ExpensesProductDto
                    {
                        ProductId = e.ProductCode,
                        ProductName = e.Product.ProductName,
                        Quantity = e.Quantity,
                        Price = e.Price,
                        Unit = e.Product.Unit,
                        Location = e.Location,
                        FinalPrice = e.FinalPrice,
                    }).ToList();

                var expenseOutput = new ExpensesInputDto
                {
                    ExpensesCode = expense.Id,
                    EmployeeCode = _employeeRepository.GetAll().FirstOrDefault(e => e.Id == expense.OrderCreator).EmployeeName,
                    OrderStatus = expense.OrderStatus,
                    ProductProvider = expense.ProductProvider,
                    Products = productList,
                    StorageId = _storageRepository.GetAll().FirstOrDefault(e => e.Id == expense.StorageId).StorageName,
                    Discount = expense.Discount,
                    PaymentDate = expense.PaymentDate,
                    Description = expense.Description,
                    TotalPrice = expense.TotalPrice,
                };

                return expenseOutput;

            } catch (Exception ex) {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
