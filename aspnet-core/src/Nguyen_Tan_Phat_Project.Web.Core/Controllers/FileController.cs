using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.UI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Nguyen_Tan_Phat_Project.Entities;
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
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;

        public FileController(IAppFolders appFolders
            , IRepository<Expenses, string> expensesRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            )
        {
            _appFolders = appFolders;
            _exportImportProductRepository = exportImportProductRepository;
            _expensesRepository = expensesRepository;
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
        }

        [DisableAuditing]
        public ActionResult ExcelExport()
        {
            var product = _exportImportProductRepository.GetAll().Include(e => e.Product)
                   .Select(e => new ExportImportProductDto
                   {
                       ProductId = e.ProductId,
                       ProductName = e.Product.ProductName,
                       Quantity = e.Quantity,
                       Price = e.Product.Price,
                       Unit = e.Product.Unit,
                       FinalPrice = e.Product.Price * e.Quantity
                   }).ToList();

            Response.Headers.Add("content-disposition", "attachment; filename=Information" + DateTime.Now.Year.ToString() + ".xlsx");
            this.Response.ContentType = "application/vnd.ms-excel";
            byte[] temp = GenerateExcelFileTest(product);
            return File(temp, "application/vnd.ms-exce");
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
    }
}
