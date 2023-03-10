using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class ProductPagedResultInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public string StorageCode { get; set; }
        public string CategoryCode { get; set; }
        public int SubCategoryCode { get; set; }
    }
}
