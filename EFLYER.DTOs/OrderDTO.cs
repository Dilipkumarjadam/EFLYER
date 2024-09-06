using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.DTOs
{
    public class OrderDTO
    {
        public int CartId { get; set; }
        public int ProductCId { get; set; }
        public int RegCId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerUnit { get; set; }
        public string ProductImagePath { get; set; }
        public int CategoryCId { get; set; }
        public IFormFile ProductImage { get; set; }
        public string CategoryName { get; set; }

    }
}
