using EFLYER.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.Repository.IRepository
{
    public interface IOrderRepository
    {
        #region----------------------------ADD METHODS IMPLEMENTATION---------------------------------------------
        public void AddToCart(OrderDTO order);
        public void DeleteCartItem(int CartId);
        public decimal GetProductPrice(int productCId);
        public void EditQuantity(int NewQuantity, int RegCId, int ProductCId);
        public void AddOrder(int UserId, decimal TotalAmount);
        #endregion

        #region----------------------------GET METHODS IMPLEMENTATION---------------------------------------------
        public List<OrderDTO> GetAllCartData();
        public List<OrderDTO> ViewOrders();
        public List<OrderDTO> GetOrderById(int id);
        #endregion
    }
}
