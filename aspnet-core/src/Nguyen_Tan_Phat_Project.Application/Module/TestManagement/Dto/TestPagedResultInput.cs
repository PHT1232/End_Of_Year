using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.TestManagement.Dto
{
    public class TestPagedResultInput : PagedResultRequestDto
    {
        public int DbContext { get; set; }
        public int testVarible { get; set; }
    }
}
