using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.UI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.ExcelExport;
using Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Dtos;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nguyen_Tan_Phat_Project.Module.ExcelExport.ExcelFileGenerator;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    public class FileController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Expenses, string> _expensesRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;

        public FileController(IAppFolders appFolders
            , IRepository<Expenses, string> expensesRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<Customer, string> customerRepository
            )
        {
            _appFolders = appFolders;
            _expensesRepository = expensesRepository;
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportCustomerRepository = exportImportCustomerRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _customerRepository = customerRepository;
        }

        [DisableAuditing]
        public ActionResult ExcelExport(string id)
        {
            try
            {
                var product = _exportImportProductRepository.GetAll().Include(e => e.Product)
                   .Where(e => e.ExportImportCode == id)
                   .Select(e => new ExportImportProductDto
                   {
                       ProductId = e.ProductId,
                       ProductName = e.Product.ProductName,
                       Quantity = e.Quantity,
                       Price = e.Product.Price,
                       Unit = e.Product.Unit,
                       FinalPrice = e.Product.Price * e.Quantity
                   }).ToList();

                var exportImport = _exportImportRepository.FirstOrDefault(e => e.Id == id);
                var exportImportCustomer = _exportImportCustomerRepository.FirstOrDefault(e => e.ExportImportCode == id);
                exportImport.Storage = _storageRepository.FirstOrDefault(e => e.Id == exportImport.StorageId);

                var customer = _customerRepository.FirstOrDefault(e => e.Id == exportImportCustomer.CustomerCode);
                customer.CustomerPhone = exportImportCustomer.PhoneToCall;
                customer.CustomerAddress = exportImportCustomer.ReciveAddress;

                var employee = _employeeRepository.FirstOrDefault(e => e.Id == exportImport.OrderCreator);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Phiếu Xuất kho: " + exportImportCustomer.ExportImportCode + ".xlsx";
                ExcelFileGenerator exf = new ExcelFileGenerator();
                byte[] temp = exf.GenerateExcelFileForExportImport(product, exportImport, customer, employee);
                return File(temp, contentType, fileName);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(string file)
        {
            

            var filePath = Path.Combine(_appFolders.ExcelTemplateFolder, file);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, "application/vnd.ms-excel");
        }

        [HttpGet]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(_appFolders.ProductUploadFolder + @"\", fileName);
            var fileStream = new FileStream(filePath, FileMode.Open);
            return File(fileStream, "image/jpeg");
        }
    }
}
