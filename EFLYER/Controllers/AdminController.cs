using EFLYER.DTOs;
using EFLYER.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFLYER.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        // GET: AdminController
        public ActionResult AdminIndex()
        {
            return View();
        }

        public ActionResult GetProduct()
        {
            var row = _adminRepository.GetProduct();
            return View(row);
        }

        // GET: AdminController/Create
        public ActionResult CreateProduct()
        {
            var obj = _adminRepository.GetCategory();
            ViewBag.Category = new SelectList(obj, "CategoryId", "CategoryName");
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(ProductDTO productDTO, IFormFile ProductImage)
        {
            var categories = _adminRepository.GetCategory(); // Fetch categories for dropdown
            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");

            try
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    // Determine the file path and ensure it exists
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductsImg");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file synchronously
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProductImage.CopyTo(stream); // Synchronously copy the file
                    }

                    productDTO.ProductImagePath = $"/ProductsImg/{fileName}"; // Set image path in DTO
                }

                _adminRepository.AddProduct(productDTO); // Add product to repository
                return RedirectToAction(nameof(AdminIndex));
            }
            catch
            {
                return View();
            }
        }


        // GET: AdminController/Edit/5
        public ActionResult EditProduct(int id)
        {
            var categories = _adminRepository.GetCategory(); // Fetch categories for dropdown
            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");
            var row = (_adminRepository.EditProductGetData().Find(P => P.ProductId == id));
            return View(row);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
       
        public ActionResult EditProduct(ProductDTO Product, IFormFile ProductImage)
        {
            try
            {
                var categories = _adminRepository.GetCategory(); // Fetch categories for dropdown
                ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    // Determine the file path and ensure it exists
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductsImg");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file synchronously
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProductImage.CopyTo(stream); // Synchronously copy the file
                    }

                    Product.ProductImagePath = $"/ProductsImg/{fileName}"; // Set image path in DTO
                }
                _adminRepository.EditProduct(Product);
                return RedirectToAction(nameof(AdminIndex));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult DeleteProduct(int id)
        {
            _adminRepository.DeleteProduct(id);
            return RedirectToAction("GetProduct");
        }

     public IActionResult GetUser()
        {
            return View();
        }
    }
}
