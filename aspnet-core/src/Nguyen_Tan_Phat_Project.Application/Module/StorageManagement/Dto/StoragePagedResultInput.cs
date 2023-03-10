using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageManagement.Dto
{
    public class StoragePagedResultInput : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
