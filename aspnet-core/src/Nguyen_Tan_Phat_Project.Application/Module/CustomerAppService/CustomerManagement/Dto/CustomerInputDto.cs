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
        public string CustomerCode { get; set; }
        public string TaxIdentification { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerWebsite { get; set; }
        public string CustomerDescription { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
