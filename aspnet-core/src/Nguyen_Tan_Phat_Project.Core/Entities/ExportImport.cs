using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class ExportImport : FullAuditedEntity<string>
    {
        public string NameOfReceiver { get; set; }
        public string OrderCreator { get; set; }
        public string DeliveryEmployee { get; set; }
        public int OrderStatus { get; set; }
        public bool IsDelivered { get; set; }
        public int OrderType { get; set; }
        [ForeignKey("Storage")]
        public string StorageId { get; set; }
        public string StorageInputId { get; set; }
        public Storage Storage { get; set; }
        public string NameOfExport { get; set; }
        public string Description { get; set; }
        public float TotalPrice { get; set; }
        public float Discount { get; set; }
    }
}
