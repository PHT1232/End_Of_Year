using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ExportImportManagement.Dto
{
    public class ExportImportPagedResultInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public DateTime[] DateTime { get; set; }
        public string Storage { get; set; }
        public string NameOfReciever { get; set; }
        public int OrderStatus { get; set; }
    }
}
