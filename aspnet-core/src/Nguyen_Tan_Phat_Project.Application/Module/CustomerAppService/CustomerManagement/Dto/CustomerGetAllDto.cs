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
        public string OrgCustomerCode { get; set; }
        public string OrgCustomerName { get; set; }
        public string OrgCustomerPhone { get; set; }
        public string OrgCustomerAddress { get; set; }
        public string OrgCustomerWebsite { get; set; }
        public BankAccount OrgCustomerBankAccount { get; set; }
    }
}
