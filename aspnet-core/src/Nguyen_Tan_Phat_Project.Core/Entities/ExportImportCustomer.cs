using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class ExportImportCustomer : FullAuditedEntity
    {
        [ForeignKey("ExportImport")]
        public string ExportImportCode { get; set; }
        public ExportImport ExportImport { get; set; }
        [ForeignKey("Customer")]
        public string CustomerCode { get; set; }
        public Customer Customer { get; set; }
        public string ReciveAddress { get; set; }
        public float Discount { get; set; }
        public string PhoneToCall { get; set; }
    }
}
