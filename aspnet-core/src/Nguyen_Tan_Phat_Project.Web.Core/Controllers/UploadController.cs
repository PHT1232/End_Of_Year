using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Global;
using Nguyen_Tan_Phat_Project.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Libary;
using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Entities;
using Abp.Auditing;
using Nguyen_Tan_Phat_Project.Roles.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement;
using Abp.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly string HashSecret = "WHOHZLSSRWPOJRSLOLBQFDJSGSFCZTHI";
        private readonly IVnPayService _vnPayService;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<Retail, string> _retailRepository;
        private readonly IRepository<RetailProduct> _retailProductRepository;
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public UploadController(IAppFolders appFolders
            , IVnPayService vnPayService
            , IRepository<Product, string> productRepository
            , IRepository<User, long> userRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<Customer, string> customerRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<Retail, string> retailRepository
            , IRepository<RetailProduct> retailProductRepository
            )
        {
            _appFolders = appFolders;
            _vnPayService = vnPayService;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _storageRepository = storageRepository;
            _productStorageRepository = productStorageRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _customerRepository = customerRepository;
            _exportImportCustomerRepository = exportImportCustomerRepository;
            _employeeRepository = employeeRepository;
            _retailRepository = retailRepository;
            _retailProductRepository = retailProductRepository;
        }

        [HttpPost]
        public async Task<List<string>> DemoUpload()
        {
            string fileFolderPath = Path.Combine(_appFolders.DemoUploadFolder + @"\" + string.Format("{0:yyyyMMdd_hhmmss}", DateTime.Now));
            if (Request.Form.Files.Count > 0)
            {
                fileFolderPath = fileFolderPath.Replace(@"\", @"/");
            }
            return await Upload(fileFolderPath);
        } 
        
        [HttpPost]
        public async Task<List<string>> ProductUpload(string id)
        {
            string fileFolderPath = Path.Combine(_appFolders.ProductUploadFolder + @"\", id);
            if (Request.Form.Files.Count > 0)
            {
                fileFolderPath = fileFolderPath.Replace(@"\", @"/");
            }
            return await Upload(fileFolderPath);
        }

        private async Task<List<string>> Upload(string fileFolderPath)
        {
            List<string> files = new List<string>();
            fileFolderPath = Path.Combine(fileFolderPath);

            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
            {
                return files;
            }
            foreach (var file in Request.Form.Files)
            {
                files.Add(GlobalFunction.SaveFile(fileFolderPath, file));
            }

            return await Task.FromResult(files);
        }

        [HttpPost]
        public async Task<string> DownloadFileUpdload(string linkFile)
        {
            if (string.IsNullOrEmpty(linkFile))
            {
                throw new UserFriendlyException("Null Parameter");
            }

            var fileName = linkFile.Split(@"/").Last();
            var path = this._appFolders.DemoUploadFolder + linkFile;
            return path;
        }

        [HttpGet]
        public string CreatePaymentUrl(string id)
        {
            var url = _vnPayService.CreatePaymentUrl(id);

            return url;
        }

        [HttpGet]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.Success == true)
            {
                RetailInputDto input = new RetailInputDto();
                input.RetailCode = response.OrderId;
                input.OrderStatus = 2;

                var retail = _retailRepository.FirstOrDefault(e => e.Id == input.RetailCode);
                var retailProduct =  _retailProductRepository.GetAll()
                    .Where(e => e.RetailId == input.RetailCode)
                    .ToList();
                retail.OrderStatus = input.OrderStatus;
                _retailRepository.Update(retail);

                foreach (var productRetail in retailProduct)
                {
                    var product = _productStorageRepository.FirstOrDefault(e => e.StorageId == productRetail.StorageId && e.ProductId == productRetail.ProductId);
                    if (product != null)
                    {
                        product.ProductQuantity -= productRetail.Quantity;
                        _productStorageRepository.Update(product);
                    }
                }
            } else if (response.Success == false) 
            { 
                return Redirect("http://unten.tech/quantri/");
            }

            return Redirect("http://unten.tech/quantri/");
        }

        //public IActionResult PaymentCallback()
        //{
        //    var response = PaymentExecute(Request.Query);

        //    return Json(response);
        //}

        //public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        //{
        //    var pay = new VnPayLibary();
        //    var response = pay.GetFullResponseData(collections, HashSecret);

        //    return response;
        //}

    }

}

