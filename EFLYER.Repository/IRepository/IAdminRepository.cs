using EFLYER.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.Repository
{
    public interface IAdminRepository
    {
        public void AddProduct(ProductDTO productDTO);
        public List<ProductDTO> GetProduct();
        public List<DropDownDTO> GetCategory();
        public bool EditProduct(ProductDTO ProductDTO);
        public List<ProductDTO> EditProductGetData();
        public void DeleteProduct(int Id);
       
    }
}
