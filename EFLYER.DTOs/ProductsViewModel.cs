using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.DTOs
{
    public class ProductsViewModel
    {
        public IEnumerable<ProductDTO> Electronics { get; set; }
        public IEnumerable<ProductDTO> Jewellery { get; set; }
        public IEnumerable<ProductDTO> Fashion { get; set; }
    }
}
