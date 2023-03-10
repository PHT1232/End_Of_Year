﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class ProductForUpdate
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDetail { get; set; }
        public float Price { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string Unit { get; set; }
    }
}
