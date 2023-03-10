using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class ProductStorage : FullAuditedEntity
    {
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Storage")]
        public string StorageId { get; set; }
        public Storage Storage { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductLocation { get; set; }
        public string Description { get; set; }
    }
}
