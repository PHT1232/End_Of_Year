using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.InventoryManagement.CategoryManagement.Dto
{
    public class CategoryPagedResultInput : PagedResultRequestDto
    {
        public string KeyWord { get; set; }
    }
}
