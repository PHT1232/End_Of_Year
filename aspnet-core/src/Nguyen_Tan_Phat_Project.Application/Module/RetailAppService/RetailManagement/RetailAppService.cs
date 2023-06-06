using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement
{
    public class RetailAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Retail, string> _retailRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;
        private readonly IRepository<RetailProduct> _retailProductRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public RetailAppService(IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Product, string> productRepository
            , IRepository<Retail, string> retailRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            , IRepository<RetailProduct> retailProductRepository
            )
        {
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
            _retailRepository = retailRepository;
            _retailProductRepository = retailProductRepository;
            _retailCustomerRepository = retailCustomerRepository;
        }

        public async Task AddNewAsync(RetailInputDto input)
        {
            try
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == input.RetailCode);
                if (retail != null)
                {
                    throw new UserFriendlyException("Đơn này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                retail = new Retail
                {
                    Id = input.RetailCode,
                    StructureId = input.StructureId,
                    NameOfReceiver = input.Customer.CustomerName,
                    OrderCreator = input.OrderCreator,
                    DeliveryEmployee = input.DeliveryEmployee,
                    OrderStatus = 1,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                    Discount = input.Discount,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _retailRepository.InsertAndGetIdAsync(retail);

                var customer = new RetailCustomer
                {
                    RetailCode = id,
                    CustomerCode = input.Customer.CustomerCode,
                    ReciveAddress = input.Customer.ReveciveAddress,
                    Discount = input.Discount,
                    PhoneToCall = input.Customer.PhoneToCall,
                };

                await _retailCustomerRepository.InsertAsync(customer);

                foreach (var product in input.Products)
                {
                    var exportImportProduct = new RetailProduct
                    {
                        RetailId = id,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                    };
                    _retailProductRepository.Insert(exportImportProduct);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        //[AbpAllowAnonymous]
        //public async Task UpdateOrderAsync(ExportImportInput input)
        //{
        //    try
        //    {
        //        var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == input.ExportImportCode);
        //        if (exportImport.OrderType == 1 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == input.ExportImportCode)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);

        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
        //                product.ProductQuantity -= productExport.Quantity;
        //                _productStorageRepository.Update(product);
        //            }
        //        }

        //        else if (exportImport.OrderType == 2 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == input.ExportImportCode)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);


        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
        //                var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
        //                if (productInput == null)
        //                {
        //                    productInput = new ProductStorage
        //                    {
        //                        StorageId = exportImport.StorageInputId,
        //                        ProductId = productExport.ProductId,
        //                        ProductLocation = productExport.Location,
        //                        ProductQuantity = productExport.Quantity,
        //                    };
        //                    _productStorageRepository.Insert(productInput);
        //                }
        //                else
        //                {
        //                    productInput.ProductQuantity += productExport.Quantity;
        //                    _productStorageRepository.Update(productInput);
        //                }
        //                productOutput.ProductQuantity -= productExport.Quantity;
        //                _productStorageRepository.Update(productOutput);
        //            }
        //        }

        //        else if (exportImport.OrderType == 3 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == input.ExportImportCode)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);


        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
        //                if (productInput == null)
        //                {
        //                    productInput = new ProductStorage
        //                    {
        //                        StorageId = exportImport.StorageInputId,
        //                        ProductId = productExport.ProductId,
        //                        ProductLocation = productExport.Location,
        //                        ProductQuantity = productExport.Quantity,
        //                    };
        //                    _productStorageRepository.Insert(productInput);
        //                }
        //                else
        //                {
        //                    productInput.ProductQuantity += productExport.Quantity;
        //                    _productStorageRepository.Update(productInput);
        //                }
        //            }
        //        }

        //        else if (exportImport.OrderType == 1 || exportImport.OrderType == 2 || exportImport.OrderType == 3 && input.OrderStatus == 3)
        //        {
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException(ex.Message);
        //    }
        //}

        //[AbpAllowAnonymous]
        //[DisableCors]
        //[DisableAuditing]
        //[HttpGet]
        //public async Task<ContentResult> UpdateOrderQRAsync(ExportImportInput input)
        //{
        //    try
        //    {
        //        var exportImport = await _exportImportRepository.FirstOrDefaultAsync(e => e.Id == input.ExportImportCode);
        //        if (exportImport.OrderType == 1 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == exportImport.Id)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);

        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
        //                product.ProductQuantity -= productExport.Quantity;
        //                _productStorageRepository.Update(product);
        //            }
        //        }

        //        else if (exportImport.OrderType == 2 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == input.ExportImportCode)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);


        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var productOutput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageId && e.ProductId == productExport.ProductId);
        //                var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
        //                if (productInput == null)
        //                {
        //                    productInput = new ProductStorage
        //                    {
        //                        StorageId = exportImport.StorageInputId,
        //                        ProductId = productExport.ProductId,
        //                        ProductLocation = productExport.Location,
        //                        ProductQuantity = productExport.Quantity,
        //                    };
        //                    _productStorageRepository.Insert(productInput);
        //                }
        //                else
        //                {
        //                    productInput.ProductQuantity += productExport.Quantity;
        //                    _productStorageRepository.Update(productInput);
        //                }
        //                productOutput.ProductQuantity -= productExport.Quantity;
        //                _productStorageRepository.Update(productOutput);
        //            }
        //        }

        //        else if (exportImport.OrderType == 3 && input.OrderStatus == 2)
        //        {
        //            var exportImportProduct = await _exportImportProductRepository.GetAll()
        //                .Where(e => e.ExportImportCode == input.ExportImportCode)
        //                .ToListAsync();
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);


        //            foreach (var productExport in exportImportProduct)
        //            {
        //                var productInput = _productStorageRepository.FirstOrDefault(e => e.StorageId == exportImport.StorageInputId && e.ProductId == productExport.ProductId);
        //                if (productInput == null)
        //                {
        //                    productInput = new ProductStorage
        //                    {
        //                        StorageId = exportImport.StorageInputId,
        //                        ProductId = productExport.ProductId,
        //                        ProductLocation = productExport.Location,
        //                        ProductQuantity = productExport.Quantity,
        //                    };
        //                    _productStorageRepository.Insert(productInput);
        //                }
        //                else
        //                {
        //                    productInput.ProductQuantity += productExport.Quantity;
        //                    _productStorageRepository.Update(productInput);
        //                }
        //            }
        //        }

        //        else if (exportImport.OrderType == 1 || exportImport.OrderType == 2 || exportImport.OrderType == 3 && input.OrderStatus == 3)
        //        {
        //            exportImport.OrderStatus = input.OrderStatus;
        //            await _exportImportRepository.UpdateAsync(exportImport);
        //        }

        //        var html = "<html>" +
        //            "<head>" +
        //            "<meta charset=\"utf-8\" />" +
        //            "<style>" +
        //            ".success-checkmark {\r\n        width: 80px;\r\n        height: 115px;\r\n        margin: 0 auto;\r\n\r\n        .check-icon {\r\n          width: 80px;\r\n          height: 80px;\r\n          position: relative;\r\n          border-radius: 50%;\r\n          box-sizing: content-box;\r\n          border: 4px solid #4caf50;\r\n\r\n          &::before {\r\n            top: 3px;\r\n            left: -2px;\r\n            width: 30px;\r\n            transform-origin: 100% 50%;\r\n            border-radius: 100px 0 0 100px;\r\n          }\r\n\r\n          &::after {\r\n            top: 0;\r\n            left: 30px;\r\n            width: 60px;\r\n            transform-origin: 0 50%;\r\n            border-radius: 0 100px 100px 0;\r\n            animation: rotate-circle 4.25s ease-in;\r\n          }\r\n\r\n          &::before,\r\n          &::after {\r\n            content: \"\";\r\n            height: 100px;\r\n            position: absolute;\r\n            background: #ffffff;\r\n            transform: rotate(-45deg);\r\n          }\r\n\r\n          .icon-line {\r\n            height: 5px;\r\n            background-color: #4caf50;\r\n            display: block;\r\n            border-radius: 2px;\r\n            position: absolute;\r\n            z-index: 10;\r\n\r\n            &.line-tip {\r\n              top: 46px;\r\n              left: 14px;\r\n              width: 25px;\r\n              transform: rotate(45deg);\r\n              animation: icon-line-tip 0.75s;\r\n            }\r\n\r\n            &.line-long {\r\n              top: 38px;\r\n              right: 8px;\r\n              width: 47px;\r\n              transform: rotate(-45deg);\r\n              animation: icon-line-long 0.75s;\r\n            }\r\n          }\r\n\r\n          .icon-circle {\r\n            top: -4px;\r\n            left: -4px;\r\n            z-index: 10;\r\n            width: 80px;\r\n            height: 80px;\r\n            border-radius: 50%;\r\n            position: absolute;\r\n            box-sizing: content-box;\r\n            border: 4px solid rgba(76, 175, 80, 0.5);\r\n          }\r\n\r\n          .icon-fix {\r\n            top: 8px;\r\n            width: 5px;\r\n            left: 26px;\r\n            z-index: 1;\r\n            height: 85px;\r\n            position: absolute;\r\n            transform: rotate(-45deg);\r\n            background-color: #ffffff;\r\n          }\r\n        }\r\n      }\r\n\r\n      @keyframes rotate-circle {\r\n        0% {\r\n          transform: rotate(-45deg);\r\n        }\r\n        5% {\r\n          transform: rotate(-45deg);\r\n        }\r\n        12% {\r\n          transform: rotate(-405deg);\r\n        }\r\n        100% {\r\n          transform: rotate(-405deg);\r\n        }\r\n      }\r\n\r\n      @keyframes icon-line-tip {\r\n        0% {\r\n          width: 0;\r\n          left: 1px;\r\n          top: 19px;\r\n        }\r\n        54% {\r\n          width: 0;\r\n          left: 1px;\r\n          top: 19px;\r\n        }\r\n        70% {\r\n          width: 50px;\r\n          left: -8px;\r\n          top: 37px;\r\n        }\r\n        84% {\r\n          width: 17px;\r\n          left: 21px;\r\n          top: 48px;\r\n        }\r\n        100% {\r\n          width: 25px;\r\n          left: 14px;\r\n          top: 45px;\r\n        }\r\n      }\r\n\r\n      @keyframes icon-line-long {\r\n        0% {\r\n          width: 0;\r\n          right: 46px;\r\n          top: 54px;\r\n        }\r\n        65% {\r\n          width: 0;\r\n          right: 46px;\r\n          top: 54px;\r\n        }\r\n        84% {\r\n          width: 55px;\r\n          right: 0px;\r\n          top: 35px;\r\n        }\r\n        100% {\r\n          width: 47px;\r\n          right: 8px;\r\n          top: 38px;\r\n        }\r\n      }" +
        //            "</style>" +
        //            "</head>" +
        //            "<body>" +
        //            "<div class=\"success-checkmark\" style=\"margin-top: 50px\">" +
        //            "<div class=\"check-icon\">" +
        //            "<span class=\"icon-line line-tip\"></span>" +
        //            "<span class=\"icon-line line-long\"></span>" +
        //            "<div class=\"icon-circle\"></div>" +
        //            "<div class=\"icon-fix\"></div>" +
        //            "</div>" +
        //            "</div>" +
        //            "<center>" +
        //            "<h2>Hoàn thành đơn</h2>" +
        //            "</center>" +
        //            "</body>" +
        //            "</html>";

        //        return new ContentResult
        //        {
        //            Content = html,
        //            ContentType = "text/html"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException(ex.Message);
        //    }
        //}
    }
}
