﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CustomerAppService.CustomerManagement.Dto
{
    public class CustomerPagedResultInput : PagedResultRequestDto
    {
        public string KeyWord { get; set; }
        public bool isRetail { get; set; }
    }
}
