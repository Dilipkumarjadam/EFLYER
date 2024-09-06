using EFLYER.DTOs;
using EFLYER.Models;
using EFLYER.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace EFLYER.Controllers
{
    public class HomeController : Controller
    {

        private readonly IEflyerRepository _eflyerRepository;

        public HomeController(IEflyerRepository eflyerRepository)
        {
            _eflyerRepository = eflyerRepository;
        }


        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
               
                // Fetch all products
                var allProducts = _eflyerRepository.GetProduct();

                // Categorize products
                var Model = new ProductsViewModel
                {
                    Electronics = allProducts.Where(p => p.CategoryName.Equals("Electronic", StringComparison.OrdinalIgnoreCase)),
                    Jewellery = allProducts.Where(p => p.CategoryName.Equals("Jewellery", StringComparison.OrdinalIgnoreCase)),
                    Fashion = allProducts.Where(p => p.CategoryName.Equals("Fashion", StringComparison.OrdinalIgnoreCase))
                };
                return View(Model);
            }
            else
            {
                return RedirectToAction("Login","Account");
            }

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
