using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos
{
    public class EmployeePagedResultInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public string UnitCode { get; set; }
    }
}
