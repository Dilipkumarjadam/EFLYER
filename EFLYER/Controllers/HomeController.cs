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


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string phoneNumber)
        {
            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Send OTP via SMS
            _eflyerRepository.SendSms(phoneNumber, $"Your OTP is {otp}");

            // Store OTP securely (e.g., in-memory cache, database)
            // For demo purposes, we'll skip this step

            return Ok("OTP sent.");
        }

        [HttpGet]
        public IActionResult ForgotPasswordByEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPasswordByEmail(string Email, string Mobile)
        {
            var checkUser = _eflyerRepository.GetUserData().Where(x => x.Email == Email && x.Mobile == Mobile);
            var obj = checkUser.FirstOrDefault();
            var Password = obj.Password;
            if (checkUser != null)
            {
                string subject = "Password Recovery";
                string body = $"Hello Sir,<br/>" +
                              "Your Account Credentials is right here so be chill!<br/>" +
                              $"Email Id: {Email}<br/>" +
                              $"Password: {Password}<br/>" +
                              "You can Login With This EmailId and Password to access the site." +
                              "Thank you!";

                _eflyerRepository.SendEmail(Email, subject, body);
            }
            return View();
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
            var user = _eflyerRepository.GetUserData().Where(x => x.RegId == CurrentSession);

            var path = user.FirstOrDefault();
            var iPath = path.ImagePath;
            int id = Convert.ToInt32(CurrentSession);

            var drop = _eflyerRepository.GetCountry();
            ViewBag.DROP = new SelectList(drop, "CountryId", "CountryName");

            var CheckEmail = _eflyerRepository.CheckEmail(registeredUserDTO.Email, id, "Update");
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
                else
                {
                    registeredUserDTO.ImagePath = iPath;
                }
                _eflyerRepository.EditUserDetails(registeredUserDTO);
                if (HttpContext.Session.GetString("UserSession") != null)
                {
                    HttpContext.Session.Remove("UserSession");
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.passError = "Password Does'nt Matched.";
                return View();
            }
            else
            {
                int UId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                var user = _eflyerRepository.GetUserData().Where(x => x.RegId == UId);
                var p = user.FirstOrDefault();
                if (oldPassword == p.Password)
                {
                    _eflyerRepository.ChangePassword(UId, newPassword);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.WrongPassword = "Password does'nt match with Old Password.";
                    return View();
                }
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
