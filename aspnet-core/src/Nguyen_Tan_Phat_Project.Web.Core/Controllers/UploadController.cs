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

namespace Nguyen_Tan_Phat_Project.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : Nguyen_Tan_Phat_ProjectControllerBase
    {
        private readonly IAppFolders _appFolders;
        private readonly string HashSecret = "WHOHZLSSRWPOJRSLOLBQFDJSGSFCZTHI";

        public UploadController(IAppFolders appFolders)
        {
            _appFolders = appFolders;
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

