using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Libary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module
{
    public class VnPayService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly string TmnCode = "4EDHWABO";
        private readonly string HashSecret = "WHOHZLSSRWPOJRSLOLBQFDJSGSFCZTHI";
        private readonly string BaseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string Command = "pay";
        private readonly string CurrCode = "VND";
        private readonly string Version = "2.1.0";
        private readonly string Locale = "vn";
        private readonly string TimeZoneId = "SE Asia Standard Time";
        private readonly string ReturnUrl = "https://localhost:44311/api/services/app/VnPayService/PaymentExecute";
        private HttpContext httpContext => _httpContextAccessor.HttpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string CreatePaymentUrl(PaymentInformationModel model)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var timeNow = DateTime.Now;
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibary();
            var urlCallBack = ReturnUrl;

            pay.AddRequestData("vnp_Version", Version);
            pay.AddRequestData("vnp_Command", Command);
            pay.AddRequestData("vnp_TmnCode", TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", CurrCode);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
            pay.AddRequestData("vnp_Locale", Locale);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

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
