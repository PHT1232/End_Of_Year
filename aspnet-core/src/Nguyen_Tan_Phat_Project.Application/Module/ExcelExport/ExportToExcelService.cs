using Abp.UI;
using Microsoft.JSInterop;
using Nguyen_Tan_Phat_Project.Global;
using Nguyen_Tan_Phat_Project.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nguyen_Tan_Phat_Project.Module.ExcelExport.ExcelFileGenerator;

namespace Nguyen_Tan_Phat_Project.Module.ExcelExport
{
    public class ExportToExcelService : Nguyen_Tan_Phat_ProjectAppServiceBase, IExportToExcelService
    {
        private readonly IAppFolders _appFolders;
        //private readonly JSRuntime _runtime;

        public ExportToExcelService(IAppFolders appFolders
            , JSRuntime runtime) {
           _appFolders = appFolders;
            //_runtime = runtime;
        }

        //public async Task ExportToExcel()
        //{
        //    var excelByte = Task.FromResult(GenerateExcelFile()).Result;
        //    await _runtime.InvokeVoidAsync("saveAsFile", $"test_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx",
        //        Convert.ToBase64String(excelByte));
        //}


        public async Task<string> String()
        {
            return "url";
        }
    }
}
