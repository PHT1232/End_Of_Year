using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class OrgCustomer : FullAuditedEntity<string>
    {
        public string TaxIdentification { get; set; }
        public string OrgCustomerName { get; set; }
        public string OrgCustomerEmail { get; set; }
        public string OrgCustomerPhone { get; set; }
        public string OrgCustomerAddress { get; set; }
        public string OrgCustomerWebsite { get; set; }
        //public string 
    }
}
