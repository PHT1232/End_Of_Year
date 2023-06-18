using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    internal class VnPayController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IExportImportAppService _exportImportAppService;

        public VnPayController(IVnPayService vnPayService
            , IExportImportAppService exportImportAppService)
        {
            _vnPayService = vnPayService;
            _exportImportAppService = exportImportAppService;
        }

        public IActionResult Index()
        {
            return View();
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
                ExportImportInput input = new ExportImportInput();
                input.ExportImportCode = response.OrderId;
                input.OrderStatus = 2;
                _exportImportAppService.UpdateOrderAsync(input);
            }
            return Json(response);
        }
    }
}
