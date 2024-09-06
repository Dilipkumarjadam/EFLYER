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
        public void AddToCart(OrderDTO order);
        public List<OrderDTO> GetAllCartData();
        public void DeleteCartItem(int CartId);
        public decimal GetProductPrice(int productCId);
        public void EditQuantity(int NewQuantity, int RegCId, int ProductCId);
    }
}
