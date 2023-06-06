using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos
{
    public class RetailGetAllDto
    {
        public string RetailCode { get; set; }
        public string NameOfReceiver { get; set; }
        public string OrderCreator { get; set; }
        public int OrderStatus { get; set; }
        public string StructureId { get; set; }
        public bool IsDelivered { get; set; }
        public float TotalPrice { get; set; }
        public float TotalPriceAfterDiscount { get; set; }
    }
}
