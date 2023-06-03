using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class RetailCustomer : Entity
    {
        [ForeignKey("Retail")]
        public string RetailCode { get; set; }
        public Retail Retail { get; set; }
        [ForeignKey("Customer")]
        public string CustomerCode { get; set; }
        public Customer Customer { get; set; }
        public string ReciveAddress { get; set; }
        public float Discount { get; set; }
        public string PhoneToCall { get; set; }
    }
}
