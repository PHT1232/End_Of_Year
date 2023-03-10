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
        public int OrderStatus { get; set; }
        public string ReceiveAddress { get; set; }
        [ForeignKey("Storage")]
        public string StorageId { get; set; }
        public Storage Storage { get; set; }
    }
}
