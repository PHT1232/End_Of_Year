using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class ProductPagedRequest : PagedResultRequestDto
    {
        public string StorageId { get; set; }
        public bool IsInsert { get; set; }
    }
}
