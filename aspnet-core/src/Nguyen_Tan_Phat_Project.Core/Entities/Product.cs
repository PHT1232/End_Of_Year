using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Product : FullAuditedEntity<string>
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDetail { get; set; }
        public float Price { get; set; }
        [ForeignKey("Category")]
        public string CategoryId { get; set; }
        public Category Category { get; set; }
        [ForeignKey("SubCategory")]
        public int? SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
        public string Unit { get; set; }
    }
}
