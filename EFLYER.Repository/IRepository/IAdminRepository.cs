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
        #region----------------------------ADD METHODS IMPLEMENTATION---------------------------------------------
        public void AddProduct(ProductDTO productDTO);
        public bool EditProduct(ProductDTO ProductDTO);
        public void DeleteProduct(int Id);
        public void AddProductsBulk(List<ProductDTO> products);
        #endregion

        #region----------------------------GET METHODS IMPLEMENTATION---------------------------------------------
        public List<ProductDTO> GetProduct();
        public List<DropDownDTO> GetCategory();
        public List<ProductDTO> EditProductGetData();
        public List<OrderDTO> GetAllCartData();
        public List<OrderDTO> ViewOrders();
        #endregion


    }
}
