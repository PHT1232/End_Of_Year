using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Controllers
{
    [Route("api/[controller]/[action]")]
    internal class VnPayController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public VnPayController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }
    }
}
