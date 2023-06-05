using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos
{
    public class RetailProductDto
    {
        public string RetailId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float FinalPrice { get; set; }

    }
}
