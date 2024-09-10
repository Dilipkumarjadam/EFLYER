using EFLYER.DTOs;
using EFLYER.Models;
using EFLYER.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using Humanizer;

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
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult UserDetails()
        {
            var CurrentSession = HttpContext.Session.GetInt32("UserId");
            var user = _eflyerRepository.GetUserData().Where(x => x.RegId == CurrentSession);
            return View(user);
        }

        [HttpGet]
        public ActionResult EditUserDetails(int id)
        {
            var drop = _eflyerRepository.GetCountry();
            ViewBag.DROP = new SelectList(drop, "CountryId", "CountryName");
            var CurrentSession = HttpContext.Session.GetInt32("UserId");
            var user = _eflyerRepository.GetUserData().Where(x => x.RegId == CurrentSession);
            var User = user.FirstOrDefault();
            return View(User);
        }

        [HttpPost]
        public ActionResult EditUserDetails(RegisteredUserDTO registeredUserDTO, IFormFile IMAGE)
        {
            var CurrentSession = HttpContext.Session.GetInt32("UserId");
            int id = Convert.ToInt32(CurrentSession);

            var drop = _eflyerRepository.GetCountry();
            ViewBag.DROP = new SelectList(drop, "CountryId", "CountryName");

            var CheckEmail = _eflyerRepository.CheckEmail(registeredUserDTO.Email,id, "Update");
            if (CheckEmail == true)
            {
                ViewBag.EmailError = "Email Already Exist Try Other.";
                return View(registeredUserDTO);
            }
            else
            {
                if (IMAGE != null && IMAGE.Length > 0)
                {
                    // Determine the file path and ensure it exists
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImage");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(IMAGE.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file synchronously
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        IMAGE.CopyTo(stream); // Synchronously copy the file
                    }

                    registeredUserDTO.ImagePath = $"/UserImage/{fileName}"; // Set image path in DTO
                }
                _eflyerRepository.EditUserDetails(registeredUserDTO);
                if (HttpContext.Session.GetString("UserSession") != null)
                {
                    HttpContext.Session.Remove("UserSession");
                }
            }
            return RedirectToAction("Login", "Account");
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
