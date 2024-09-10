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
        public JsonResult EditQuantity(int ProductCId, int RegCId, int CurrentQuantity, string action)
        {
            int newQuantity = action == "Increase" ? CurrentQuantity + 0 : CurrentQuantity - 0;
            _repository.EditQuantity(newQuantity, RegCId, ProductCId);

            var totalAmount = _repository.GetAllCartData()
                .Where(x => x.RegCId == RegCId)
                .Sum(x => x.TotalPrice);

            return Json(new { totalAmount = totalAmount });
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

            var orderd = _repository.ViewOrders().Where(x => x.RegOId == CurrentUserId);
            var orderId = orderd.Select(x => x.OrderId).FirstOrDefault();
            return RedirectToAction("ThankYou", new { id = orderId });
        }

        public ActionResult ThankYou(int id)
        {
            var order = _repository.GetOrderById(id);
            var a = order.FirstOrDefault();

            if (a == null)
            {
                return NotFound();
            }
            DateTime orderDate;
            bool isDateParsed = DateTime.TryParse(a.OrderDate, out orderDate);

            if (!isDateParsed)
            {
                orderDate = DateTime.Now;
            }
          
            var deliveryDate = orderDate.AddDays(7);
            ViewBag.OrderId = id;
            ViewBag.TotalAmount = a.TotalAmount;
            ViewBag.OrderDate = orderDate.ToString("MMMM d, yyyy");
            ViewBag.DeliveryDate = deliveryDate.ToString("MMMM d, yyyy");
            ViewBag.DeliveryInfo = "Your order will be delivered within 7 business days.";

            return View(a);
        }



    }
}
