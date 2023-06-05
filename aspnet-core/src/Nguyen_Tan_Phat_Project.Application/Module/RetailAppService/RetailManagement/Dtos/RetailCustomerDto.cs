using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos
{
    public class RetailCustomerDto
    {
        public string RetailCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string RevciveAddress { get; set; }
        public float Discount { get; set; } 
        public string PhoneToCall { get; set; }
    }
}
