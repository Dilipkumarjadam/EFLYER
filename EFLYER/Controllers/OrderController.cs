using EFLYER.DTOs;
using EFLYER.Repository.IRepository;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace EFLYER.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _repository;

        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }
        // GET: OrderController
        public ActionResult CartIndex()
        {
            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            var obj = _repository.GetAllCartData().Where(x => x.RegCId == CurrentUserId);
            var totalAmount = obj.Sum(x => x.TotalPrice);
            ViewBag.TotalAmount = totalAmount;
            return View(obj);
        }

        // GET: OrderController/Create
        public ActionResult AddToCart()
        {
            var orderDetailsJson = TempData["OrderDetails"] as string;
            if (!string.IsNullOrEmpty(orderDetailsJson))
            {
                var dTO = JsonConvert.DeserializeObject<OrderDTO>(orderDetailsJson);
                if (dTO != null)
                {
                    _repository.AddToCart(dTO);
                }
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public ActionResult IsProductInCart(OrderDTO dTO)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var isInCart = _repository.GetAllCartData().Where(x => x.RegCId == currentUserId && x.ProductCId == dTO.ProductCId);
            if (isInCart.Count() < 1)
            {
                TempData["OrderDetails"] = JsonConvert.SerializeObject(dTO);
                return RedirectToAction("AddToCart");
            }
            else
            {
                return RedirectToAction("CartIndex");
            }
        }

        [HttpGet]
        public JsonResult GetCartQuantity()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
            {
                return Json(new { Quantity = 0 });
            }

            var cartItems = _repository.GetAllCartData().Where(x => x.RegCId == currentUserId);
            var quantity = cartItems.Sum(x => x.Quantity);

            return Json(new { Quantity = quantity });
        }



        [HttpPost]
        public ActionResult EditQuantity(int ProductCId, int RegCId, int CurrentQuantity, string action)
        {
            int newQuantity = action == "Increase" ? CurrentQuantity + 1 : CurrentQuantity - 1;
            _repository.EditQuantity(newQuantity, RegCId, ProductCId);
            return RedirectToAction("CartIndex");
        }



        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            _repository.DeleteCartItem(id);
            return RedirectToAction("CartIndex");
        }

        public ActionResult PlaceOrder()
        {
            var CurrentUserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var obj = _repository.GetAllCartData().Where(x => x.RegCId == CurrentUserId);
            var totalAmount = obj.Sum(x => x.TotalPrice);
            _repository.AddOrder(CurrentUserId, totalAmount);

            return View();
        }



    }
}
