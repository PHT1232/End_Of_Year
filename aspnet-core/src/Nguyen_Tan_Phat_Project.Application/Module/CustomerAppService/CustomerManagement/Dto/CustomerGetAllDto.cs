using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CustomerAppService.CustomerManagement.Dto
{
    public class CustomerGetAllDto
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerWebsite { get; set; }
        public float Discount { get; set; }
        public string StructureName { get; set; }
        public string CustomerBankId { get; set; }
        public string CustomerBankName { get; set; }
    }
}
