using EFLYER.DTOs;
using EFLYER.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace EFLYER.Controllers
{
    public class ProductController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public ProductController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public ActionResult Electronic()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                var products = _adminRepository.GetProduct();
                var filteredProducts = products.Where(p => p.CategoryPId == 3).ToList();
                return View(filteredProducts);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Jewellery()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                var products = _adminRepository.GetProduct();
                var filteredProducts = products.Where(p => p.CategoryPId == 2).ToList();
                return View(filteredProducts);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Fashion()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                var products = _adminRepository.GetProduct();
                var filteredProducts = products.Where(p => p.CategoryPId == 1).ToList();
                return View(filteredProducts);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
