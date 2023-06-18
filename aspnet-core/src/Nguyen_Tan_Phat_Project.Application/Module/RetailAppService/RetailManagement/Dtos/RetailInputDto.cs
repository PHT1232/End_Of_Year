using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos
{
    public class RetailInputDto
    {
        public RetailCustomerDto Customer { get; set; }
        public List<RetailProductDto> Products { get; set; }
        public string RetailCode { get; set; }
        public string OrderCreator { get; set; }
        public string DeliveryEmployee { get; set; }
        public int OrderStatus { get; set; }
        public string StructureId { get; set; }
        public string Description { get; set; }
        public int PaymentMethod { get; set; }
        public bool IsHomeDelivery { get; set; }
        public bool IsDelivered { get; set; }
        public float TotalPrice { get; set; }
        public float Discount { get; set; }
    }
}
