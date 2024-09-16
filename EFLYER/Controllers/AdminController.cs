using EFLYER.DTOs;
using EFLYER.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

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

        public ActionResult ViewOrders()
        {
            var row = _adminRepository.ViewOrders();
            return View(row);
        }

        public ActionResult OrderDetails(int Id)
        {
            var row = _adminRepository.GetAllCartData().Where(x => x.RegCId == Id);
            return View(row);
        }


        public ActionResult GetProduct()
        {
            var row = _adminRepository.GetProduct();
            return View(row);
        }

        public ActionResult BulkUpload()
        {
            return View();
        }

        // POST: AdminController/BulkUpload
        [HttpPost]
        public async Task<ActionResult> BulkUpload(BulkUploadViewModel model)
        {
            if (model.FileUpload != null && model.FileUpload.Length > 0)
            {
                try
                {
                    using (var reader = new StreamReader(model.FileUpload.OpenReadStream()))
                    {
                        var content = await reader.ReadToEndAsync();
                        var products = ParseProducts(content);
                        _adminRepository.AddProductsBulk(products);
                    }
                    return RedirectToAction(nameof(AdminIndex));
                }
                catch (Exception ex)
                {
                    // Handle exception (e.g., log error, show message)
                    ModelState.AddModelError("", "An error occurred while uploading the file.");
                }
            }
            return View();
        }

        public sealed class ProductMap : ClassMap<ProductDTO>
        {
            public ProductMap()
            {
                Map(m => m.ProductName).Name("ProductName");
                Map(m => m.Description).Name("Description");
                Map(m => m.Price).Name("Price");
                Map(m => m.ProductImagePath).Name("ProductImagePath");
                Map(m => m.CategoryPId).Name("CategoryPId");
                // Optionally map other properties if needed
            }
        }

        private List<ProductDTO> ParseProducts(string fileContent)
        {
            var products = new List<ProductDTO>();

            using (var reader = new StringReader(fileContent))
            using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null, // Ignore header validation errors
                MissingFieldFound = null // Optionally, handle missing fields
            }))
            {
                csv.Context.RegisterClassMap<ProductMap>();
                products = csv.GetRecords<ProductDTO>().ToList();
            }

            return products;
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
            var p = row.ProductImagePath;
            HttpContext.Session.SetString("Path", p);
            return View(row);
        }

        // POST: AdminController/Edit/5
        [HttpPost]

        public ActionResult EditProduct(ProductDTO Product, IFormFile ProductImage)
        {
            try
            {
                var Imagepath = HttpContext.Session.GetString("Path");
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
                else
                {
                    Product.ProductImagePath = Imagepath;
                }
                _adminRepository.EditProduct(Product);
                HttpContext.Session.Remove("Path");
                return RedirectToAction(nameof(AdminIndex));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5

        [HttpGet]
        public ActionResult DeleteProduct()
        {
            var row = _adminRepository.GetProduct();
            return View(row);
        }

        [HttpGet]
        public ActionResult DeleteProducts(int id)
        {
            _adminRepository.DeleteProduct(id);
            return RedirectToAction("DeleteProduct");
        }

        public IActionResult GetUser()
        {
            return View();
        }
    }
}
