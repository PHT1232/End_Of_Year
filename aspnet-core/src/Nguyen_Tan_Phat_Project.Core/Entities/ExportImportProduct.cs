using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class ExportImportProduct : FullAuditedEntity
    {
        [ForeignKey("ExportImport")]
        public string ExportImportCode { get; set; }
        public ExportImport ExportImport { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; } 
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float FinalPrice { get; set; }
        public string Location { get; set; }
    }
}
