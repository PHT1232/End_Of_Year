using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Retail : FullAuditedEntity<string>
    {
        public string OrderCreator { get; set; }
        public string OrderStatus { get; set; }
        [ForeignKey("Structure")]
        public string StructureId { get; set; }
        public Structure Structure { get; set; }
        public float TotalPrice { get; set; }
        public float Discount { get; set; }
    }
}
