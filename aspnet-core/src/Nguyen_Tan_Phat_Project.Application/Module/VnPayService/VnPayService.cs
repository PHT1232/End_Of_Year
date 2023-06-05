using Abp.Domain.Repositories;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Libary;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module
{
    public class VnPayService : Nguyen_Tan_Phat_ProjectAppServiceBase, IVnPayService
    {
        private readonly string TmnCode = "4EDHWABO";
        private readonly string HashSecret = "WHOHZLSSRWPOJRSLOLBQFDJSGSFCZTHI";
        private readonly string BaseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string Command = "pay";
        private readonly string CurrCode = "VND";
        private readonly string Version = "2.1.0";
        private readonly string Locale = "vn";
        private readonly string TimeZoneId = "SE Asia Standard Time";
        private readonly string ReturnUrl = "https://unten.tech:44311/api/Upload/PaymentCallback";
        private HttpContext httpContext => _httpContextAccessor.HttpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IExportImportAppService _exportImportAppService;

        public VnPayService(IHttpContextAccessor httpContextAccessor
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IExportImportAppService exportImportAppService
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _exportImportProductRepository = exportImportProductRepository;
            _exportImportRepository = exportImportRepository;
            _exportImportAppService = exportImportAppService;
        }

        public string CreatePaymentUrl(string id)
        {
            var model = _exportImportRepository.FirstOrDefault(e => e.Id == id);

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var timeNow = DateTime.Now;
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibary();
            var urlCallBack = ReturnUrl;

            var productStorage = _exportImportProductRepository.GetAll().Where(e => e.ExportImportCode == model.Id).ToList();

            var priceToPay = model.TotalPrice - (model.TotalPrice * (model.Discount / 100));

            pay.AddRequestData("vnp_Version", Version);
            pay.AddRequestData("vnp_Command", Command);
            pay.AddRequestData("vnp_TmnCode", TmnCode);
            pay.AddRequestData("vnp_Amount", ((Int64)priceToPay * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", CurrCode);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
            pay.AddRequestData("vnp_Locale", Locale);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Id} {productStorage.Select(e => e.Quantity).Sum()}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", model.Id);

            var paymentUrl = pay.CreateRequestUrl(BaseUrl, HashSecret);

            return paymentUrl;
        }

        //public PaymentResponseModel PaymentCallBack()
        //{
        //    var response = PaymentExecute(Request.Query);

        //}

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibary();
            var response = pay.GetFullResponseData(collections, HashSecret);

            return response;
        }

    }
}
