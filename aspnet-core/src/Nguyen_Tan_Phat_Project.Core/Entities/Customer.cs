using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Customer : FullAuditedEntity<string>
    {
        public string TaxIdentification { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerWebsite { get; set; }
        public string CustomerDescription { get; set; }
        [ForeignKey("Structure")]
        public string StructureCode { get; set; }
        public float Discount { get; set; }
        [ForeignKey("BankAccount")]
        public string BankId { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
