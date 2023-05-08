using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CustomerAppService.CustomerManagement.Dto
{
    public class CustomerInputDto
    {
        public string OrgCustomerCode { get; set; }
        public string TaxIdentification { get; set; }
        public string OrgCustomerName { get; set; }
        public string OrgCustomerEmail { get; set; }
        public string OrgCustomerPhone { get; set; }
        public string OrgCustomerAddress { get; set; }
        public string OrgCustomerWebsite { get; set; }
        public string OrgCustomerDescription { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
