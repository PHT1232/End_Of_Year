﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Expenses : FullAuditedEntity<string>
    {
        public string OrderCreator { get; set; }
        public int OrderStatus { get; set; }
        public string ProductProvider { get; set; }
        [ForeignKey("Storage")]
        public string StorageId { get; set; }
        public Storage Storage { get; set; }
        public DateTime PaymentDate { get; set; }
        public float Discount { get; set; }
        public string Description { get; set; }
        public string DescriptionForDenied { get; set; }
        public float TotalPrice { get; set; }
    }
}
