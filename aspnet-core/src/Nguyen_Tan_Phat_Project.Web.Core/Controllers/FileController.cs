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
using IronBarCode;
using System.Threading;
using System.Threading.Tasks;
using static Nguyen_Tan_Phat_Project.Module.ExcelExport.ExcelFileGenerator;
using QRCoder;
using System.Drawing;
using Nguyen_Tan_Phat_Project.Module.ExcelExport.Dtos;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    public class FileController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Expenses, string> _expensesRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;

        public FileController(IAppFolders appFolders
            , IRepository<Expenses, string> expensesRepository
            , IRepository<Product, string> productRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<Customer, string> customerRepository
            , IRepository<ProductStorage> productStorageRepository
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
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
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
                       StorageId = e.StorageId,
                       ProductName = e.Product.ProductName,
                       Quantity = e.Quantity,
                       Location = _productStorageRepository.GetAll().FirstOrDefault(x => x.ProductId == e.ProductId && x.StorageId == e.StorageId).ProductLocation,
                       Price = e.Product.Price,
                       Unit = e.Product.Unit,
                       FinalPrice = e.Product.Price * e.Quantity
                   }).ToList();

                var exportImport = _exportImportRepository.FirstOrDefault(e => e.Id == id);
                var exportImportCustomer = _exportImportCustomerRepository.FirstOrDefault(e => e.ExportImportCode == id);
                //exportImport.Storage = _storageRepository.FirstOrDefault(e => e.Id == exportImport.StorageId);

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
        public ActionResult ExcelExportForXuatHang(string id)
        {
            try
            {
                var product = _exportImportProductRepository.GetAll()
                    .Include(e => e.Product)
                    .Where(e => e.ExportImportCode == id)
                    .GroupBy(e => e.ProductId)
                    .Select(g => new ExportImportProductDto
                    {
                        ProductId = g.Key,
                        StorageId = g.First().StorageId,
                        ProductName = g.First().Product.ProductName,
                        Quantity = g.Sum(e => e.Quantity),
                        Location = g.First().Location,
                        Price = g.First().Product.Price,
                        Unit = g.First().Product.Unit,
                        FinalPrice = g.First().Product.Price * g.Sum(e => e.Quantity)
                    });

                var productList = product.ToList();

                var exportImport = _exportImportRepository.FirstOrDefault(e => e.Id == id);
                var exportImportCustomer = _exportImportCustomerRepository.FirstOrDefault(e => e.ExportImportCode == id);
                //exportImport.Storage = _storageRepository.FirstOrDefault(e => e.Id == exportImport.StorageId);

                var customer = _customerRepository.FirstOrDefault(e => e.Id == exportImportCustomer.CustomerCode);
                customer.CustomerPhone = exportImportCustomer.PhoneToCall;
                customer.CustomerAddress = exportImportCustomer.ReciveAddress;

                var employee = _employeeRepository.FirstOrDefault(e => e.Id == exportImport.OrderCreator);
                var deliveryEmployee = _employeeRepository.FirstOrDefault(e => e.Id == exportImport.DeliveryEmployee);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Phiếu Xuất hàng: " + exportImportCustomer.ExportImportCode + ".xlsx";
                ExcelFileGenerator exf = new ExcelFileGenerator();
                byte[] temp = exf.GenerateDeliveryExcel(productList, exportImport, customer, employee, deliveryEmployee);
                return File(temp, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [DisableAuditing]
        public IActionResult ExcelExportForSalary(string structureId, string date)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date);
                var exportImport = _exportImportRepository.GetAll()
                    .Where(e => e.StructureId == structureId && e.CreationTime.Month <= dateTime.Month && e.CreationTime.Month >= (dateTime.Month - 1) && e.OrderStatus == 2)
                    .ToList();

                var employee = _employeeRepository.GetAll()
                    .Where(e => e.WorkUnitId == structureId)
                    .ToList();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Bảng lương tháng " + dateTime.Month + ".xlsx";
                ExcelFileGenerator exf = new ExcelFileGenerator();
                byte[] temp = exf.GenerateExcelExportSalary(employee, exportImport, dateTime);
                return File(temp, contentType, fileName);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [DisableAuditing]
        public ActionResult ExcelExportForBaoGia(byte[] fileBytes)
        {
            try
            {
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Báo giá.xlsx";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
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
            var filePath = "";
            if (fileName == null || fileName == "null")
            {
                filePath = Path.Combine(_appFolders.ProductUploadFolder + @"\", "Default\\no-image.png");
            }
            else
            {
                filePath = Path.Combine(_appFolders.ProductUploadFolder + @"\", fileName);
            }


            //FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Byte[] buffer = System.IO.File.ReadAllBytes(filePath);
            var file = File(buffer, "image/jpeg");
            return file;
        }

        [HttpGet]
        public IActionResult GenerateQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode bm = new BitmapByteQRCode(qrCodeData);
            Byte[] qrCodeImage = bm.GetGraphic(20);
            var file = File(qrCodeImage, "image/jpeg");
            return file;
            //qrCodeImage.Save("qrcode.png", ImageFormat.Png);
        }
    }
}
